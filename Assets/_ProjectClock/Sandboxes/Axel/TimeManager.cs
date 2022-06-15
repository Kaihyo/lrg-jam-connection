using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // (Axel) Probablement quelque chose du genre 1 min IG = 0.5s IRL
    [SerializeField] private float[] _minToRealTime = new float[]{-0.05f,-0.1f,-0.5f,-1f,1f,0.5f,0.1f,0.05f};
    [SerializeField] [Range(0,7)] private int indexTimeSpeed = 4;

    private float _timer;

    public static Action OnMinutesChanged;
    public static Action OnHoursChanged;

    public static bool IsRunning { get; private set; }
    public static int Minutes { get; private set; }
    public static int Hours { get; private set; }
    
    //(Axel) Pour exporter le sens d'ecoulement tu temps
    public static int TickingSign { get; private set; }
    private void OnEnable()
    {
        ConnectionUi.OnToggleDisplay += OnConnectUiDisplay;
    }

    private void OnDisable()
    {
        ConnectionUi.OnToggleDisplay -= OnConnectUiDisplay;
    }

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        ProcessTime();
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SetTimerRunning(!IsRunning);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(indexTimeSpeed<7)
                indexTimeSpeed++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(indexTimeSpeed>0)
                indexTimeSpeed--;
        }
    }

    // (Manu) Je le passe en fonction si jamais on veut qu'un composant externe ait la main sur le lancement du temps
    public void StartTimer()
    {
        _timer = Math.Abs(_minToRealTime[indexTimeSpeed]);
        Minutes = 0;
        Hours = 0;
        IsRunning = true;
    }

    // (Manu) Si on veut mettre le jeu en pause sans modifier _minToRealTime
    public void SetTimerRunning(bool isRunning)
    {
        IsRunning = isRunning;
    }

    private void ProcessTime()
    {
        // (Manu) J'etais pas sur du comportement qu'on voulait pour le _minToRealTime
        // Dans le doute j'ai mis une garde quand il est a 0 pour eviter de spammer les event a chaque frame
        if (!IsRunning || _minToRealTime[indexTimeSpeed] == 0)
        {
            return;
        }

        _timer -= Time.deltaTime;

        if(_timer <= 0)
        {
            TickingSign = Math.Sign(_minToRealTime[indexTimeSpeed]);
            Minutes += Math.Sign(_minToRealTime[indexTimeSpeed]); // (Manu) Juste pour simplifier le if-else du code d'origine
            OnMinutesChanged?.Invoke();

            if(Minutes >= 60)
            {
                Minutes = 0;

                Hours++;
                if(Hours >= 24)
                {
                    Hours = 0;
                }
                OnHoursChanged?.Invoke();
            }
            else if(Minutes < 0)
            {
                Minutes = 59;

                Hours--;
                if(Hours < 0)
                {
                    Hours = 23;
                }
                OnHoursChanged?.Invoke();
            }

            _timer = Math.Abs(_minToRealTime[indexTimeSpeed]);
        }
    }

    private void OnConnectUiDisplay(bool isDisplayed)
    {
        SetTimerRunning(!isDisplayed);
    }
}
