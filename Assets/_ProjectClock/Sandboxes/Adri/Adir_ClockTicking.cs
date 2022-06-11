using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adir_ClockTicking : MonoBehaviour
{
    public Transform arrow;
    public bool ticking = true;

    public GameObject obj01;
    public GameObject obj02;
        
    public LineBetweenObjects line;
    
    void Start()
    {
        TimeManager.OnMinutesChanged += UpdateTime;
        line.SetObjects(obj01, obj02);
    }

    private void UpdateTime()
    {
        if (ticking)
        {
            float ratio = -360.0f / 60.0f;
            arrow.Rotate(Vector3.forward, ratio);
        }
    }
}
