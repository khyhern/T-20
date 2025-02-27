using UnityEngine;
using UnityEngine.UI;
using System;

public class CountdownTimer : MonoBehaviour
{
    public Text timerText;
    public float startValue = 300f; // Set your starting time here
    public float endValue = 100f; // Set your target minimum value (acts as '0')
    public float timeRemaining;
    public bool timerIsRunning = false;

    public static event Action OnDifficultyIncrease; // Event for increasing difficulty

    private float difficultyIncreaseInterval = 40f; // Every 40 seconds
    private float nextIncreaseTime;

    void Start()
    {
        timeRemaining = startValue;

        if (timeRemaining > endValue)
        {
            timerIsRunning = true;
            nextIncreaseTime = timeRemaining - difficultyIncreaseInterval;
        }
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > endValue)
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
                timeRemaining = endValue;
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
        FindFirstObjectByType<PlayerProgress>()?.TimeUp();
    }
}

/*using UnityEngine;
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
        FindFirstObjectByType<PlayerProgress>()?.TimeUp();
    }
}*/