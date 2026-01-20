using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Player UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthBarTrail; 
    // [NEW] ตัวพ่อของหลอดเลือด (Container) ที่จะให้มันสั่น
    [SerializeField] private RectTransform healthBarContainer; 
    
    [Header("Shake Settings")] // [NEW] ตั้งค่าความแรง
    [SerializeField] private float defaultShakeDuration = 0.2f;
    [SerializeField] private float defaultShakeMagnitude = 5f; // ความแรงในการสั่น (หน่วยเป็น pixel ของ UI)

    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image reloadBar;

    // ... (Header อื่นๆ เหมือนเดิม) ...
    [Header("Warning UI")] 
    [SerializeField] private GameObject warningLeft;
    [SerializeField] private GameObject warningRight;           
    [Header("Base UI")]
    [SerializeField] private Image baseHealthBar;
    [SerializeField] private Image baseHealthBarTrail; 
    [SerializeField] private RectTransform baseHealthBarContainer;
    [Header("Settings")]
    [SerializeField] private float trailSpeed = 2f; 
    [Header("Game Info")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI stateText;
    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [Header("Death UI")]
    [SerializeField] private GameObject deathPanel;

    private Coroutine trailCoroutine;
    private Coroutine baseTrailCoroutine;
    private Coroutine shakeCoroutine; // [NEW] เก็บ Coroutine สั่นไว้กันตีกัน
    private Coroutine baseShakeCoroutine;

    private void Awake()
    {
        Instance = this;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (deathPanel != null) deathPanel.SetActive(false);
    }

    private void Start()
    {
        // ... (โค้ดเดิม) ...
        if (PlayerHealth.Instance != null)
            UpdateHealthBar(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth);

        var gm = GameManager.Instance;
        if (gm != null && gm.BaseCtrl != null)
            UpdateBaseHealth(gm.BaseCtrl.CurrentHealth, gm.BaseCtrl.MaxHealth);

        if (gm != null)
        {
            UpdateWave(gm.CurrentWave);
            UpdateGameState("Running");
        }
    }
    
    // ================= SHAKE LOGIC [NEW] =================
    public void ShakeHealthBar()
    {
        if (healthBarContainer != null)
        {
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(ShakeRoutine(healthBarContainer, defaultShakeDuration, defaultShakeMagnitude));
        }
    }
    
    public void ShakeBaseHealthBar()
    {
        if (baseHealthBarContainer != null)
        {
            // ถ้ากำลังสั่นอยู่ ให้หยุดอันเก่าแล้วเริ่มสั่นใหม่ (Reset การสั่น)
            if (baseShakeCoroutine != null) StopCoroutine(baseShakeCoroutine);
            baseShakeCoroutine = StartCoroutine(ShakeRoutine(baseHealthBarContainer, defaultShakeDuration, defaultShakeMagnitude));
        }
        else
        {
            Debug.LogWarning("อย่าลืมลาก BaseHealthBarContainer ใส่ใน UIManager นะ!");
        }
    }

    private IEnumerator ShakeRoutine(RectTransform target, float duration, float magnitude)
    {
        Vector3 originalPos = target.anchoredPosition; // จำตำแหน่งเดิมไว้
        float elapsed = 0f;

        while (elapsed < duration)
        {   
            if (Time.timeScale == 0f)
            {
                target.anchoredPosition = originalPos; // คืนค่าเดิม
                yield break; // จบการทำงานทันที
            }
            
            // สุ่มตำแหน่ง X, Y เล็กน้อย
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            target.anchoredPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        target.anchoredPosition = originalPos; // คืนค่าตำแหน่งเดิมเป๊ะๆ ตอนจบ
    }

    // ... (ส่วนอื่นๆ เหมือนเดิมเป๊ะ) ...
    public void UpdateHealthBar(float current, float max)
    {
        float targetFill = current / max;
        if (healthBar != null) healthBar.fillAmount = targetFill;
        if (healthBarTrail != null)
        {
            if (trailCoroutine != null) StopCoroutine(trailCoroutine);
            trailCoroutine = StartCoroutine(UpdateTrail(healthBarTrail, targetFill));
        }
    }
    
    // ... Copy โค้ดเดิมส่วนที่เหลือมาแปะตรงนี้ (UpdateBaseHealth, UpdateTrail, WarningRoutine ฯลฯ) ...
    public void UpdateBaseHealth(float current, float max)
    {
        float targetFill = current / max;

        if (baseHealthBar != null) baseHealthBar.fillAmount = targetFill;

        if (baseHealthBarTrail != null)
        {
            if (baseTrailCoroutine != null) StopCoroutine(baseTrailCoroutine);
            baseTrailCoroutine = StartCoroutine(UpdateTrail(baseHealthBarTrail, targetFill));
        }
    }
    private IEnumerator UpdateTrail(Image trailImage, float targetFill)
    {
        // รอแป๊บนึงให้เห็น impact
        yield return new WaitForSeconds(0.3f);

        // ค่อยๆ ลดหลอดขาว
        while (trailImage.fillAmount > targetFill)
        {
            trailImage.fillAmount = Mathf.Lerp(trailImage.fillAmount, targetFill, Time.deltaTime * trailSpeed);
            yield return null;
        }

        trailImage.fillAmount = targetFill;
    }
    public void ShowWarning(SpawnSide side, float duration) { StopCoroutine("WarningRoutine"); StartCoroutine(WarningRoutine(side, duration)); }
    private IEnumerator WarningRoutine(SpawnSide side, float duration)
    {
        bool showLeft = (side == SpawnSide.Left || side == SpawnSide.Both);
        bool showRight = (side == SpawnSide.Right || side == SpawnSide.Both);
        if (warningLeft) warningLeft.SetActive(showLeft);
        if (warningRight) warningRight.SetActive(showRight);
        CanvasGroup cgLeft = warningLeft?.GetComponent<CanvasGroup>();
        if (cgLeft == null && warningLeft) cgLeft = warningLeft.AddComponent<CanvasGroup>();
        CanvasGroup cgRight = warningRight?.GetComponent<CanvasGroup>();
        if (cgRight == null && warningRight) cgRight = warningRight.AddComponent<CanvasGroup>();
        float timer = 0f;
        while (timer < duration)
        {   
            if (Time.timeScale == 0f)
            {
                if (warningLeft) warningLeft.SetActive(false);
                if (warningRight) warningRight.SetActive(false);
                yield break; 
            }
            
            timer += Time.deltaTime;
            float alpha = 0.2f + (0.8f * Mathf.Abs(Mathf.Sin(timer * 10f)));
            if (showLeft && cgLeft) cgLeft.alpha = alpha;
            if (showRight && cgRight) cgRight.alpha = alpha;
            yield return null;
        }
        if (warningLeft) warningLeft.SetActive(false);
        if (warningRight) warningRight.SetActive(false);
    }
    public void TogglePausePanel(bool show) { if (pausePanel) pausePanel.SetActive(show); }
    public void ToggleDeathPanel(bool show) { if (deathPanel) deathPanel.SetActive(show); }
    public void UpdateAmmo(int c, int m) { if (ammoText) ammoText.text = $"Ammo: {c}/{m}"; }
    public void UpdateReload(float p) { if (reloadBar) reloadBar.fillAmount = p; }
    public void UpdateWave(int w) { if (waveText) waveText.text = $"Wave {w}"; }
    public void UpdateGameState(string s) { if (stateText) stateText.text = s; }
}