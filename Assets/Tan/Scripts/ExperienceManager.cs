using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    public delegate void ExperienceChangeHandler(int amount);
    public event ExperienceChangeHandler OnExperienceChange;

    // Singleton initialization
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameObject
            return; // Exit early to prevent overwriting the original Instance
        }

        Instance = this; // Set this as the active Instance
    }

    // Method to safely add experience
    public void AddExperience(int amount)
    {
        OnExperienceChange?.Invoke(amount); // Trigger event if there are subscribers
    }
}

