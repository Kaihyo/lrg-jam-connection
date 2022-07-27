using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adir_ClockTicking : MonoBehaviour
{
    public Transform arrow;
    public bool ticking = true;

    public GameObject obj01;
    public GameObject obj02;

    public Transform arrowStart;
    public Transform arrowEnd;
        
    public LineBetweenObjects line;

    private float _curSecond = 0.0f;
    
    void Start()
    {
        TimeManager.OnMinutesChanged += UpdateTime;

        if (line != null)
        {
            line.SetObjects(obj01, obj02);
        }
    }

    private void Update()
    {
        Vector2 arrowVector = (arrowEnd.position - arrowStart.position).normalized;
        float angle;
        
        if (Vector2.Dot(Vector2.right, arrowVector) < 0)
        {
            angle = Mathf.Ceil(360.0f - Vector2.Angle(Vector2.up, arrowVector));
            _curSecond = Mathf.Ceil((60.0f/360.0f) * angle);
        }
        else
        {
            angle = Mathf.Ceil(Vector2.Angle(Vector2.up, arrowVector));
            _curSecond = Mathf.Ceil((60.0f/360.0f) * angle);
        }
    }

    private void UpdateTime()
    {
        if (ticking)
        {
            float ratio = -360.0f / 60.0f;
            // (Axel) je rajoute ça pour que la clock puisse tick a l'envers
            ratio *= TimeManager.TickingSign;
            arrow.Rotate(Vector3.forward, ratio);
        }
    }
}
