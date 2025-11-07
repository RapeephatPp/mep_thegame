using UnityEngine;

public class PlayerHealth : Character, IDamageable
{   
    public static PlayerHealth Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; // บวกเลือดปัจจุบันตาม max ใหม่ (ถ้าต้องการ)

        Debug.Log("MaxHealth Increased: " + maxHealth);
    }
    
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
    
    protected override void Die()
    {
        Debug.Log("Player Died!");
        GameManager.Instance.OnPlayerDeath();
    }
    
}