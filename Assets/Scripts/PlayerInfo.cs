using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [SyncVar] public string publicName;
    
    

    public string Name { get; set; }

    public int ID { get; set; }

    public int CurrentPosition { get; set; }

    public float CurrentArc { get; set; }

    public int CurrentLap { get; set; }

    public Color CurrentColor { get; set; }
    public int NextCollider { get; set; }
    public double TotalTime { get; set; }
    public double LapTime { get; set; }

    public bool WrongWay { get; set; }

    public bool IsReady { get; set; }

    public bool CanMove { get; set; }
    
    public bool IsViewer { get; set; }
    public override string ToString()
    {
        return Name;
    }
    
    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}