using UnityEngine;

public class BaseController : CharacterBase, IDamageable
{
    protected override void Die()
    {
        Debug.Log("Base destroyed! Game Over!");
        GameManager.Instance.OnBaseDestroyed();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"base Healed! Current Health: {currentHealth}/{maxHealth}");
    }
}