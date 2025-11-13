using UnityEngine;

public class PlayerHealth : Character, IDamageable
{   
    public static PlayerHealth Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // ลดเลือดตามระบบ Character

        // อัปเดต UI เฉพาะผู้เล่น
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        Debug.Log("MaxHealth Increased: " + maxHealth);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // อัปเดต UI ตอนฮีลด้วย
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
    }

    protected override void Die()
    {
        Debug.Log("Player Died!");
        GameManager.Instance.OnPlayerDeath();
    }
}