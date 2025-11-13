using UnityEngine;
using System.Collections;

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
    public int MaxAmmo => maxAmmo;

    [SerializeField] private float reloadTime = 2f;

    private int currentAmmo;
    private float nextFireTime = 0f;
    private bool isReloading = false;

    private void Awake()
    {
        Instance = this;
        currentAmmo = maxAmmo;
    }

    public void Fire(Vector2 direction)
    {
        if (isReloading || Time.time < nextFireTime) return;
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
        
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        currentAmmo--;

        // อัปเดต UI Ammo
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);

        // ยิงกระสุน
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        BulletController bc = bullet.GetComponent<BulletController>();
        if (bc != null) bc.SetDamage(bulletDamage);

        Vector2 dir = direction.normalized;
        rb.linearVelocity = dir * bulletSpeed;
        bullet.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
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
