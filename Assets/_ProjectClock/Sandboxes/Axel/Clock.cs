using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{

    public TextMeshProUGUI timeText;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        TimeManager.OnMinutesChanged += UpdateTime;
        TimeManager.OnHoursChanged += UpdateTime;
    }

    private void OnDisable()
    {
        TimeManager.OnMinutesChanged -= UpdateTime;
        TimeManager.OnHoursChanged -= UpdateTime;
    }

    // Update is called once per frame
    private void UpdateTime()
    {
        timeText.text = $"{TimeManager.Hours:00}:{TimeManager.Minutes:00}";
    }
}
