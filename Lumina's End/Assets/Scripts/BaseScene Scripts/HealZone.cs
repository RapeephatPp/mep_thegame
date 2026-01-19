using UnityEngine;

public class HealZone : MonoBehaviour
{
    [Header("Heal Settings")]
    [SerializeField] private int healAmount = 100;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private AudioClip healClip;

    [Header("UI Settings")]
    [SerializeField] private GameObject interactUI; // ลาก UI "กด E" ใส่ตรงนี้
    [SerializeField] private GameObject usedUI;     // ลาก UI "พักไปแล้ว" ใส่ตรงนี้
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0); // ความสูงพื้นฐาน

    [Header("Floating Animation")] // [NEW] ส่วนตั้งค่าการขยับ
    [SerializeField] private float floatSpeed = 4f;      // ความเร็วในการขยับ
    [SerializeField] private float floatStrength = 0.2f; // ระยะการขยับขึ้นลง (ยิ่งเยอะยิ่งเด้งแรง)

    private bool hasHealed = false;
    private bool isPlayerInRange = false;
    private Transform targetPlayer;

    private void Start()
    {
        // เริ่มเกม ซ่อน UI ทั้งหมด
        if (interactUI) interactUI.SetActive(false);
        if (usedUI) usedUI.SetActive(false);
    }

    private void Update()
    {
        // ทำงานเมื่อผู้เล่นอยู่ในระยะ
        if (isPlayerInRange && targetPlayer != null)
        {
            // คำนวณตำแหน่งและ Animation
            UpdateUIPosition();

            // เช็คการกดปุ่ม (กดได้เฉพาะตอนยังไม่ฮีล)
            if (!hasHealed && Input.GetKeyDown(interactKey))
            {
                StartHealSequence();
            }
        }
    }

    private void UpdateUIPosition()
    {
        // เลือก UI ตัวที่เปิดอยู่มาขยับ
        GameObject activeUI = hasHealed ? usedUI : interactUI;
        
        if (activeUI != null)
        {
            // 1. คำนวณค่า Y ที่จะขยับขึ้นลง (ใช้ Time.time เพื่อให้ขยับตลอดเวลา)
            float yBounce = Mathf.Sin(Time.time * floatSpeed) * floatStrength;

            // 2. เอาตำแหน่งผู้เล่น + ความสูงที่ตั้งไว้ + ค่าขยับขึ้นลง
            activeUI.transform.position = targetPlayer.position + uiOffset + new Vector3(0, yBounce, 0);
        }
    }

    private void StartHealSequence()
    {
        // สั่ง SceneFader ให้ทำงาน (ถ้ามี)
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeAndExecute(PerformHeal);
        }
        else
        {
            PerformHeal();
        }
    }

    private void PerformHeal()
    {
        // Logic การฮีล
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.Heal(healAmount);
            
            if (AudioManager.Instance != null && healClip != null)
            {
                AudioManager.Instance.PlaySFX(healClip, 1f, 1f);
            }
        }

        hasHealed = true; // จำว่าฮีลแล้ว

        // สลับ UI
        if (interactUI) interactUI.SetActive(false);
        if (usedUI) usedUI.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            targetPlayer = other.transform;

            // เปิด UI ตามสถานะ
            if (hasHealed)
            {
                if (usedUI) usedUI.SetActive(true);
            }
            else
            {
                if (interactUI) interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            targetPlayer = null;

            // ปิด UI ทั้งหมด
            if (interactUI) interactUI.SetActive(false);
            if (usedUI) usedUI.SetActive(false);
        }
    }
}