using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EquipWeapon : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponSlot
    {
        public GameObject weaponObject; // The entire weapon GameObject
        public KeyCode keybind;
        public int requiredLevel; // Minimum level required to equip this weapon
        public string weaponName; // Name of the weapon for UI display
    }

    public WeaponSlot[] weapons;
    public Text weaponText;
    public Text equipHintText;
    private GameObject activeWeapon = null;
    private PlayerScript player; // Reference to the player's script

    private HashSet<int> shownWeapons = new HashSet<int>(); // Tracks weapons already hinted


    private void Start()
    {
        equipHintText.text = "";
        equipHintText.color = new Color(equipHintText.color.r, equipHintText.color.g, equipHintText.color.b, 0);

        player = Object.FindFirstObjectByType<PlayerScript>();

        if (player == null)
        {
            Debug.LogError("PlayerScript not found! Make sure the player object has the script attached.");
            return;
        }

        HideWeaponText();
        ActivateFirstWeapon();
        DisplayEquipHints();
    }

    private void Update()
    {
        foreach (WeaponSlot slot in weapons)
        {
            if (Input.GetKeyDown(slot.keybind))
            {
                if (player.currentLevel >= slot.requiredLevel)
                {
                    Equip(slot.weaponObject);
                }
                else
                {
                    Debug.Log("You need to be level " + slot.requiredLevel + " to equip this weapon!");
                }
                break;
            }
        }
    }

    private void Equip(GameObject newWeapon)
    {
        // Ensure all weapons are deactivated
        foreach (WeaponSlot slot in weapons)
        {
            if (slot.weaponObject != null)
            {
                slot.weaponObject.SetActive(false); // Disable all weapons first
                Debug.Log("Deactivating: " + slot.weaponObject.name);
            }
        }

        if (newWeapon != null)
        {
            activeWeapon = newWeapon;
            activeWeapon.SetActive(true);
            Debug.Log("Activating: " + activeWeapon.name);

            // Display weapon identity
            ShootingContinuous contShooting = activeWeapon.GetComponent<ShootingContinuous>();
            ShootingSemi semiShooting = activeWeapon.GetComponent<ShootingSemi>();

            if (contShooting != null)
            {
                ShowWeaponText("SMG");
            }
            else if (semiShooting != null)
            {
                ShowWeaponText("Shotgun");
            }
        }
    }

    private void ActivateFirstWeapon()
    {
        if (weapons.Length > 0 && weapons[0].weaponObject != null)
        {
            Equip(weapons[0].weaponObject);
        }
    }

    private void ShowWeaponText(string weaponIdentity)
    {
        StopAllCoroutines();
        weaponText.text = weaponIdentity;
        weaponText.color = new Color(weaponText.color.r, weaponText.color.g, weaponText.color.b, 1);
        StartCoroutine(FadeWeaponText());
    }

    private IEnumerator FadeWeaponText()
    {
        yield return new WaitForSeconds(2f);
        float fadeDuration = 1f;
        float elapsedTime = 0f;
        Color startColor = weaponText.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            weaponText.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1, 0, elapsedTime / fadeDuration));
            yield return null;
        }
        HideWeaponText();
    }

    private void HideWeaponText()
    {
        weaponText.color = new Color(weaponText.color.r, weaponText.color.g, weaponText.color.b, 0);
    }

    public void DisplayEquipHints()
    {
        string hintMessage = "";
        foreach (WeaponSlot slot in weapons)
        {
            // If player reaches the required level and hasn't been shown before
            if (player.currentLevel == slot.requiredLevel && !shownWeapons.Contains(slot.requiredLevel))
            {
                hintMessage += $"Press {slot.keybind} to equip {slot.weaponName}\n";
                shownWeapons.Add(slot.requiredLevel); // Mark this weapon as shown
            }
        }

        // Display message only if a new weapon hint was added
        if (!string.IsNullOrEmpty(hintMessage))
        {
            equipHintText.text = hintMessage;
            equipHintText.color = new Color(equipHintText.color.r, equipHintText.color.g, equipHintText.color.b, 1);
            StartCoroutine(FadeEquipHint());
        }
    }


    private IEnumerator FadeEquipHint()
    {
        yield return new WaitForSeconds(5f);
        float fadeDuration = 1f;
        float elapsedTime = 0f;
        Color startColor = equipHintText.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            equipHintText.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1, 0, elapsedTime / fadeDuration));
            yield return null;
        }
        equipHintText.text = "";
    }
}
