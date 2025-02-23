using TMPro;
using UnityEngine;

public class AmmoUIController : MonoBehaviour
{
    public TextMeshProUGUI ammoText; // Reference to the TextMeshPro component
    public GunSystem gunSystem; // Reference to the gun system

    void Update()
    {
        if (gunSystem != null)
        {
            // Format ammo count as three-digit numbers
            ammoText.text = $"{gunSystem.currentAmmo.ToString("D3")}/{gunSystem.maxAmmo.ToString("D3")}";
        }
    }
}
