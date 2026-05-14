using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Настройки времени")]
    public float realTimeDuration = 150f; 
    public float lessonMinutes = 90f;    

    private float timeRemaining;
    private bool isGameActive = true;

    [Header("UI элементы")]
    public TextMeshProUGUI timerText; 

    void Start()
    {
        timeRemaining = realTimeDuration;
    }

    void Update()
    {
        if (!isGameActive) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            timeRemaining = 0;
            GameOver();
        }
    }

    void UpdateTimerDisplay()
    {
        float progress = 1 - (timeRemaining / realTimeDuration);

        float totalLessonMinutes = 95f;
        float minutesPassed = progress * totalLessonMinutes;


        int roundedMinutesPassed = Mathf.FloorToInt(minutesPassed / 5) * 5;

        int startMinutesTotal = (8 * 60) + 45;

        int currentTotalMinutes = startMinutesTotal + roundedMinutesPassed;

        int hours = currentTotalMinutes / 60;
        int minutes = currentTotalMinutes % 60;

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }
    }

    void GameOver()
    {
        isGameActive = false;
        Debug.Log("Время вышло! Урок окончен, ты не успел собрать шпаргалки.");

    }

    public void StopTimer()
    {
        isGameActive = false;
    }
}