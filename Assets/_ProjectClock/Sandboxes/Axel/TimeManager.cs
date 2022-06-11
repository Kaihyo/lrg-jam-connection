using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    static public Action OnHoursChanged;
    static public Action OnMinutesChanged;

    static public int Hours {  get; private set; }
    static public int Minutes {  get; private set; }
    
    private float timer;
    
    //Probbablement quelque chose du genre 1 min IG = 0.5s IRL 
    public float minToRealTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        timer = Math.Abs(minToRealTime);
        Minutes = 0;
        Hours = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            if (minToRealTime < 0) //Time steps backward
            {
                Minutes--;
            }
            else //Time steps forwards
            {
                Minutes++;
            }
            
            OnMinutesChanged?.Invoke();
            
            if (Minutes >= 60)
            {
                Hours++;
                Minutes = 0;

                if (Hours >= 24)
                {
                    Hours = 0;
                }
                OnHoursChanged?.Invoke();
            }
            else if (Minutes < 0)
            {
                Hours--;
                Minutes = 59;

                if (Hours < 0)
                {
                    Hours = 23;
                }
                OnHoursChanged?.Invoke();
            }
            timer = Math.Abs(minToRealTime);
        }
    }
}
