using UnityEngine;

public class HealZone : MonoBehaviour
{
    [SerializeField] private int healAmount = 5;
    [SerializeField] private float interval = 0.5f;

    private float timer;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (PlayerHealth.Instance == null) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            PlayerHealth.Instance.Heal(healAmount);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        timer = 0f;
    }
}