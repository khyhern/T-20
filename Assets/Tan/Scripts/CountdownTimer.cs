using UnityEngine;
using UnityEngine.UI;
using System;

public class CountdownTimer : MonoBehaviour
{
    public Text timerText;
    public float timeRemaining;
    public bool timerIsRunning = false;

    public static event Action OnDifficultyIncrease; // Event for increasing difficulty

    private float difficultyIncreaseInterval = 40f; // Every 40 seconds
    private float nextIncreaseTime;

    void Start()
    {
        if (timeRemaining > 0)
        {
            timerIsRunning = true;
            nextIncreaseTime = timeRemaining - difficultyIncreaseInterval;
        }
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();

                if (timeRemaining <= nextIncreaseTime)
                {
                    OnDifficultyIncrease?.Invoke(); // Trigger event
                    nextIncreaseTime -= difficultyIncreaseInterval; // Set next interval
                    Debug.Log("Enemy health will increase!");
                }
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                UpdateTimerDisplay();
                TimerFinished();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TimerFinished()
    {
        Debug.Log("Timer has finished!");
        PauseGame();
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        Debug.Log("Game Paused!");
    }
}
