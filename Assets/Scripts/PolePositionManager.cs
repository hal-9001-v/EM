using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

//public class PolePositionManager : NetworkBehaviour
public class PolePositionManager : NetworkBehaviour
{
    const int MaxPlayers = 4;
    private MyNetworkManager _networkManager;

    public List<PlayerInfo> _players = new List<PlayerInfo>();

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

        if (!_isRaceInProgress && isServer)
        {
            ArePlayersReady();
        }
    }

    public void AddPlayer(PlayerInfo player)
    {
        if (_players.Count < MaxPlayers)
        {
            _players.Add(player);
        }
        else
        {
            Debug.Log("CARRERA LLENA");
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

    public string GetRaceProgress()
    {
        // Update car arc-lengths
        float[] arcLengths = new float[MaxPlayers];


        for (int i = 0; i < _players.Count; ++i)
        {
            if (_players[i] != null)
            {
                arcLengths[i] = ComputeCarArcLength(i);
            }
        }

        _players.Sort(new PlayerInfoComparer(arcLengths));

        string raceOrder = "";
        foreach (var player in _players)
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
        Vector3 carPos = this._players[id].transform.position;

        int segIdx;
        float carDist;
        Vector3 carProj;
        float minArcL =
            this._circuitController.ComputeClosestPointArcLength(carPos, out segIdx, out carProj, out carDist);

        this._debuggingSpheres[id].transform.position = carProj;
        if (this._players[id].CurrentLap == 0)
        {
            minArcL -= _circuitController.CircuitLength;
        }
        else
        {
            minArcL += _circuitController.CircuitLength *
                       (_players[id].CurrentLap - 1);
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
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].IsReady) count++;
        }

        if (count > _players.Count / 2 && _players.Count >= 2)
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
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].CanMove = isActiveMovement;
        }
    }

    [ClientRpc]
    private void RpcSetMovementToPlayers()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].CanMove = isActiveMovement;
        }

        _uiManager.myChangingPlayer.gameObject.GetComponent<PlayerController>()
            .CmdSetRaceStartTime(_timer.GetCurrentServerTime());
    }
}