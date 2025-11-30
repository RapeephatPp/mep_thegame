using UnityEngine;

public class BaseController : MonoBehaviour, IDamageable
{   
    [Header("Audio")]
    [SerializeField] private AudioClip baseHitClip;
    [SerializeField] private AudioClip baseDestroyedClip;
    public static BaseController Instance { get; private set; }
    
    private int maxHealth = 1000;
    private int currentHealth;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    
    public void SetHealthFromRun(int current, int max)
    {
        maxHealth = max;
        currentHealth = Mathf.Clamp(current, 0, maxHealth);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateBaseHealth(currentHealth, maxHealth);
    }

    
    private void Awake()
    {   
        Instance = this;
        currentHealth = maxHealth;
    }
    
    public void HealBase(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Base healed by {amount}, current HP: {currentHealth}");
        UIManager.Instance.UpdateBaseHealth(currentHealth, maxHealth);
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateBaseHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= Mathf.RoundToInt(amount);
        Debug.Log($"Base took {amount} damage. HP: {currentHealth}");
        
        if (AudioManager.Instance != null && baseHitClip != null)
            AudioManager.Instance.PlaySFX(baseHitClip, 0.95f, 1.05f);
        
        // อัปเดต UI
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateBaseHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Base destroyed! Game Over!");
            GameManager.Instance.OnBaseDestroyed();
        }
    }
}