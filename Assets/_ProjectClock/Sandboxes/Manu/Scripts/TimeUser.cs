using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimeUser : MonoBehaviour
{
    [SerializeField] protected bool _isConnected = true;
    [SerializeField] protected Transform _connectionAnchor;

    public Transform ConnectionAnchor => _connectionAnchor;

    private void Start()
    {
        if(_isConnected)
        {
            TimeManager.OnMinutesChanged += ProcessAction;
        }
    }

    private void OnDisable()
    {
        SetConnected(false);
    }

    private void FixedUpdate()
    {
        if(_isConnected && TimeManager.IsRunning)
        {
            ProcessAction();
        }
    }

    public virtual void SetConnected(bool isConnected)
    {
        _isConnected = isConnected;

        if(isConnected)
        {
            TimeManager.OnMinutesChanged += ProcessAction;
        }
        else
        {
            TimeManager.OnMinutesChanged -= ProcessAction;
        }
    }

    protected abstract void ProcessAction();
}
