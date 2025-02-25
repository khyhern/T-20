using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;

    public Rigidbody2D rb;
    public Animator animator;
    public Collider2D playerCollider;

    public int maxHealth = 100;
    public int playerHealthPoints = 100;
    public int currentExperience = 0;
    public int maxExperience = 300;
    public int currentLevel = 1;

    private Vector2 movement;
    private Vector2 knockback;

    private bool isDashing = false;
    private float dashCooldownRemaining = 0f; // Tracks cooldown time

    private DashCooldownUI dashCooldownUI; // Reference to the cooldown UI script

    public Text healthText;
    public Text levelText;

    public GameObject upgradeUIPanel; // Reference to Upgrade UI

    public bool IsDashOnCooldown => dashCooldownRemaining > 0;
    public float DashCooldownRemaining => dashCooldownRemaining;

    public EquipWeapon equipWeapon; // Reference to EquipWeapon script

    void Start()
    {
        equipWeapon = FindAnyObjectByType<EquipWeapon>(); // Automatically find EquipWeapon in scene
        dashCooldownUI = FindAnyObjectByType<DashCooldownUI>();

        if (dashCooldownUI == null)
        {
            Debug.LogError("DashCooldownUI not found! Make sure it's in the scene.");
        }

        if (upgradeUIPanel != null)
        {
            upgradeUIPanel.SetActive(false); // Ensure UI is hidden at start
        }
        else
        {
            Debug.LogError("Upgrade UI Panel is not assigned in the Inspector!");
        }
    }

    void Update()
    {
        if (upgradeUIPanel != null && upgradeUIPanel.activeSelf)
            return; // Stop player movement if UI is active

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (movement.x > 0)
            transform.localScale = new Vector3(1, 1, 1);

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !IsDashOnCooldown)
        {
            StartCoroutine(DashRoutine());
        }

        if (dashCooldownRemaining > 0)
        {
            dashCooldownRemaining -= Time.deltaTime;
        }

        if (healthText != null)
            healthText.text = $"Health: {playerHealthPoints}";

        if (levelText != null)
            levelText.text = $"Level {currentLevel}";
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            Vector2 move = movement.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move + knockback * Time.fixedDeltaTime);
        }

        knockback = Vector2.Lerp(knockback, Vector2.zero, 5f * Time.fixedDeltaTime);
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        dashCooldownRemaining = dashCooldown;
        playerCollider.enabled = false;

        Vector2 dashDirection = movement.normalized;
        rb.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        playerCollider.enabled = true;
        isDashing = false;
        rb.linearVelocity = movement.normalized * moveSpeed;
    }

    private void OnEnable()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChange += HandleExperienceChange;
        }
        else
        {
            Debug.LogError("ExperienceManager.Instance is null. Check Edit > Project Settings > Script Execution Order.");
        }
    }

    private void OnDisable()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChange -= HandleExperienceChange;
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        knockback = force;
    }

    public void TakeDamage(int damageToPlayer)
    {
        playerHealthPoints -= damageToPlayer;

        if (playerHealthPoints < 0)
        {
            playerHealthPoints = 0;
        }

        Debug.Log($"Player Health: {playerHealthPoints}");

        if (playerHealthPoints <= 0)
            PlayerDie();
    }

    private void PlayerDie()
    {
        Debug.Log("Game Over!");
        Time.timeScale = 0f;
    }

    private void HandleExperienceChange(int newExperience)
    {
        currentExperience += newExperience;
        if (currentExperience >= maxExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerHealthPoints = maxHealth;
        currentLevel++;
        currentExperience = 0;

        if (equipWeapon != null)
        {
            equipWeapon.DisplayEquipHints();
        }

        if (currentLevel > 2)
        {
            ShowUpgradeUI();
        }
    }

    private void ShowUpgradeUI()
    {
        if (upgradeUIPanel != null)
        {
            upgradeUIPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }
    }

    public void CloseUpgradeUI()
    {
        if (upgradeUIPanel != null)
        {
            upgradeUIPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game
        }
    }
}


/*{
    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Animator animator;


    public int playerHealthPoints;


    Vector2 movement;           //To store horizontal and vertical movement / x & y

    private Vector2 knockback;  //To store knockback force


    // Update is called once per frame 
    void Update()
    {
        //For INPUTS 
        movement.x = Input.GetAxisRaw("Horizontal"); // < = -1 | > = 1 | No button press return 0
        movement.y = Input.GetAxisRaw("Vertical");


        // Adjust player's scale based on moving direction (To flip the animation of walkingRight)
        if (movement.x < 0)                                 // Moving left
            transform.localScale = new Vector3(-1, 1, 1);       // Flip the sprite on the x-axis
        else if (movement.x > 0)                            // Moving right
            transform.localScale = new Vector3(1, 1, 1);        // Reset the sprite to face right


        //~~Allows to adjust paramters
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude); //Length of movement Vector (sqr for optimization)
    }

    //Best for physics use 
    private void FixedUpdate()
    {
        //For MOVEMENTS
        //movement = movement.normalized;     //Maintain diagonal movement speed 
        //rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        Vector2 move = movement.normalized * moveSpeed * Time.fixedDeltaTime;

        //Apply knockback and decay it over time
        rb.MovePosition(rb.position + move + knockback * Time.fixedDeltaTime);
        knockback = Vector2.Lerp(knockback, Vector2.zero, 5f * Time.fixedDeltaTime);    //Smoothly reduce knockback
    }

    public void ApplyKnockback(Vector2 force)
    {
        knockback = force;      //Set the knockback force
    }

    public void TakeDamage(int damageToPlayer)
    {
        playerHealthPoints -= damageToPlayer;
        Debug.Log($"Player Health: {playerHealthPoints}");

        if (playerHealthPoints <= 0)
            PlayerDie();
    }

    private void PlayerDie()
    {
        //Add death animation 

        Debug.Log("Game Over!");
        Time.timeScale = 0f;            //Freeze all in-game activity || Can use to pause game
        //To Game Over Scene 
    }
}*/