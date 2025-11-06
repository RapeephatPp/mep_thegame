using UnityEngine;

public class EnemyController : CharacterBase, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 1.5f;
    
    public Transform targetBase;
    private Rigidbody2D rb;
    private Transform playerTarget;
    private Transform baseTarget;
    private float nextAttackTime;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        playerTarget = GameObject.FindWithTag("Player").transform;
        baseTarget = GameObject.FindWithTag("Base").transform;
    }

    private void FixedUpdate()
    {
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
        Vector2 dir = new Vector2(target.position.x - transform.position.x, 0).normalized;
        rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
        transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
    }

    private void TryAttack(Transform target)
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(attackDamage);
        }
    }

    protected override void Die()
    {   
        Debug.Log($"{name} died!");
        Destroy(gameObject);
    }
}
