using UnityEngine;

public class BaseController : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= Mathf.RoundToInt(amount);
        Debug.Log($"Base took {amount} damage. HP: {currentHealth}");

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