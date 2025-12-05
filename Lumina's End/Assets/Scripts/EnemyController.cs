using UnityEngine;

public class EnemyController : Character, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 1.5f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;
    //Stun Effect
    //private bool isStunned = false;
    
    
    public Transform targetBase;
    private Rigidbody2D rb;
    private Transform playerTarget;
    private Transform baseTarget;
    private float nextAttackTime;
    private bool isKnockedBack = false;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;

        var baseObj = GameObject.FindWithTag("Base");
        if (baseObj != null) baseTarget = baseObj.transform;
    }

    private void FixedUpdate()
    {   
        if (isKnockedBack) return;
        
        Transform currentTarget = GetClosestTarget();
        if (currentTarget == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.position);
        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack(currentTarget);
        }
        else
        {
            MoveTowards(currentTarget);
        }
    }

    private Transform GetClosestTarget()
    {
        if (playerTarget == null || baseTarget == null) return null;
        float distPlayer = Vector2.Distance(transform.position, playerTarget.position);
        float distBase = Vector2.Distance(transform.position, baseTarget.position);
        return (distPlayer < distBase) ? playerTarget : baseTarget;
    }

    private void MoveTowards(Transform target)
    {
        Vector2 dir = (target.position - transform.position).normalized;
        rb.linearVelocity = dir * speed;

        // หันตามทิศ
        if (dir.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
    }

    private void TryAttack(Transform target)
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;
        
        if (AudioManager.Instance != null && attackClip != null)
            AudioManager.Instance.PlaySFX(attackClip, 0.95f, 1.05f);

        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(attackDamage);
        }
    }
    
    public override void TakeDamage(float damage)
    {
        if (AudioManager.Instance != null && hurtClip != null)
            AudioManager.Instance.PlaySFX(hurtClip, 0.95f, 1.05f);
        
        base.TakeDamage(damage);
    }
    
    //Apply Knockback
    public void ApplyKnockback(Vector2 force, float duration)
    {
        if (rb == null) return;

        // หยุดเดินชั่วคราว
        StopCoroutine("KnockbackRoutine"); // หยุด Coroutine เก่าถ้ามี
        StartCoroutine(KnockbackRoutine(force, duration));
    }
    System.Collections.IEnumerator KnockbackRoutine(Vector2 force, float duration)
    {
        isKnockedBack = true;
        
        // Reset ความเร็วเดิมก่อน แล้วใส่แรงผลักทันที
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        isKnockedBack = false;
        // หลังจากนี้ FixedUpdate จะกลับมาทำงาน ให้มอนเดินต่อ
    }
    
    protected override void Die()
    {   
        if (AudioManager.Instance != null && deathClip != null)
            AudioManager.Instance.PlaySFX(deathClip, 0.95f, 1.05f);

        if (WeaponSystem.Instance != null && WeaponSystem.Instance.HasVampireShot)
        {
            PlayerHealth.Instance.Heal(WeaponSystem.Instance.LifeStealAmount);
        }
        
        Debug.Log($"{name} died!");
        
        GameManager.Instance.RegisterEnemyDeath();
        Destroy(gameObject);
    }
}
