using UnityEngine;
using TMPro;

public class DemoClock : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;

    private void OnEnable()
    {
        TimeManager.OnMinutesChanged += UpdateTime;
        TimeManager.OnHoursChanged += UpdateTime;
    }

    private void OnDisable()
    {
        TimeManager.OnMinutesChanged -= UpdateTime;
        TimeManager.OnHoursChanged -= UpdateTime;
    }

    private void UpdateTime()
    {
        _timeText.text = $"{TimeManager.Hours:00}:{TimeManager.Minutes:00}";
    }
}
