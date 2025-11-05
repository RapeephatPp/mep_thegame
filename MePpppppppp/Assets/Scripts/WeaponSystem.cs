using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletDamage = 25f;

    private float nextFireTime;

    public void Fire(Vector2 direction)
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        BulletController bc = bullet.GetComponent<BulletController>();

        if (bc != null)
            bc.SetDamage(bulletDamage);

        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;
    }

    public void UpgradeDamage(float amount)
    {
        bulletDamage += amount;
    }

    public void UpgradeFireRate(float multiplier)
    {
        fireRate *= multiplier; // เช่น 0.8 = เร็วขึ้น 20%
    }
}