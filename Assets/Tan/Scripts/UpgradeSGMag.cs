using UnityEngine;

public class UpgradeSGMag : MonoBehaviour
{
    public int magSize = 2;
    public ShootingSemi shotgun;
    public GameObject upgradeMenu;

    public void IncreaseMagazineSize()
    {
        if (shotgun != null)
        {
            shotgun.magazineSize += magSize;
            shotgun.currentAmmo = shotgun.magazineSize; // Refill magazine after upgrade
            shotgun.UpdateAmmoDisplay();
        }

        CloseUpgradeMenu();
    }

    private void CloseUpgradeMenu()
    {
        if (upgradeMenu != null)
        {
            upgradeMenu.SetActive(false);
        }
        Time.timeScale = 1f; // Resume game
    }
}
