using UnityEngine;
using UnityEngine.UI;

// Reference : https://www.youtube.com/watch?v=J1ng1zA3-Pk&ab_channel=GameDevGuide 

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    // public int minimum = 0;
    public int maximum;
    public int current;
    public Image mask;
    public Image fill;
    public Color color;

    private PlayerScript player; // Reference to the PlayerScript

    void Start()
    {
        // Find the PlayerScript on the Player GameObject
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<PlayerScript>();
        }
        else
        {
            Debug.LogWarning("Player GameObject not found! Ensure it has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Sync ProgressBar values with PlayerScript's experience values
            current = player.currentExperience;
            maximum = player.maxExperience;
        }

        GetCurrentFill();
    }


    void GetCurrentFill()
    {
        float currentOffset = current; // current - minimum
        float maximumOffset = maximum; // current - minimum

        //if (maximumOffset <= 0) // Prevent division by zero
        //{
        //    mask.fillAmount = 0; // Default to empty
        //    return;
        //}


        float fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = fillAmount; 

        fill.color = color;
    }
}
