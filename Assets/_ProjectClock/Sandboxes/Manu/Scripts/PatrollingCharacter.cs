using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingCharacter : TimeUser
{
    [SerializeField] private Vector3 _startPosition = Vector3.zero;
    [SerializeField] private Vector3 _endPosition = Vector3.zero;
    [SerializeField] private float _velocity = 2f;

    private Rigidbody _rigibody;

    private void Awake()
    {
        _rigibody = GetComponent<Rigidbody>();
    }

    public override void SetConnected(bool isConnected)
    {
        base.SetConnected(isConnected);

        if (!isConnected)
        {
            _rigibody.velocity = Vector3.zero;
        }
    }

    protected override void ProcessAction()
    {
        float targetVelocity = _velocity * (1 / Mathf.Abs(TimeManager.Instance.CurrentTimeMod));
        _rigibody.MovePosition(transform.position + _velocity * targetVelocity * Time.deltaTime * TimeManager.TickingSign * transform.right);

        Debug.Log($"Distance to starting point : {Vector3.Distance(transform.position, _startPosition)}");
        if (TimeManager.TickingSign > 0)
        {
            if (Vector3.Distance(transform.position, _endPosition) < 0.1f)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (Vector3.Distance(transform.position, _startPosition) < 0.1f)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else if(TimeManager.TickingSign < 0)
        {
            if (Vector3.Distance(_endPosition, transform.position) < 0.1f)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (Vector3.Distance(_startPosition, transform.position) < 0.1f)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
}
