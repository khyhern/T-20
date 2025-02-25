using UnityEngine;

public class UpgradeSGDamage : MonoBehaviour
{
    public int addDamage = 10;
    public ShootingSemi shotgun;
    public GameObject upgradeMenu;

    public void IncreaseDamage()
    {
        if (shotgun != null)
        {
            shotgun.bulletDamage += addDamage;
        }

        CloseUpgradeMenu();
    }

    private void CloseUpgradeMenu()
    {
        if (upgradeMenu != null)
        {
            upgradeMenu.SetActive(false);
        }
        Time.timeScale = 1f;    // Resume game
    }
}
