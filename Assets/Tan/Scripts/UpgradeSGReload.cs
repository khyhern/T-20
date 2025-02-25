using UnityEngine;

public class UpgradeSGReload : MonoBehaviour
{
    public float decreaseReload = 0.5f;
    public ShootingSemi shotgun;
    public GameObject upgradeMenu;

    public void DecreaseReloadTime()
    {
        if (shotgun != null)
        {
            shotgun.reloadTime = Mathf.Max(0.5f, shotgun.reloadTime - decreaseReload); // Prevent negative reload time
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
