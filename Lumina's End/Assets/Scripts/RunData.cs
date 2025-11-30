using UnityEngine;

public static class RunData
{
    public static bool HasData;

    public static int currentWave;

    public static int playerCurrentHp;
    public static int playerMaxHp;

    public static int baseCurrentHp;
    public static int baseMaxHp;

    public static int bulletDamage;
    public static float bulletSpeed;
    public static float fireCooldown;
    public static int maxAmmo;
    public static float reloadTime;
    
    // ---------- Special Effects ----------
    public static bool vampireShot;
    public static int lifeStealAmount;

    public static bool adrenalineRush;
    public static float adrenalineMultiplier;
    public static float adrenalineDuration;
    
    public static void Reset()
    {
        HasData = false;
        currentWave = 0;
        playerCurrentHp = playerMaxHp = 0;
        baseCurrentHp = baseMaxHp = 0;
        bulletDamage = 0;
        bulletSpeed = 0f;
        fireCooldown = 0f;
        maxAmmo = 0;
        reloadTime = 0f;
    }
}
