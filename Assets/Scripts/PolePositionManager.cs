﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class PolePositionManager : NetworkBehaviour
{
    public int numPlayers;

    const int MaxPlayers = 4;
    private MyNetworkManager _networkManager;

    public List<PlayerInfo> players = new List<PlayerInfo>();



    private CircuitController _circuitController;
    private GameObject[] _debuggingSpheres;

    private UIManager _uiManager;

    private Timer _timer;
    [SerializeField]private double startingRaceTime;

    [SerializeField] [SyncVar (hook = (nameof(HandleTimerUpdate)))] private double _currentTime;

    [SerializeField]private double myCurrentTime;


    [SyncVar(hook = nameof(HandleIsRaceInProgress))]
    private bool isRaceInProgress = false;
    [SerializeField] private bool isClientRaceInProgress;

    [SyncVar(hook = nameof(HandleActiveMovement))]
    public bool isActiveMovement = false;

    [SerializeField] public bool isActiveClientMovement;

    private void Awake()
    {
        if (_networkManager == null) _networkManager = FindObjectOfType<MyNetworkManager>();
        if (_circuitController == null) _circuitController = FindObjectOfType<CircuitController>();
        if (_uiManager == null) _uiManager = FindObjectOfType<UIManager>();

        if (_timer == null) _timer = FindObjectOfType<Timer>();

        _debuggingSpheres = new GameObject[_networkManager.maxConnections];
        for (int i = 0; i < _networkManager.maxConnections; ++i)
        {
            _debuggingSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _debuggingSpheres[i].GetComponent<SphereCollider>().enabled = false;
        }
    }


    private void Update()
    {

        if (!isClientRaceInProgress && ArePlayersReady() && players.Count >= 2)
        {
            ActiveRace();
            StarCountDownUI();
        }
        
        if(isLocalPlayer) {
            UIHandle();
            CmdUpdateTimer();
            ServerUpdateTimer();
        }

        // _uiManager.UpdateRaceRank(GetRaceProgress());

    }

    public void UIHandle(){
        
        CmdUpdateTimer();

    }

    private void HandleTimerUpdate(double oldDouble, double newDouble){

        myCurrentTime = newDouble;
        _uiManager.UpdateLapTime(newDouble);
        _uiManager.UpdateTotalTime(newDouble);

    }

    [Command]
    public void CmdUpdateTimer(){

        ServerUpdateTimer();

    }

    [Server]
    public void ServerUpdateTimer(){

        _currentTime = _timer.GetCurrentServerTime() - startingRaceTime;
        Debug.Log(_currentTime);

    }

    public double GetCurrentRaceTime(){

        return _currentTime;

    }
    public void AddPlayer(PlayerInfo player)
    {

        if (players.Count < 4)
        {
            players.Add(player);

        }
        else
        {
            Debug.Log("CARRERA LLENA");
        }

    }



    public void RemovePlayer(PlayerInfo player)
    {
        players.Remove(player);
        //PlayerTransforms.Remove(player.transform);

    }

    public string GetRaceProgress()
    {
        // Update car arc-lengths
        float[] arcLengths = new float[MaxPlayers];

        for (int i = 0; i < players.Count; i++)
        {
            players[i].CurrentArc = ComputeCarArcLength(i);

        }
        players.Sort((a, b) => b.CurrentArc.CompareTo(a.CurrentArc));

        string raceOrder = "";
        foreach (var player in players)
        {
            if (player != null)
            {
                raceOrder += player.Name + " ";
            }
        }

        return raceOrder;
    }

    float ComputeCarArcLength(int id)
    {
        // Compute the projection of the car position to the closest circuit 
        // path segment and accumulate the arc-length along of the car along
        // the circuit.
        Vector3 carPos = this.players[id].transform.position;

        int segIdx;
        float carDist;
        Vector3 carProj;
        float minArcL =
            this._circuitController.ComputeClosestPointArcLength(carPos, out segIdx, out carProj, out carDist);

        this._debuggingSpheres[id].transform.position = carProj;


        if (this.players[id].CurrentLap == 0)
        {
            minArcL -= _circuitController.CircuitLength;
        }
        else
        {
            minArcL += _circuitController.CircuitLength *
                       (players[id].CurrentLap - 1);
        }

        return minArcL;
    }

    public void StarCountDownUI()
    {
        _uiManager.ActivateCountDown();

        StartCoroutine(ChangeNumbersCountDown());
    }

    IEnumerator ChangeNumbersCountDown()
    {
        yield return new WaitForSeconds(1);

        _uiManager.UpdateTextCountDown("3");

        yield return new WaitForSeconds(1);

        _uiManager.UpdateTextCountDown("2");

        yield return new WaitForSeconds(1);

        _uiManager.UpdateTextCountDown("1");

        yield return new WaitForSeconds(1);

        _uiManager.UpdateTextCountDown("GO");

        ActiveMovement();
        _uiManager.ActivateInGameHUD();
    }

    [Server]
    private bool ArePlayersReady()
    {
        int count = 0;
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log(players[i].IsReady);
            if (players[i].IsReady) count++;
        }

        if (count > players.Count / 2) return true;
        else return false;
    }

    [Server]
    private void ActiveRace()
    {
        isRaceInProgress = true;
        startingRaceTime = _timer.GetCurrentServerTime();
    }

    private void HandleIsRaceInProgress(bool oldActiveRace, bool newActiveRace)
    {
        isClientRaceInProgress = newActiveRace;
    }

    [Server]
    private void ActiveMovement()
    {
        isActiveMovement = true;
    }

    private void HandleActiveMovement(bool oldActiveMovement, bool newActiveMovement)
    {
        isActiveClientMovement = newActiveMovement;
    }
}