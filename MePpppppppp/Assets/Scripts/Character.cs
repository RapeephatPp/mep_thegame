using UnityEngine;

public abstract class Character : MonoBehaviour
{   
    [Header("Character Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= Mathf.FloorToInt(damage);
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}");

        // อัปเดตเฉพาะ Player
        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    protected abstract void Die(); // ให้คลาสลูกนิยามเอง
}