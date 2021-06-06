﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class PlayerController : NetworkBehaviour
{
    private GameObject _startCollider;

    [SyncVar(hook = nameof(HandleCurrentCheckPointCheck))]
    private int _currentCheckPoint;

    [SyncVar(hook = nameof(HandleCurrentLapCheck))]
    private int _currentLap;

    [SyncVar(hook = nameof(HandleWrongWayCheck))]
    private bool _wrongWay;

    [SyncVar(hook = nameof(HandleLastChekcpointTransform))]
    private Vector3 _myFixPos;

    [SyncVar] private bool _canMove = false;


    Timer t;
    private double teleportThreshhold = 5;
    private double currentThreshhold = 0;
    private bool isCounting = false;
    private double startingThreshholdTime;

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


    private PlayerInfo m_PlayerInfo;
    public UIManager m_UImanager;
    public SetupPlayer m_SetupPlayer;
    private Rigidbody m_Rigidbody;

    private float m_SteerHelper = 0.8f;


    private float m_CurrentSpeed = 0;

    public int mode;

    int _currentCamera;

    [SyncVar] public bool isViewer = false;
    CameraController _camera;
    private PolePositionManager _polePositionManager;

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

    #endregion Variables

    #region Unity Callbacks

    public void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PlayerInfo = GetComponent<PlayerInfo>();
        m_SetupPlayer = GetComponent<SetupPlayer>();
        t = FindObjectOfType<Timer>();
        _startCollider = GameObject.Find("0");

    }


    private void Start()
    {

        if (isLocalPlayer)
        {
            m_UImanager = FindObjectOfType<UIManager>();
            _polePositionManager = FindObjectOfType<PolePositionManager>();
            _camera = FindObjectOfType<CameraController>();
            CmdCheckPointCheck(_startCollider.name);

            if (m_UImanager.playerIsViewer)
            {
                mode = 1;
                CmdSetViewer();

            }
            else mode = 0;

            CmdPrepareForMode(mode);
        }

    }

    [Command]
    void CmdSetViewer()
    {
        isViewer = true;
    }

    public void InitializeInput(BasicPlayer _input)
    {
        _input.PC.Move.performed += ctx =>
        {
            Vector3 rawInput = ctx.ReadValue<Vector2>();

            _inputSteering = rawInput.x;
            _inputAcceleration = rawInput.y;
        };

        _input.PC.Brake.performed += ctx => { _inputBrake = true; };

        _input.PC.Brake.canceled += ctx => { _inputBrake = false; };

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

        m_UImanager.UpdateCurrentLap(_currentLap, 4);
        m_UImanager.UpdateWarning(_wrongWay);


    }

    [Server]
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
        if (!m_PlayerInfo.CanMove) return;
        ApplyMovement(steering, acceleration, brake);
        if (isServer)
        {
            //Debug.Log("Your current server time is " + t.GetCurrentServerTime() + " and the threshold is " + currentThreshhold + " and you are currently counting " + isCounting);
            if ((m_Rigidbody.velocity.magnitude < 0.5)) CalculateTeleport();
            else
            {
                isCounting = false;
                currentThreshhold = t.GetCurrentServerTime() + teleportThreshhold;
            }
        }
    }

    [Client]
    void ClientApplyMovement()
    {
        ApplyMovement(_inputSteering, _inputAcceleration, _inputBrake);
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

    public void FixedUpdate()
    {
        CmdApplyMovement(_inputSteering, _inputAcceleration, _inputBrake);
        //ClientApplyMovement();


    }

    [Server]
    private void CalculateTeleport()
    {
        if (!isCounting)
        {
            isCounting = true;
            startingThreshholdTime = t.GetCurrentServerTime();
            currentThreshhold = startingThreshholdTime + teleportThreshhold;

        }


        if (t.GetCurrentServerTime() >= currentThreshhold)
        {

            int lookAux = GetNextCheckPoint();
            int posAux = lookAux - 1;
            GameObject temp = GameObject.Find(posAux.ToString());
            GameObject look = GameObject.Find(lookAux.ToString());
            Teleport(temp, look);
            currentThreshhold = t.GetCurrentServerTime() + 30;
            isCounting = false;

        }

    }


    private void Teleport(GameObject temp, GameObject look)
    {


        gameObject.transform.position = temp.transform.position;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(gameObject.transform.rotation.x, 0, gameObject.transform.rotation.z));
        gameObject.transform.LookAt(look.transform);


    }

    private int GetNextCheckPoint()
    {

        return _currentCheckPoint;

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
        return int.Parse(_startCollider.name);
    }


    private void HandleCurrentCheckPointCheck(int oldCheckPoint, int newCheckPoint)
    {
        Debug.Log("My player info checkpoint is " + m_PlayerInfo.NextCollider);
        m_PlayerInfo.NextCollider = newCheckPoint;
    }

    private void HandleCurrentLapCheck(int oldLap, int newLap)
    {
        Debug.Log("My player info current lap is " + m_PlayerInfo.CurrentLap);
        m_PlayerInfo.CurrentLap = newLap;
    }

    private void HandleWrongWayCheck(bool oldBool, bool newBool)
    {
        Debug.Log("Wrong Way!");
        m_PlayerInfo.WrongWay = newBool;
    }
    private void HandleLastChekcpointTransform(Vector3 oldPos, Vector3 newPos)
    {
        Debug.Log("Position Reset!");

    }


    [Command]
    public void CmdCheckPointCheck(string name)
    {
        ServerCheckPointCheck(name);
    }



    [Server]
    public void ServerCheckPointCheck(string name)
    {
        if (int.Parse(name) == _currentCheckPoint)
        {
            Debug.Log("Current Collider = " + name);

            if (_currentLap != 0) _wrongWay = false;

            //Check if next collider is last collider 
            if (_currentCheckPoint == GetEnd())
            {
                //Update laps
                _currentLap++;
                Debug.Log("Current Lap = " + _currentLap);
            }

            //Get total chekpoints
            LineRenderer _circuitPath = FindObjectOfType<LineRenderer>();
            int num = CircuitController.GetColliderNumber(_circuitPath.positionCount);

            //if next checkpoint is the last checkpoint, the next collider becomes first checkpoint
            if (_currentCheckPoint == (num - 1))
            {
                _currentCheckPoint = 0;
            } //Else nex checkpoint = currentcheckpooint++ 
            else _currentCheckPoint = int.Parse(name) + 1;

            Debug.Log("Next Collider = " + _currentCheckPoint);
        }
        else
        {
            if (_currentLap != 0 && _currentCheckPoint != 1) _wrongWay = true;
            Debug.Log("Wrong Way!!!!");
        }
    }


    void DisplayNextCamera()
    {
        if (mode == 1)
        {
            List<PlayerInfo> players = _polePositionManager.players;

            //This is terrible but, this is it.
            players.FindAll((p) =>
            {
                if (p.GetComponent<PlayerController>().isViewer) return true;

                return false;

            });


            if (players.Count != 0)
            {

                _currentCamera++;

                if (_currentCamera >= players.Count)
                {
                    _currentCamera = 0;
                }

                //_camera.m_Focus = _polePositionManager.PlayerTransforms[_currentCamera].gameObject;
                _camera.m_Focus = _polePositionManager.players[_currentCamera].transform;
            }


        }

    }

    void DisplayPreviousCamera()
    {
        if (mode == 1)
        {
            List<PlayerInfo> players = _polePositionManager.players;

            players.FindAll((p) =>
            {
                if (p.GetComponent<PlayerController>().isViewer) return true;

                return false;

            });


            if (players.Count != 0)
            {

                _currentCamera--;

                {
                    _currentCamera = players.Count - 1;
                }


                _camera.m_Focus = _polePositionManager.players[_currentCamera].transform;
            }


        }


    }

    private void OnTriggerEnter(Collider other)
    {
        //If i collide with trigger -> checkpoint && it's name is my next collideer
        if (other.tag == "ControlCollider")
        {
            string s = other.name;
            if (isLocalPlayer)
            {
                CmdCheckPointCheck(s);
            }
        }
    }
}