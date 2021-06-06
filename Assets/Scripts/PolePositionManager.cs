using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

//public class PolePositionManager : NetworkBehaviour
public class PolePositionManager : NetworkBehaviour
{
    private const int MaxPlayers = 4;
    
    public int MaxLaps = 2;

    private const int MinimumNamberOfPlayers = 1;

    private MyNetworkManager _networkManager;

    private List<PlayerInfo> _players = new List<PlayerInfo>();
    public List<PlayerInfo> _playersInRace = new List<PlayerInfo>();
    public SyncList<PlayerInfo> _spectators = new SyncList<PlayerInfo>();


    private CircuitController _circuitController;
    private GameObject[] _debuggingSpheres;

    private UIManager _uiManager;

    private Timer _timer;

    [SyncVar] private bool _isRaceInProgress;

    [SyncVar] public bool isActiveMovement = false;


    [SyncVar] public bool arePlayersReady;

    private bool countDownStarted;

    double threshHold;

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
        _uiManager.UpdateRaceRank(GetRaceProgress());

        if (isServer)
        {
            if (_isRaceInProgress)
            {
                string raceProgress = GetRaceProgress();

                _uiManager.UpdateRaceRank(raceProgress);

                RpcUpdateRaceProgress(raceProgress);
            }
            else ArePlayersReady();
        }

        UpdateClientLists();
    }
    
    [ClientRpc]
    void RpcUpdateRaceProgress(string raceProgress)
    {
        _uiManager.UpdateRaceRank(raceProgress);

    }


    public void AddPlayer(PlayerInfo player)
    {
        _players.Add(player);
        
    }



    [Server]
    public void UpdateClientLists()
    {
        foreach (var player in _playersInRace)
        {
            _spectators.Add(player);
        }
    }


    public int GetPlayerCount()
    {
        return _players.Count;
    }

    public void RemovePlayer(PlayerInfo player)
    {
        _players.Remove(player);
    }

    private class PlayerInfoComparer : Comparer<PlayerInfo>
    {
        float[] _arcLengths;

        public PlayerInfoComparer(float[] arcLengths)
        {
            _arcLengths = arcLengths;
        }

        public override int Compare(PlayerInfo x, PlayerInfo y)
        {
            if (_arcLengths[x.ID] < _arcLengths[y.ID])
                return 1;
            else return -1;
        }
    }

    [Server]
    public string GetRaceProgress()
    {
        // Update car arc-lengths
        float[] arcLengths = new float[MaxPlayers];


        for (int i = 0; i < _playersInRace.Count; ++i)
        {
            if (_playersInRace[i] != null)
            {
                arcLengths[i] = ComputeCarArcLength(i);
            }
        }

        _playersInRace.Sort(new PlayerInfoComparer(arcLengths));

        string raceOrder = "";
        foreach (var player in _playersInRace)
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
        Vector3 carPos = this._playersInRace[id].transform.position;

        int segIdx;
        float carDist;
        Vector3 carProj;
        float minArcL =
            this._circuitController.ComputeClosestPointArcLength(carPos, out segIdx, out carProj, out carDist);

        this._debuggingSpheres[id].transform.position = carProj;
        if (this._playersInRace[id].CurrentLap == 0)
        {
            minArcL -= _circuitController.CircuitLength;
        }
        else
        {
            minArcL += _circuitController.CircuitLength *
                       (_playersInRace[id].CurrentLap - 1);
        }

        return minArcL;
    }

    [ClientRpc]
    public void RpcStarCountDownUI()
    {
        _uiManager.ActivateCountDown();
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1f);
        _uiManager.UpdateTextCountDown("3");
        yield return new WaitForSeconds(1f);
        _uiManager.UpdateTextCountDown("2");
        yield return new WaitForSeconds(1f);
        _uiManager.UpdateTextCountDown("1");
        yield return new WaitForSeconds(1f);
        _uiManager.UpdateTextCountDown("GO");
        yield return new WaitForSeconds(1f);
        _uiManager.ActivateInGameHUD();
    }

    [Server]
    private void ArePlayersReady()
    {
        int count = 0;
        for (int i = 0; i < _playersInRace.Count; i++)
        {
            if (_playersInRace[i].IsReady) count++;
        }

        if (count > _playersInRace.Count / 2 && _playersInRace.Count >= MinimumNamberOfPlayers)
        {
            _isRaceInProgress = true;
            RpcStarCountDownUI();

            StartCoroutine(CountDown2());
        }
    }

    private IEnumerator CountDown2()
    {
        yield return new WaitForSeconds(5f);
        isActiveMovement = true;
        SetMovementToPlayers();
    }

    private void SetMovementToPlayers()
    {
        RpcSetMovementToPlayers();
        for (int i = 0; i < _playersInRace.Count; i++)
        {
            _playersInRace[i].CanMove = isActiveMovement;
        }
    }

    [ClientRpc]
    private void RpcSetMovementToPlayers()
    {
        for (int i = 0; i < _playersInRace.Count; i++)
        {
            _playersInRace[i].CanMove = isActiveMovement;
        }

    }

    [Server]
    public void EndRace()
    {
        isActiveMovement = false;
        RpcEndRace();
        for (int i = 0; i < _playersInRace.Count; i++)
        {
            _playersInRace[i].CanMove = isActiveMovement;
        }
        

    }
    
    [ClientRpc]
    public void RpcEndRace()
    {
        for (int i = 0; i < _playersInRace.Count; i++)
        {
            _playersInRace[i].CanMove = isActiveMovement;
        }
    }
}