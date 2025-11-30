using UnityEngine;

public class BaseExit : MonoBehaviour
{
    [SerializeField] private BaseSceneManager baseManager;
    [SerializeField] private KeyCode useKey = KeyCode.E;
    
    [Header("Audio")]
    [SerializeField] private AudioClip useDoorClip;

    private bool playerInZone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = true;
        // ตรงนี้ถ้ามี UI ให้แสดงข้อความ "กด E เพื่อออกไปสู้ต่อ" ก็เรียกได้
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = false;
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
    }
}