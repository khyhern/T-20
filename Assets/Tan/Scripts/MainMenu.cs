using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string sceneToLoad = "MainMenu"; // Assign this in the Inspector

    public void LoadScene()
    {
        Time.timeScale = 1f; // Resumes the game
        SceneManager.LoadScene(sceneToLoad);
    }
}
