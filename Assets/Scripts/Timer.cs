using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleCurrentTime))] [SerializeField] private double _currentTime = 0;
    [SerializeField] public double _currentClientTime = 0;
    
    private void HandleCurrentTime(double oldTime, double newCurrentTime)
    {
        _currentClientTime = newCurrentTime;
    }

    [Server]
    private void UpdateCurrentTime()
    {
        _currentTime = NetworkTime.time;
    }

    [Server]
    public void ResetTimer()
    {
        _currentTime = 0;
    }

    private void Update()
    {
        UpdateCurrentTime();
    }
    
    
}
