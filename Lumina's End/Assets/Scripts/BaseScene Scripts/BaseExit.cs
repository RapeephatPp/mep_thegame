using UnityEngine;

public class BaseExit : MonoBehaviour
{
    [SerializeField] private BaseSceneManager baseManager;
    [SerializeField] private KeyCode useKey = KeyCode.E;
    
    [Header("Audio")]
    [SerializeField] private AudioClip useDoorClip;
    
    [Header("UI Settings")]
    [SerializeField] private GameObject interactUI; // ลากตัว UI ที่เป็นลูกของ HealZone มาใส่
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // ความสูงที่จะให้ลอยเหนือหัว

    private bool playerInZone;
    
    private Transform targetPlayer; // ตัวแปรเก็บตำแหน่งผู้เล่น
    private bool isPlayerInRange = false;
    
    private void Start()
    {
        // เริ่มเกมมาซ่อน UI ของตัวเองไว้ก่อน
        if (interactUI != null) interactUI.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = true;
        
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            targetPlayer = other.transform; // จำไว้ว่าผู้เล่นคนไหนเดินเข้ามา

            if (interactUI != null) interactUI.SetActive(true); // เปิด UI
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = false;
        
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            targetPlayer = null; // ลืมผู้เล่นคนนั้นซะ

            if (interactUI != null) interactUI.SetActive(false); // ปิด UI
        }
    }

    private void Update()
    {
        if (!playerInZone) return;
        if (Input.GetKeyDown(useKey) && baseManager != null)
        {   
            if (AudioManager.Instance != null && useDoorClip != null)
                AudioManager.Instance.PlaySFX(useDoorClip);
            
            baseManager.ReturnToBattle();
        }

        if (isPlayerInRange && targetPlayer != null)
        {
            // 1. สั่งให้ UI กระโดดไปที่ตำแหน่งผู้เล่น + ความสูง
            if (interactUI != null)
            {
                interactUI.transform.position = targetPlayer.position + uiOffset;
            }
        }
    }
}