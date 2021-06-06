using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleCurrentTime))] [SerializeField]
    private double _currentTime = 0;

    [SerializeField] private double _currentClientTime = 0;

    private void HandleCurrentTime(double oldTime, double newCurrentTime)
    {
        _currentClientTime = newCurrentTime;
    }

    [Command]
    private void CmdUpdateCurrentTime()
    {
        UpdateCurrentTime();
    }

    [Server]
    private void UpdateCurrentTime()
    {
        _currentTime = NetworkTime.time;
    }

    private void Update()
    {
        if (isServer) UpdateCurrentTime();
    }

    public double GetCurrentClientTime()
    {
        return _currentClientTime;
    }

    public double GetCurrentServerTime()
    {
        return _currentTime;
    }
}