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

    protected override void ProcessAction()
    {
        _rigibody.velocity = _velocity * transform.right;
        if (Vector3.Distance(transform.position, _endPosition) < 0.1f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (Vector3.Distance(transform.position, _startPosition) < 0.1f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
