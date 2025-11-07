using UnityEngine;

public class BaseController : Character, IDamageable
{
    protected override void Die()
    {
        Debug.Log("Base destroyed! Game Over!");
        GameManager.Instance.OnBaseDestroyed();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + Mathf.FloorToInt(amount), 0, maxHealth);
        Debug.Log($"Base Healed! Current Health: {currentHealth}/{maxHealth}");
    }
}