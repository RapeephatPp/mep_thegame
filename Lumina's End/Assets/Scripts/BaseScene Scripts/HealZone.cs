using UnityEngine;

public class HealZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int healAmount = 5;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private AudioClip healClip;

    [Header("UI Settings")]
    [SerializeField] private GameObject interactUI; // ลากตัว UI ที่เป็นลูกของ HealZone มาใส่
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // ความสูงที่จะให้ลอยเหนือหัว

    private Transform targetPlayer; // ตัวแปรเก็บตำแหน่งผู้เล่น
    private bool isPlayerInRange = false;

    private void Start()
    {
        // เริ่มเกมมาซ่อน UI ของตัวเองไว้ก่อน
        if (interactUI != null) interactUI.SetActive(false);
    }

    private void Update()
    {
        // ถ้ามีผู้เล่นอยู่ในระยะ และมี UI
        if (isPlayerInRange && targetPlayer != null)
        {
            // 1. สั่งให้ UI กระโดดไปที่ตำแหน่งผู้เล่น + ความสูง
            if (interactUI != null)
            {
                interactUI.transform.position = targetPlayer.position + uiOffset;
            }

            // 2. เช็คการกดปุ่ม
            if (Input.GetKeyDown(interactKey))
            {
                HealPlayer();
            }
        }
    }

    private void HealPlayer()
    {
        if (PlayerHealth.Instance != null)
        {
            if (PlayerHealth.Instance.CurrentHealth >= PlayerHealth.Instance.MaxHealth)
            {
                // อาจจะเพิ่ม Feedback ว่าเลือดเต็มแล้วตรงนี้
                return;
            }

            PlayerHealth.Instance.Heal(healAmount);
            
            if (AudioManager.Instance != null && healClip != null)
            {
                AudioManager.Instance.PlaySFX(healClip, 1f, 1f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            targetPlayer = other.transform; // จำไว้ว่าผู้เล่นคนไหนเดินเข้ามา

            if (interactUI != null) interactUI.SetActive(true); // เปิด UI
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            targetPlayer = null; // ลืมผู้เล่นคนนั้นซะ

            if (interactUI != null) interactUI.SetActive(false); // ปิด UI
        }
    }
}