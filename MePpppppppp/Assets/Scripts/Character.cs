using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected abstract void Die(); // ให้คลาสลูกนิยามเอง
}