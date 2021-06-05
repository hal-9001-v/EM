using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class PlayerController : NetworkBehaviour
{
    [SyncVar] private int _startCollider = 0;
    [SyncVar] public bool is_viewer = false;

    #region Variables
    [Header("Movement")] public List<AxleInfo> axleInfos;
    public float forwardMotorTorque = 100000;
    public float backwardMotorTorque = 50000;
    public const float maxSteeringAngle = 15;
    public float engineBrake = 1e+12f;
    public float footBrake = 1e+24f;
    public float topSpeed = 200f;
    public float downForce = 100f;
    public float slipLimit = 0.2f;
    private float CurrentRotation { get; set; }
    private float _inputAcceleration { get; set; }
    private float _inputSteering { get; set; }
    private bool _inputBrake { get; set; }
    public bool _inputPauseMenu { get; set; }

    private PlayerInfo m_PlayerInfo;

    private Rigidbody m_Rigidbody;

    private PolePositionManager _polePositionManager;
    private float m_SteerHelper = 0.8f;

    int _currentCamera;
    CameraController _camera;

    private float m_CurrentSpeed = 0;

    private float Speed
    {
        get { return m_CurrentSpeed; }
        set
        {
            if (Math.Abs(m_CurrentSpeed - value) < float.Epsilon) return;
            m_CurrentSpeed = value;
            if (OnSpeedChangeEvent != null)
                OnSpeedChangeEvent(m_CurrentSpeed);
        }
    }

    public delegate void OnSpeedChangeDelegate(float newVal);

    public event OnSpeedChangeDelegate OnSpeedChangeEvent;

    public int mode;

    #endregion Variables

    #region Unity Callbacks

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PlayerInfo = GetComponent<PlayerInfo>();

        mode = FindObjectOfType<Mode>().mode;


    }


    [Command]
    public void CmdPrepareForMode(int newMode)
    {
        RpcPrepareForMode(newMode);

    }

    [ClientRpc]
    void RpcPrepareForMode(int newMode)
    {
        switch (newMode)
        {
            case 0:

                break;

            case 1:
                foreach (Collider c in GetComponentsInChildren<Collider>())
                {
                    c.enabled = false;
                }

                foreach (Renderer r in GetComponentsInChildren<Renderer>())
                {
                    r.enabled = false;
                }

                foreach (Canvas c in GetComponentsInChildren<Canvas>())
                {
                    c.enabled = false;
                }
                break;

            default:
                break;
        }
    }

    private void HandleModeUpdated(int oldMode, int newMode)
    {

        Debug.Log("Changed to " + newMode);

    }

    public void Start()
    {
        CmdPrepareForMode(mode);

        _polePositionManager = FindObjectOfType<PolePositionManager>();
        _camera = FindObjectOfType<CameraController>();

        if (_camera == null) Debug.Log("Fux");
    }

    public void InitializeInput(BasicPlayer _input)
    {
        _input.PC.Move.performed += ctx =>
        {

            Vector3 rawInput = ctx.ReadValue<Vector2>();

            _inputSteering = rawInput.x;
            _inputAcceleration = rawInput.y;
        };

        _input.PC.Brake.performed += ctx =>
        {
            _inputBrake = true;
        };

        _input.PC.Brake.canceled += ctx =>
        {
            _inputBrake = false;

        };

        _input.PC.Camera.performed += ctx =>
        {
            if (ctx.ReadValue<float>() > 0)
                DisplayNextCamera();
            else
                DisplayPreviousCamera();
        };


        _input.Enable();

    }

    public void Update()
    {
        Speed = m_Rigidbody.velocity.magnitude;

        Debug.Log(_polePositionManager.PlayerTransforms.Count);
    }

    void ApplyMovement(float inputSteering, float inputAcceleration, bool inputBrake)
    {
        float steering = maxSteeringAngle * inputSteering;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            if (axleInfo.motor)
            {
                if (inputAcceleration > float.Epsilon)
                {
                    axleInfo.leftWheel.motorTorque = forwardMotorTorque;
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.motorTorque = forwardMotorTorque;
                    axleInfo.rightWheel.brakeTorque = 0;
                }

                if (inputAcceleration < -float.Epsilon)
                {
                    axleInfo.leftWheel.motorTorque = -backwardMotorTorque;
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.motorTorque = -backwardMotorTorque;
                    axleInfo.rightWheel.brakeTorque = 0;
                }

                if (Math.Abs(inputAcceleration) < float.Epsilon)
                {
                    axleInfo.leftWheel.motorTorque = 0;
                    axleInfo.leftWheel.brakeTorque = engineBrake;
                    axleInfo.rightWheel.motorTorque = 0;
                    axleInfo.rightWheel.brakeTorque = engineBrake;
                }

                if (inputBrake)
                {
                    axleInfo.leftWheel.brakeTorque = footBrake;
                    axleInfo.rightWheel.brakeTorque = footBrake;
                }
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        SteerHelper();
        SpeedLimiter();
        AddDownForce();
        TractionControl();

    }

    [Command]
    void CmdApplyMovement(float steering, float acceleration, bool brake)
    {
        ApplyMovement(steering, acceleration, brake);
    }

    [Client]
    void ClientApplyMovement()
    {
        ApplyMovement(_inputSteering, _inputAcceleration, _inputBrake);
    }


    public void FixedUpdate()
    {
        if (mode == 0)
        {
            CmdApplyMovement(_inputSteering, _inputAcceleration, _inputBrake);
        }

        //ClientApplyMovement();
    }

    #region Rpcs



    #endregion

    #region Server


    #endregion



    #endregion

    #region Methods

    // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    private void TractionControl()
    {
        foreach (var axleInfo in axleInfos)
        {
            WheelHit wheelHitLeft;
            WheelHit wheelHitRight;
            axleInfo.leftWheel.GetGroundHit(out wheelHitLeft);
            axleInfo.rightWheel.GetGroundHit(out wheelHitRight);

            if (wheelHitLeft.forwardSlip >= slipLimit)
            {
                var howMuchSlip = (wheelHitLeft.forwardSlip - slipLimit) / (1 - slipLimit);
                axleInfo.leftWheel.motorTorque -= axleInfo.leftWheel.motorTorque * howMuchSlip * slipLimit;
            }

            if (wheelHitRight.forwardSlip >= slipLimit)
            {
                var howMuchSlip = (wheelHitRight.forwardSlip - slipLimit) / (1 - slipLimit);
                axleInfo.rightWheel.motorTorque -= axleInfo.rightWheel.motorTorque * howMuchSlip * slipLimit;
            }
        }
    }

    // this is used to add more grip in relation to speed
    private void AddDownForce()
    {
        foreach (var axleInfo in axleInfos)
        {
            axleInfo.leftWheel.attachedRigidbody.AddForce(
                -transform.up * (downForce * axleInfo.leftWheel.attachedRigidbody.velocity.magnitude));
        }
    }

    private void SpeedLimiter()
    {
        float speed = m_Rigidbody.velocity.magnitude;
        if (speed > topSpeed)
            m_Rigidbody.velocity = topSpeed * m_Rigidbody.velocity.normalized;
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider col)
    {
        if (col.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = col.transform.GetChild(0);
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);
        var myTransform = visualWheel.transform;
        myTransform.position = position;
        myTransform.rotation = rotation;
    }

    private void SteerHelper()
    {
        foreach (var axleInfo in axleInfos)
        {
            WheelHit[] wheelHit = new WheelHit[2];
            axleInfo.leftWheel.GetGroundHit(out wheelHit[0]);
            axleInfo.rightWheel.GetGroundHit(out wheelHit[1]);
            foreach (var wh in wheelHit)
            {
                if (wh.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(CurrentRotation - transform.eulerAngles.y) < 10f)
        {
            var turnAdjust = (transform.eulerAngles.y - CurrentRotation) * m_SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnAdjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }

        CurrentRotation = transform.eulerAngles.y;
    }

    #endregion

    public int GetEnd()
    {

        return _startCollider;

    }


    void DisplayNextCamera()
    {
        if (mode == 1 && _polePositionManager.players.Count != 0)
        {
            _currentCamera++;

            if (_currentCamera >= _polePositionManager.players.Count)
            {
                _currentCamera = 0;
            }

            //_camera.m_Focus = _polePositionManager.PlayerTransforms[_currentCamera].gameObject;
            FindObjectOfType<CameraController>().m_Focus = _polePositionManager.players[_currentCamera].transform;

        }

    }

    void DisplayPreviousCamera()
    {
        if (mode == 1 && _polePositionManager.players.Count != 0)
        {
            _currentCamera--;

            if (_currentCamera < 0)
            {
                _currentCamera = _polePositionManager.players.Count - 1;
            }

            //_camera.m_Focus = _polePositionManager.PlayerTransforms[_currentCamera].gameObject;
            FindObjectOfType<CameraController>().m_Focus = _polePositionManager.players[_currentCamera].transform;


        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ControlCollider" && int.Parse(other.name) == m_PlayerInfo.NextCollider)
        {

            if (m_PlayerInfo.NextCollider == GetEnd())
            {

                m_PlayerInfo.CurrentLap++;
                Debug.Log("Current Lap = " + m_PlayerInfo.CurrentLap);

            }
            LineRenderer _circuitPath = FindObjectOfType<LineRenderer>();
            int num = CircuitController.GetColliderNumber(_circuitPath.positionCount);

            if (m_PlayerInfo.NextCollider == (num - 1))
            {

                m_PlayerInfo.NextCollider = 0;

            }
            else m_PlayerInfo.NextCollider = int.Parse(other.name) + 1;


            Debug.Log("Next Collider = " + m_PlayerInfo.NextCollider);







        }
    }

}