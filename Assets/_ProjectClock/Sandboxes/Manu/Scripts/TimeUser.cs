using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimeUser : MonoBehaviour
{
    [SerializeField] protected bool _isConnected = true;
    [SerializeField] protected Transform _connectionPoint;

    public Transform ConnectionPoint => _connectionPoint;

    private void FixedUpdate()
    {
        if(_isConnected && TimeManager.IsRunning)
        {
            ProcessAction();
        }
    }

    public void SetConnected(bool isConnected)
    {
        _isConnected = isConnected;
    }

    protected abstract void ProcessAction();
}
