using UnityEngine;

public class UpgradeSMGReload : MonoBehaviour
{
    public float decreaseReload = 0.5f;
    public ShootingContinuous smg;
    public GameObject upgradeMenu;

    public void DecreaseReloadTime()
    {
        if (smg != null)
        {
            smg.reloadTime = Mathf.Max(0.5f, smg.reloadTime - decreaseReload); // Prevent negative reload time
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
