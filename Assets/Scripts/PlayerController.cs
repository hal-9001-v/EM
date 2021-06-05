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
    private int _startCollider = 0;
    [SyncVar (hook = nameof(HandleCurrentCheckPointCheck))] private int _currentCheckPoint;
    [SyncVar (hook = nameof(HandleCurrentLapCheck))] private int _currentLap;
    [SyncVar (hook = nameof(HandleWrongWayCheck))] private bool _wrongWay;

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
    public UIManager m_UImanager;

    private Rigidbody m_Rigidbody;
    private float m_SteerHelper = 0.8f;


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

    #endregion Variables

    #region Unity Callbacks

    public void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PlayerInfo = GetComponent<PlayerInfo>();


    }

  
    private void Start() {
        if(isLocalPlayer) m_UImanager = FindObjectOfType<UIManager>();
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

          
            _input.Enable();

        }


   
        public void Update()
        {
            
            Speed = m_Rigidbody.velocity.magnitude;
            
            m_UImanager.UpdateCurrentLap(_currentLap, 4);
            m_UImanager.UpdateWarning(_wrongWay);

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
            CmdApplyMovement(_inputSteering, _inputAcceleration, _inputBrake);
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

        public int  GetEnd(){

            return _startCollider;

        }
        

        private void HandleCurrentCheckPointCheck(int oldCheckPoint, int newCheckPoint){

            Debug.Log("My player info checkpoint is " + m_PlayerInfo.NextCollider);
            m_PlayerInfo.NextCollider = newCheckPoint;
           
        }

        private void HandleCurrentLapCheck(int oldLap, int newLap){

            Debug.Log("My player info current lap is " + m_PlayerInfo.CurrentLap);
            m_PlayerInfo.CurrentLap = newLap;

        }

        private void HandleWrongWayCheck(bool oldBool, bool newBool){

            Debug.Log("Wrong Way!");
            m_PlayerInfo.WrongWay = newBool;

        }


        [Command]
        public void CmdCheckPointCheck(string name){

            ServerCheckPointCheck(name);
        }


        [Server]
        public void ServerCheckPointCheck(string name){

           
                if(int.Parse(name) == _currentCheckPoint){
                    Debug.Log("Current Collider = " + name);

                    
                    if(_currentLap != 0) _wrongWay = false;

                    //Check if next collider is last collider 
                    if(_currentCheckPoint == GetEnd()){
                        
                        //Update laps
                        _currentLap++;
                        Debug.Log("Current Lap = " + _currentLap);

                        }
                    
                    //Get total chekpoints
                    LineRenderer _circuitPath = FindObjectOfType<LineRenderer>();
                    int num = CircuitController.GetColliderNumber(_circuitPath.positionCount);

                    //if next checkpoint is the last checkpoint, the next collider becomes first checkpoint
                    if(_currentCheckPoint == (num-1))
                    {
                        _currentCheckPoint = 0;
                        
                    }//Else nex checkpoint = currentcheckpooint++ 
                    else _currentCheckPoint = int.Parse(name) +1;

                    Debug.Log("Next Collider = " + _currentCheckPoint);
      
                } else {
                    if(_currentLap != 0) _wrongWay = true;
                    Debug.Log("Wrong Way!!!!");
                    
                }
               

        }

        private void OnTriggerEnter(Collider other) {
            
            
            //If i collide with trigger -> checkpoint && it's name is my next collideer
            if(other.tag == "ControlCollider") {
                string s = other.name;
                
                if(isLocalPlayer) {
                    CmdCheckPointCheck(s);
                }
            }
        }

    }