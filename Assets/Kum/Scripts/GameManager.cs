using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    // Define the different states of the game
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    // Store the current state of the game
    public GameState currentState;

    // Store the previous state of the game before it was paused
    public GameState previousState;

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    [Header("Results Screen Displays")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
	public TMP_Text timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);
	
	[Header("Timer Countdown")]
	public float levelTime; // Countdown timer
	public TMP_Text timeDisplay;
	public TMP_Text timerText;


    // Flag to check if the game is over
    public bool isGameOver = false;

    // Flag to check if the player is choosing their upgrades
    public bool choosingUpgrade = false;

    // Reference to the player's game object
    public GameObject playerObject;

    void Awake()
{
    if (instance == null)
    {
        instance = this;
    }
    else
    {
        Debug.LogWarning("EXTRA " + this + " DELETED");
        Destroy(gameObject);
    }

    DisableScreens();

    // ✅ Ensure the level starts at 15 minutes
    if (levelTime <= 0)
    {
        levelTime = 900f;
    }
}


 void Update()
{
    switch (currentState)
    {
        case GameState.Gameplay:
            CheckForPauseAndResume();

            // ✅ Ensure timer decreases only in Gameplay
            if (levelTime > 0)
            {
                levelTime -= Time.deltaTime;
                UpdateTimerUI();
            }
            else if (levelTime <= 0)
            {
                levelTime = 0; // Prevents negative values
                EndLevel();
            }
            break;

        case GameState.Paused:
            CheckForPauseAndResume();
            break;

        case GameState.GameOver:
            if (!isGameOver)
            {
                isGameOver = true;
                Time.timeScale = 600f; // Stop the game
                Debug.Log("Game is over");
                DisplayResults();
            }
            break;

        case GameState.LevelUp:
            if (!choosingUpgrade)
            {
                choosingUpgrade = true;
                Time.timeScale = 0f; // Pause the game
                Debug.Log("Upgrades shown");
                levelUpScreen.SetActive(true);
            }
            break;

        default:
            Debug.LogWarning("STATE DOES NOT EXIST");
            break;
    }
}


    public IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        // Start generating the floating text.
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        // Makes sure this is destroyed after the duration finishes.
        Destroy(textObj, duration);

        // Parent the generated text object to the canvas.
        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        // Pan the text upwards and fade it away over time.
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnownPosition = target.position;
        while (t < duration)
        {
            // If the RectTransform is missing for whatever reason, end this loop.
            if (!rect) break;

            // Fade the text to the right alpha value.
            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            // Update the enemy's position if it is still around.
            if (target) lastKnownPosition = target.position;

            // Pan the text upwards.
            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnownPosition + new Vector3(0, yOffset));

            // Wait for a frame and update the time.
            yield return w;
            t += Time.deltaTime;
        }
    }
	


  public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        // If the canvas is not set, end the function so we don't
        // generate any floating text.
        if (!instance.damageTextCanvas) return;

        // Find a relevant camera that we can use to convert the world
        // position to a screen position.
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(
            text, target, duration, speed
        ));
    }

    // Define the method to change the state of the game
    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Stop the game
            pauseScreen.SetActive(true); // Enable the pause screen
            Debug.Log("Game is paused");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // Resume the game
            pauseScreen.SetActive(false); //Disable the pause screen
            Debug.Log("Game is resumed");
        }
    }

    // Define the method to check for pause and resume input
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
         UpdateTimerUI(); // Ensure timer is updated before showing results
			timeDisplay.text = timerText.text;
			ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsAndPassiveItemsUI(List<Player_Inventory.Slot> chosenWeaponsData, List<Player_Inventory.Slot> chosenPassiveItemsData)
    {
        // Check that both lists have the same length
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.LogError("Chosen weapons and passive items data lists have different lengths");
            return;
        }

        // Assign chosen weapons data to chosenWeaponsUI
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            // Check that the sprite of the corresponding element in chosenWeaponsData is not null
            if (chosenWeaponsData[i].image.sprite)
            {
                // Enable the corresponding element in chosenWeaponsUI and set its sprite to the corresponding sprite in chosenWeaponsData
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].image.sprite;
            }
            else
            {
                // If the sprite is null, disable the corresponding element in chosenWeaponsUI
                chosenWeaponsUI[i].enabled = false;
            }
        }

        // Assign chosen passive items data to chosenPassiveItemsUI
        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            // Check that the sprite of the corresponding element in chosenPassiveItemsData is not null
            if (chosenPassiveItemsData[i].image.sprite)
            {
                // Enable the corresponding element in chosenPassiveItemsUI and set its sprite to the corresponding sprite in chosenPassiveItemsData
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].image.sprite;
            }
            else
            {
                // If the sprite is null, disable the corresponding element in chosenPassiveItemsUI
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }

    private void UpdateTimerUI()
{
		if (timerText == null)
	{
    Debug.LogError("Timer Text is NOT assigned in GameManager! Assign it in the Inspector.");
    return;
    }

    // Ensure countdown never goes negative
    float clampedTime = Mathf.Max(levelTime, 0);

    // Convert remaining time to minutes and seconds
    int minutes = Mathf.FloorToInt(clampedTime / 60);
    int seconds = Mathf.FloorToInt(clampedTime % 60);

    // Format timer and update UI
    string formattedTime = $"{minutes:D2}:{seconds:D2}";
    timerText.text = formattedTime;// Ensures both texts display the countdown
 
}


    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f;    //Resume the game
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }
	
	 private void EndLevel()
    {
        // Log the end of the level
        Debug.Log("Level ended!");

        // Implement level end logic (e.g., show results screen, transition to next level)
    }
}