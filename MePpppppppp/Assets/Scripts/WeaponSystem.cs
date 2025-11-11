using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem Instance;

    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;

    public int bulletDamage = 10;
    public float fireCooldown = 0.3f;

    private float nextFireTime = 0f;

    private void Awake()
    {
        Instance = this;
    }

    public void Fire(Vector2 direction)
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireCooldown;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        BulletController bc = bullet.GetComponent<BulletController>();
        if (bc != null)
            bc.SetDamage(bulletDamage);

        if (rb != null)
        {
            Vector2 dir = direction.normalized; // เเน่ใจว่าเป็นหน่วยเวกเตอร์
            rb.linearVelocity = dir * bulletSpeed;

            // (option) ให้กระสุนหันไปตามทิศทาง เพื่อให้ sprite หมุนถูก
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
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