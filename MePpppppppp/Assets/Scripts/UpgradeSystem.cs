using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{   
    private UpgradeSystem upgradeSystem;

    
    public void ApplyUpgrade(UpgradeType type)
    {
        // ✅ ดึงข้อมูลที่จำเป็นจาก GameManager / PlayerController โดยตรง
        var player = PlayerController.Instance;
        if (player == null)
        {
            Debug.LogError("❌ PlayerController.Instance is null!");
            return;
        }

        var weapon = player.GetWeaponSystem();
        var baseCtrl = GameManager.Instance.BaseCtrl;

        switch (type)
        {
            case UpgradeType.IncreaseDamage:
                if (weapon != null)
                {
                    weapon.UpgradeDamage(10);
                    Debug.Log("✅ Increased damage");
                }
                break;

            case UpgradeType.IncreaseFireRate:
                if (weapon != null)
                {
                    weapon.UpgradeFireRate(0.8f);
                    Debug.Log("✅ Increased fire rate");
                }
                break;

            case UpgradeType.RepairBase:
                if (baseCtrl != null)
                {
                    baseCtrl.Heal(200);
                    Debug.Log("✅ Base repaired");
                }
                break;
        }
    }
}

public enum UpgradeType
{
    IncreaseDamage,
    IncreaseFireRate,
    RepairBase
}