using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("References")]
    [SerializeField] private FloatingText floatingTextPrefab;
    private FloatingText currentTextInstance;

    [Header("Audio")]
    [SerializeField] private AudioClip successClip; // เสียงติ๊ง! เมื่อทำผ่าน

    // Flags เช็คสถานะ
    private bool hasWalked = false;
    private bool hasShot = false;
    private bool hasReloaded = false;
    private bool cardPhaseShown = false;
    private bool basePhaseShown = false;

    // เช็คว่าเคยจบคอร์สไปหรือยัง
    private bool IsTutorialComplete => PlayerPrefs.GetInt("TutorialComplete", 0) == 1;

    void Awake()
    {
        // Singleton แบบอยู่ข้ามฉากได้ (เพราะต้องตามไป BaseScene)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // ถ้าเรียนจบแล้ว ไม่ต้องทำอะไร
        if (IsTutorialComplete) return;

        // เริ่มมาสอนเดินก่อนเลย
        Invoke("StartWalkingTutorial", 1f); 
    }

    // --- STEP 1: WALKING ---
    void StartWalkingTutorial()
    {
        ShowMessage("Press [A] [D] to Move");
    }

    public void OnPlayerMove()
    {
        if (IsTutorialComplete || hasWalked) return;

        hasWalked = true;
        PlaySuccessSound();
        
        // ไปขั้นต่อไป: สอนยิง
        ShowMessage("Mouse to Aim\n[LMB] to Shoot");
    }

    // --- STEP 2: SHOOTING ---
    public void OnPlayerShoot()
    {
        if (IsTutorialComplete || !hasWalked || hasShot) return;

        hasShot = true;
        PlaySuccessSound();
        
        // ไปขั้นต่อไป: ซ่อนข้อความก่อน รอจังหวะรีโหลดค่อยโผล่
        HideMessage(); 
    }

    // --- STEP 3: RELOAD ---
    // (เรียกจาก WeaponSystem ตอนกระสุนหมด)
    public void TriggerReloadTutorial()
    {
        if (IsTutorialComplete || hasReloaded) return;
        
        ShowMessage("Press [R] to Reload!");
    }

    public void OnPlayerReload()
    {
        if (IsTutorialComplete || hasReloaded) return;

        hasReloaded = true;
        PlaySuccessSound();
        HideMessage();
    }

    // --- STEP 4: CARD SELECTION ---
    // (เรียกจาก GameManager ตอนจบ Wave)
    public void TriggerCardTutorial()
    {
        if (IsTutorialComplete || cardPhaseShown) return;

        cardPhaseShown = true;
        ShowMessage("Choose a Card\nto Upgrade!");
    }

    // --- STEP 5: BASE SCENE ---
    // (เรียกจาก BaseSceneManager)
    public void TriggerBaseTutorial()
    {
        // ถือว่าจบทูทอเรียลหลักแล้ว บันทึกเลยก็ได้
        if (IsTutorialComplete) return;
        
        basePhaseShown = true;
        ShowMessage("Welcome to Base!");
        
        // จบคอร์ส!
        PlayerPrefs.SetInt("TutorialComplete", 1);
        PlayerPrefs.Save();
        
        // ซ่อนข้อความหลังผ่านไป 5 วิ
        StartCoroutine(HideAfterDelay(5f));
    }

    // --- Helpers ---
    private void ShowMessage(string msg)
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return; // ถ้าหาไม่เจอ (เช่นอยู่ในฉากโหลด)

        if (currentTextInstance == null)
        {
            currentTextInstance = Instantiate(floatingTextPrefab);
            DontDestroyOnLoad(currentTextInstance.gameObject); // ให้ตัวอักษรตามข้ามฉากได้ถ้าจำเป็น
        }

        currentTextInstance.Show(msg, player);
    }

    private void HideMessage()
    {
        if (currentTextInstance != null) currentTextInstance.Hide();
    }
    
    public void OnCardSelected()
    {
        // ถ้าอยู่ในสถานะสอนเลือกการ์ดอยู่ ให้ซ่อนข้อความ
        if (cardPhaseShown)
        {
            HideMessage();
        }
    }
    
    private void PlaySuccessSound()
    {
        if (AudioManager.Instance != null && successClip != null)
            AudioManager.Instance.PlaySFX(successClip);
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideMessage();
    }
}