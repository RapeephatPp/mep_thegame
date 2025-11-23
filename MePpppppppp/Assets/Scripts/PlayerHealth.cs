using UnityEngine;

public class PlayerHealth : Character, IDamageable
{   
    public static PlayerHealth Instance { get; private set; }
    
    [Header("Last Stand")]
    [SerializeField, Range(0f,1f)] private float lastStandHpPercent = 0.3f;   
    [SerializeField] private float lastStandDamageMultiplier = 0.6f;         
    
    private void Awake()
    {
        Instance = this;
    }

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public override void TakeDamage(float damage)
    {
        // เช็คเลือดก่อนโดนดาเมจ
        float hpRatio = (float)currentHealth / maxHealth;
        if (hpRatio <= lastStandHpPercent)
        {
            damage *= lastStandDamageMultiplier;
        }

        base.TakeDamage(damage);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        Debug.Log("MaxHealth Increased: " + maxHealth);
    }
    
    protected override void Start()
    {
        base.Start(); // ตั้งค่า currentHealth = maxHealth ก่อน
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // อัปเดต UI ตอนฮีลด้วย
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
    }
    
    public void SetHealthFromRun(int current, int max)
    {
        maxHealth = max;
        currentHealth = Mathf.Clamp(current, 0, maxHealth);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
    }

    protected override void Die()
    {
        Debug.Log("Player Died!");
        GameManager.Instance.OnPlayerDeath();
    }
}