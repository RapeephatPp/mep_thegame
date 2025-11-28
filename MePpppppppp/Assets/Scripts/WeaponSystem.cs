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
    

    [SerializeField] private float reloadTime = 2f;

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
        bulletSpeed += amount;
    }

    public void AddAmmoCapacity(int amount)
    {
        maxAmmo += amount;
        currentAmmo = amount;
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    }
    
    public void SetWeaponFromRun(int damage, float speed, float cooldown, int maxAmmo)
    {
        bulletDamage = damage;
        bulletSpeed = speed;
        fireCooldown = cooldown;
        this.maxAmmo = maxAmmo;

        // currentAmmo มีอยู่ในสคริปต์อยู่แล้ว
        currentAmmo = Mathf.Clamp(currentAmmo, 0, this.maxAmmo);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo, this.maxAmmo);
    }

}
