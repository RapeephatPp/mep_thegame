using System;
using UnityEngine;
using System.Collections;
using System.Threading;

public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem Instance;

    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private int bulletDamage = 10;

    [Header("Fire / Reload")]
    [SerializeField] private float fireCooldown = 0.3f;
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private float reloadTime = 2f;
    
    [Header("Special Effects")]
    //VampireShot
    [SerializeField] private bool vampireShot = false;
    [SerializeField] private int lifeStealAmount = 0;
    //AdenalineRush
    [SerializeField] private bool adrenalineRush = false;
    [SerializeField] private float adrenalineMultiplier = 1f;
    [SerializeField] private float adrenalineDuration = 1f;
    //ShockRound
    /*[SerializeField] private bool shockRound = false;
    [SerializeField] private float shockChance = 0f;
    [SerializeField] private float shockDuration = 0f;*/
    
    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip1;
    [SerializeField] private AudioClip reloadClip2;
    [SerializeField] private AudioClip emptyClip;
    
    public Transform FirePoint => firePoint;
    public float BulletSpeed => bulletSpeed;
    public int BulletDamage => bulletDamage;
    public float FireCooldown => fireCooldown;
    public int MaxAmmo => maxAmmo;
    public float ReloadTime => reloadTime; 
    //Special Effect
    public bool HasVampireShot => vampireShot;
    public int LifeStealAmount => lifeStealAmount;
    private bool adrenalineActive = false;
    public bool HasAdrenalineRush => adrenalineRush;
    public float AdrenalineMultiplier => adrenalineMultiplier;
    public float AdrenalineDuration  => adrenalineDuration;
    

    private int currentAmmo;
    private float nextFireTime = 0f;
    private bool isReloading = false;
    public GameObject muzzleFlashPrefab;
    

    private void Awake()
    {
        Instance = this;
        currentAmmo = maxAmmo;
    }

    public void Fire(Vector2 direction)
    {
        if (Time.time < nextFireTime || isReloading)
            return;

        if (currentAmmo <= 0)
        {   
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(emptyClip, 0.9f, 1.1f);
            
            StartCoroutine(Reload());
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        currentAmmo--;
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
        
        // Screen Shake
        StartCoroutine(CameraShake.Instance.Shake(0.04f, 0.04f));
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(shootClip, 0.95f, 1.05f);
        

        // --------- Muzzle Flash ---------
        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(
                muzzleFlashPrefab,
                firePoint.position,
                Quaternion.identity,      // ไม่เอา firePoint.rotation แล้ว
                firePoint                 // ให้เป็นลูกของ firePoint เหมือนเดิม
            );

            // ให้ flare หันไปตามทิศยิง
            flash.transform.right = direction.normalized;
        }
        
        // Bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.transform.right = direction.normalized;   // หันไปตามทิศยิง
        var bc = bullet.GetComponent<BulletController>();
        if (bc != null)
        {
            bc.Setup(direction, bulletSpeed, bulletDamage);
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * bulletSpeed;
    }


    private IEnumerator Reload()
    {
        if (isReloading) yield break;
        isReloading = true;
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(reloadClip1);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(reloadClip2);
        
        float timer = 0f;
        while (timer < reloadTime)
        {   
            timer += Time.deltaTime;
            
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateReload(timer / reloadTime);
            
            yield return null;
        }
        
        currentAmmo = maxAmmo;
        isReloading = false;
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
        UIManager.Instance.UpdateReload(0f);
    }

    public void TryManualReload()
    {
        if (isReloading) return;
        
        if (currentAmmo >= maxAmmo) return;
        
        StartCoroutine(Reload());
    }

    public void AddDamage(int amount)
    {
        bulletDamage += amount;
    }

    public void AddFireRate(float amount)
    {
        fireCooldown = Mathf.Max(0.05f, fireCooldown - amount);
    }

    public void AddBulletSpeed(float amount)
    {
        bulletSpeed = Mathf.Min(bulletSpeed + amount, 20f);
    }

    public void AddAmmoCapacity(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[Weapon] AddAmmoCapacity called with amount={amount}, skipped.");
            return;
        }

        int oldMax = maxAmmo;
        int oldCur = currentAmmo;

        maxAmmo += amount;
        currentAmmo = maxAmmo;   // รีฟิลเต็มแม็กหลังอัปเกรด

        Debug.Log($"[Weapon] AmmoCapacity +{amount} | {oldCur}/{oldMax} -> {currentAmmo}/{maxAmmo}");

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    }

    public void AddReloadSpeed(float amount)
    {
        //Decrease Reload time but not lower than 0.3s
        reloadTime = Math.Max(0.3f, reloadTime - amount);
    }

    public void EnableVampireShot(int healAmount)
    {
        vampireShot = true;
        lifeStealAmount = healAmount;
    }
    
    public void EnableAdrenalineRush(float multiplier, float duration)
    {
        adrenalineRush = true;
        adrenalineMultiplier = multiplier;
        adrenalineDuration = duration;
    }
    
    public void TriggerAdrenaline()
    {
        if (!adrenalineRush || adrenalineActive) return;
        StartCoroutine(AdrenalineRoutine());
    }
    
    IEnumerator AdrenalineRoutine()
    {
        adrenalineActive = true;
        float originalCooldown = fireCooldown;
        fireCooldown /= adrenalineMultiplier; // ยิงเร็วขึ้น
        yield return new WaitForSeconds(adrenalineDuration);
        fireCooldown = originalCooldown;
        adrenalineActive = false;
    }

    /*public void EnableShockRound(float chance, float duration)
    {
        shockRound = true;
        shockChance = chance;
        shockDuration = duration;
    }*/
    
    public void SetWeaponFromRun(int damage, float speed, float cooldown, int maxAmmo, float reloadTime)
    {
        bulletDamage = damage;
        bulletSpeed = speed;
        fireCooldown = cooldown;
        this.maxAmmo = maxAmmo;
        this.reloadTime = reloadTime;

        // currentAmmo มีอยู่ในสคริปต์อยู่แล้ว
        currentAmmo = Mathf.Clamp(currentAmmo, 0, this.maxAmmo);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo, this.maxAmmo);
    }

}
