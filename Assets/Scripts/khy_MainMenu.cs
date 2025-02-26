using UnityEngine;
using UnityEngine.SceneManagement;

public class khy_MainMenu : MonoBehaviour
{
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

}
