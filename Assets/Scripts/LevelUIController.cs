using TMPro;
using UnityEngine;

public class LevelUIController : MonoBehaviour
{
    public TextMeshProUGUI levelText; // Reference to the TMP UI text
    public PlayerStats playerStats; // Reference to the PlayerStats script

    void Start()
    {
        if (playerStats != null)
        {
            UpdateLevelUI();
        }
    }

    public void UpdateLevelUI()
    {
        if (levelText != null && playerStats != null)
        {
            levelText.text = $"Level {playerStats.level}";
        }
    }
}
