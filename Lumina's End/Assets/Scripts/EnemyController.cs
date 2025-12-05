using UnityEngine;

public class EnemyController : Character, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 1.5f;
    
    [Header("Data")]
    [SerializeField] private EnemySO enemyData;
    
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
    
    private float currentShield;

    protected override void Start()
    {   
        if (enemyData != null)
        {
            // ตั้งค่า Stat เริ่มต้น
            maxHealth = Mathf.RoundToInt(enemyData.maxHp);
            currentShield = (enemyData.enemyType == EnemyType.Shieldler) ? enemyData.shieldHp : 0;
            
            rb = GetComponent<Rigidbody2D>();
            // ถ้าเป็นตัวบิน ให้ปิดแรงโน้มถ่วง
            if (enemyData.IsFlying)
            {
                rb.gravityScale = 0f; 
                
                // ⭐ เพิ่มตรงนี้: ดันตัวขึ้นไปบนฟ้าทันทีที่เกิด (สุ่มความสูงนิดหน่อยให้ดูไม่ซ้ำ)
                float flyHeight = Random.Range(1.5f, 3.0f);
                transform.position += Vector3.up * flyHeight;
            }
        }
        
        base.Start();
        
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;

        var baseObj = GameObject.FindWithTag("Base");
        if (baseObj != null) baseTarget = baseObj.transform;
    }

    private void FixedUpdate()
    {   
        if (isKnockedBack) return;
        
        if (enemyData == null) return;

        Transform currentTarget = GetClosestTarget();
        if (currentTarget == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.position);
        
        if (enemyData.enemyType == EnemyType.Bomber && distance <= enemyData.attackRange)
        {
            Explode();
            return;
        }
        
        if (distance <= enemyData.attackRange)
        {
            rb.linearVelocity = Vector2.zero; // หยุดเดิน
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
        
        if (enemyData.IsFlying)
        {
            // บิน: เคลื่อนที่ตามทิศทางเป้าหมายทั้งแกน X และ Y
            rb.linearVelocity = dir * enemyData.moveSpeed;
        }
        else
        {
            // เดินดิน: เคลื่อนที่เฉพาะแกน X ส่วนแกน Y ให้แรงโน้มถ่วงจัดการ
            rb.linearVelocity = new Vector2(dir.x * enemyData.moveSpeed, rb.linearVelocity.y);
        }

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
    
    private void Explode()
    {
        if (enemyData.explosionEffect != null)
        {
            Instantiate(enemyData.explosionEffect, transform.position, Quaternion.identity);
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, enemyData.explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") || hitCollider.CompareTag("Base"))
            {
                var dmg = hitCollider.GetComponent<IDamageable>();
                if (dmg != null) dmg.TakeDamage(enemyData.explosionDamage);
            }
        }
        
        Debug.Log("Bomber Exploded!");
        
        // ⭐ เพิ่มบรรทัดนี้สำคัญมาก! บอก GameManager ว่าตัวนี้ตายแล้วนะ
        GameManager.Instance.RegisterEnemyDeath(); 
        
        Destroy(gameObject);
    }
    
    public override void TakeDamage(float damage)
    {   
        if (currentShield > 0)
        {
            currentShield -= damage;
            Debug.Log($"Shield blocked! Current Shield: {currentShield}");
            if (currentShield <= 0)
            {
                Debug.Log("Shield Broken!");
                // อาจจะเล่นเสียงโล่แตกตรงนี้
            }
            return; // รับดาเมจเข้าโล่แทน เลือดไม่ลด
        }
        
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
        
        if (enemyData.enemyType == EnemyType.Bomber)
        {
            Explode(); // ตายแล้วระเบิดด้วย (Death Rattle)
        }

        if (WeaponSystem.Instance != null && WeaponSystem.Instance.HasVampireShot)
        {
            PlayerHealth.Instance.Heal(WeaponSystem.Instance.LifeStealAmount);
        }
        
        if (enemyData.enemyType == EnemyType.Bomber)
        {
            Explode(); 
            return; // ⭐ จบการทำงานตรงนี้เลย ไม่ต้องไปทำข้างล่างซ้ำ
        }
        
        Debug.Log($"{name} died!");
        
        GameManager.Instance.RegisterEnemyDeath();
        Destroy(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (enemyData != null && enemyData.enemyType == EnemyType.Bomber)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.explosionRadius);
        }
    }
    
}
