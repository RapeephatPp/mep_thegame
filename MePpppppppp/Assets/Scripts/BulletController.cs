using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 25f;
    public GameObject hitEffectPrefab;

    
    
    void Start()
    {
        Destroy(gameObject, lifetime);
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

                // เล่นอนิเมชันแบบไม่ต้องมี Animator Controller
                Animator anim = effect.GetComponent<Animator>();
                anim.Play("Hit_Anim", 0, 0); // ชื่อคลิปจริงของ HitEffect

                Destroy(effect, 0.25f);
            }

            Destroy(gameObject);
        }
    }

}