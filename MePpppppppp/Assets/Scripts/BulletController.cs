using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 25f;
    public GameObject hitEffectPrefab;

    public static float LifetimeMultiplier { get; private set; } = 1f;

    
    
    private Vector2 moveDir;
    private float speed;
    public void Setup(Vector2 direction, float speed, int damage)
    {
        moveDir = direction.normalized;   // เก็บทิศทางเป็น unit vector
        this.speed = speed;
        this.damage = damage;
        
        
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
    void Start()
    {
        Destroy(gameObject, lifetime * LifetimeMultiplier);
    }
    
    private void Update()
    {
        // เคลื่อนที่ตามทิศ moveDir ตรง ๆ
        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }
    
    public static void AddLifetime(float amount)
    {
        LifetimeMultiplier += amount;
    }

    public void SetDamage(float newDamage) 
    {
        damage = newDamage;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            var dmg = col.GetComponent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(damage);

            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

                // -----------------------------
                // หันเอฟเฟกต์ซ้าย/ขวาแบบง่าย ๆ
                // สมมุติ sprite ปกติวาดให้เอฟเฟกต์พุ่งไปทางขวา
                // -----------------------------
                float angle = 0f;

                // กระสุนวิ่งไปทางซ้าย => ให้หมุน 180 องศา รอบแกน Z
                if (moveDir.x < 0f)
                {
                    angle = 180f;
                }

                effect.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                // เล่นอนิเมชัน ถ้ามี
                Animator anim = effect.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Play("Hit_Anim", 0, 0);
                }

                Destroy(effect, 0.25f);
            }

            Destroy(gameObject);
        }
    }

}