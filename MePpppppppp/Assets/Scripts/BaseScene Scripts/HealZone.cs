using UnityEngine;

public class HealZone : MonoBehaviour
{   
    [Header("Heal Zone")]
    [SerializeField] private int healAmount = 5;
    [SerializeField] private float interval = 0.5f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip healClip;

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
            
            if (AudioManager.Instance != null && healClip != null)
                AudioManager.Instance.PlaySFX(healClip, 0.95f, 1.05f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        timer = 0f;
    }
}