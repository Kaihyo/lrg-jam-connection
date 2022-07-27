using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager _instance;

    // (Axel) Probablement quelque chose du genre 1 min IG = 0.5s IRL
    [SerializeField] private float[] _minToRealTime = new float[]{-0.05f,-0.1f,-0.5f,-1f,1f,0.5f,0.1f,0.05f};
    [SerializeField] [Range(0,5)] private int indexTimeSpeed = 3;

    private float _timer;

    public static Action OnMinutesChanged;
    public static Action OnHoursChanged;
    
    // Des trucs pour l'affichage du selecteur
    public RectTransform TimeSelectorGraphics;
    private float[] angleToRotate= new float[]{67.5f,45f,22.5f,-22.5f,-45f,-67.5f};
    public static bool IsRunning { get; private set; }
    public static int Minutes { get; private set; }
    public static int Hours { get; private set; }
    
    //(Axel) Pour exporter le sens d'ecoulement tu temps
    public static int TickingSign { get; private set; }

    public static TimeManager Instance => _instance;
    public float CurrentTimeMod => _minToRealTime[indexTimeSpeed];

    private void OnEnable()
    {
        ConnectionUi.OnToggleDisplay += OnConnectUiDisplay;
    }

    private void OnDisable()
    {
        ConnectionUi.OnToggleDisplay -= OnConnectUiDisplay;
    }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if(_instance == this)
        {
            _instance = null;
        }
    }

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SetTimerRunning(!IsRunning);
            if(!IsRunning)
                TimeSelectorGraphics.rotation = Quaternion.Euler(0, 0, 0);
            else 
                TimeSelectorGraphics.rotation = Quaternion.Euler(0, 0, angleToRotate[indexTimeSpeed]);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (indexTimeSpeed < 5)
            {
                if (IsRunning)
                {
                    indexTimeSpeed++;
                    TimeSelectorGraphics.rotation = Quaternion.Euler(0, 0, angleToRotate[indexTimeSpeed]);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (indexTimeSpeed > 0)
            {
                if (IsRunning)
                {
                    indexTimeSpeed--;
                    TimeSelectorGraphics.rotation = Quaternion.Euler(0, 0, angleToRotate[indexTimeSpeed]);
                }
            }
        }

        ProcessTime();
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
