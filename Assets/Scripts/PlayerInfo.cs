﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public string Name { get; set; }

    public int ID { get; set; }

    public int CurrentPosition { get; set; }

    public int CurrentLap { get; set; }

    public Color CurrentColor { get; set; }
    public int NextCollider { get; set; }

    public bool WrongWay { get; set; }

    public bool IsReady { get; set; }

    public bool CanMove { get; set; }

    public override string ToString()
    {
        return Name;
    }
}