using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // (Axel) Probablement quelque chose du genre 1 min IG = 0.5s IRL
    [SerializeField] private float _minToRealTime = 0.5f;

    private float _timer;

    public static Action OnMinutesChanged;
    public static Action OnHoursChanged;

    public static bool IsRunning { get; private set; }
    public static int Minutes { get; private set; }
    public static int Hours { get; private set; }

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        ProcessTime();
    }

    // (Manu) Je le passe en fonction si jamais on veut qu'un composant externe ait la main sur le lancement du temps
    public void StartTimer()
    {
        _timer = Math.Abs(_minToRealTime);
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
        // (Manu) J'étais pas sûr du comportement qu'on voulait pour le _minToRealTime
        // Dans le doute j'ai mis une garde quand il est à 0 pour éviter de spammer les event à chaque frame
        if (!IsRunning || _minToRealTime == 0)
        {
            return;
        }

        _timer -= Time.deltaTime;

        if(_timer <= 0)
        {
            Minutes += Math.Sign(_minToRealTime); // (Manu) Juste pour simplifier le if-else du code d'origine
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

            _timer = Math.Abs(_minToRealTime);
        }
    }
}
