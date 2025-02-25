using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign the menu UI in the inspector
    public Button resumeButton;
    public Button quitButton;

    private bool isPaused = false;

    void Start()
    {
        // Ensure the menu is hidden at the start
        pauseMenuUI.SetActive(false);

        // Add listeners to buttons
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freezes the game
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resumes the game
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Ensure time is normal before quitting
        Application.Quit();
        Debug.Log("Game Quit"); // This message appears in the editor but does nothing in a built game

    }
}
