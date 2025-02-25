using UnityEngine;

public class UpgradeSMGDamage : MonoBehaviour
{
    public int addDamage = 5;
    public ShootingContinuous smg;
    public GameObject upgradeMenu;

    public void IncreaseDamage()
    {
        if (smg != null)
        {
            smg.bulletDamage += addDamage;
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
