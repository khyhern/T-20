using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CountdownTimerTMP : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Assign a TextMeshProUGUI element in the Inspector
    private float timeRemaining = 300f; // 5 minutes (300 seconds)
    private bool timerRunning = true;

    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= 1;
            UpdateTimerUI();
            yield return new WaitForSeconds(1f);
        }

        timeRemaining = 0;
        timerRunning = false;
        UpdateTimerUI();
        TimerEnded();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TimerEnded()
    {
        Debug.Log("Timer has ended!");
        SceneManager.LoadScene("WinScene");
    }
}