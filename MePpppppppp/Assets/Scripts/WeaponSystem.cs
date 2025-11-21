using UnityEngine;
using System.Collections;

public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem Instance;

    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    public Transform FirePoint => firePoint; 
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private int bulletDamage = 10;

    [Header("Fire / Reload")]
    [SerializeField] private float fireCooldown = 0.3f;
    [SerializeField] private int maxAmmo = 10;
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
            StartCoroutine(Reload());
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        currentAmmo--;

        // Screen Shake (เบาลง)
        StartCoroutine(CameraShake.Instance.Shake(0.04f, 0.04f));
        

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

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * bulletSpeed;
    }


    private IEnumerator Reload()
    {
        if (isReloading) yield break;
        isReloading = true;
        
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

    public void AddDamage(int amount)
    {
        bulletDamage += amount;
    }

    public void AddFireRate(float amount)
    {
        fireCooldown = Mathf.Max(0.05f, fireCooldown - amount);
    }
}
