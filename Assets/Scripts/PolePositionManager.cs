using System.Collections.Generic;
using Mirror;
using UnityEngine;

//public class PolePositionManager : MonoBehaviour
public class PolePositionManager : NetworkBehaviour
{
    public int numPlayers;

    const int MaxPlayers = 4;
    private MyNetworkManager _networkManager;

    private List<PlayerInfo> _players = new List<PlayerInfo>();

    private CircuitController _circuitController;
    private GameObject[] _debuggingSpheres;

    private UIManager _uiManager;

    private void Awake()
    {
        if (_networkManager == null) _networkManager = FindObjectOfType<MyNetworkManager>();
        if (_circuitController == null) _circuitController = FindObjectOfType<CircuitController>();
        if (_uiManager == null) _uiManager = FindObjectOfType<UIManager>();


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
    }

    public void AddPlayer(PlayerInfo player)
    {
        _players.Add(player);
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
}