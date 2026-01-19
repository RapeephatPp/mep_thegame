using UnityEngine;

public class BaseExit : MonoBehaviour
{
    [SerializeField] private BaseSceneManager baseManager;
    [SerializeField] private KeyCode useKey = KeyCode.E;
    
    [Header("Audio")]
    [SerializeField] private AudioClip useDoorClip;
    
    [Header("UI Settings")]
    [SerializeField] private GameObject interactUI; 
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0);
    
    [Header("Floating Animation")] 
    [SerializeField] private float floatSpeed = 4f;      
    [SerializeField] private float floatStrength = 0.2f;

    private Transform targetPlayer; // เก็บตำแหน่งผู้เล่น
    private bool isPlayerInRange = false; 
    
    private void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            targetPlayer = other.transform; 
            if (interactUI != null) interactUI.SetActive(true); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            targetPlayer = null; 
            if (interactUI != null) interactUI.SetActive(false); 
        }
    }

    private void Update()
    {   
        // ถ้าไม่อยู่ในระยะ ไม่ต้องทำอะไรเลย
        if (!isPlayerInRange || targetPlayer == null) return;

        // 1. จัดการตำแหน่ง UI (รวม Animation แล้ว)
        UpdateUIPosition();
        
        if (Input.GetKeyDown(useKey) && baseManager != null)
        {   
            PlaySound();
            baseManager.ReturnToBattle();
        }
    }
    
    private void PlaySound()
    {
        if (AudioManager.Instance != null && useDoorClip != null)
            AudioManager.Instance.PlaySFX(useDoorClip);
    }
    
    private void UpdateUIPosition()
    {
        if (interactUI != null && targetPlayer != null)
        {
            // คำนวณการเด้ง
            float yBounce = Mathf.Sin(Time.time * floatSpeed) * floatStrength;

            // set ตำแหน่งทีเดียวจบ (ผู้เล่น + offset + เด้ง)
            interactUI.transform.position = targetPlayer.position + uiOffset + new Vector3(0, yBounce, 0);
        }
    }
}