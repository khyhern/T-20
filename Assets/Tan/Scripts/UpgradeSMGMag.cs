using UnityEngine;

public class UpgradeSMGMag : MonoBehaviour
{
    public int magSize = 10;
    public ShootingContinuous smg;
    public GameObject upgradeMenu;

    public void IncreaseMagazineSize()
    {
        if (smg != null)
        {
            smg.magazineSize += magSize;
            smg.currentAmmo = smg.magazineSize; // Refill magazine after upgrade
            smg.UpdateAmmoDisplay();
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
