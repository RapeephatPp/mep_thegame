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
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image reloadBar;

    [Header("Warning UI")] 
    [SerializeField] private GameObject warningLeft;
    [SerializeField] private GameObject warningRight;           

    [Header("Base UI")]
    [SerializeField] private Image baseHealthBar;

    [Header("Game Info")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI stateText;
    
    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    
    [Header("Death UI")]
    [SerializeField] private GameObject deathPanel;
    
    private void Start()
    {
        // Player HP (ถ้ามี)
        if (PlayerHealth.Instance != null)
            UpdateHealthBar(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth);

        // Base HP ผ่าน GameManager
        var gm = GameManager.Instance;
        if (gm != null && gm.BaseCtrl != null)
            UpdateBaseHealth(gm.BaseCtrl.CurrentHealth, gm.BaseCtrl.MaxHealth);

        // Wave & State เริ่มต้น
        if (gm != null)
        {
            UpdateWave(gm.CurrentWave);   // ใช้ property ที่เพิ่มในข้อ 2
            UpdateGameState("Running");
        }
    }

    /*private void Update()
    {
        UpdateHealthBar(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth);
    }*/

    private void Awake()
    {
        Instance = this;
        Debug.Log("[UIManager] Awake in scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (deathPanel != null)
            deathPanel.SetActive(false);
        
    }
    
    //Warning UI
    public void ShowWarning(SpawnSide side, float duration)
    {
        StopCoroutine("WarningRoutine");
        StartCoroutine(WarningRoutine(side, duration));
    }
    
    private IEnumerator WarningRoutine(SpawnSide side, float duration)
    {
        // เช็คว่าจะเปิดฝั่งไหนบ้าง
        bool showLeft = (side == SpawnSide.Left || side == SpawnSide.Both);
        bool showRight = (side == SpawnSide.Right || side == SpawnSide.Both);

        // Reset state
        if (warningLeft) warningLeft.SetActive(showLeft);
        if (warningRight) warningRight.SetActive(showRight);

        // ดึง CanvasGroup มาเตรียมไว้
        CanvasGroup cgLeft = warningLeft?.GetComponent<CanvasGroup>();
        if (cgLeft == null && warningLeft) cgLeft = warningLeft.AddComponent<CanvasGroup>();
        
        CanvasGroup cgRight = warningRight?.GetComponent<CanvasGroup>();
        if (cgRight == null && warningRight) cgRight = warningRight.AddComponent<CanvasGroup>();

        float timer = 0f;
        float blinkSpeed = 10f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = 0.2f + (0.8f * Mathf.Abs(Mathf.Sin(timer * blinkSpeed)));

            if (showLeft && cgLeft) cgLeft.alpha = alpha;
            if (showRight && cgRight) cgRight.alpha = alpha;

            yield return null;
        }

        if (warningLeft) warningLeft.SetActive(false);
        if (warningRight) warningRight.SetActive(false);
    }
    
    //Pause UI
    public void TogglePausePanel(bool show)
    {
        if (pausePanel != null)
            pausePanel.SetActive(show);
    }
    
    //Death UI
    public void ToggleDeathPanel(bool show)
    {
        if (deathPanel != null)
            deathPanel.SetActive(show);
    }
    
    // ---------- Health ----------
    public void UpdateHealthBar(float current, float max)
    {
        /*Debug.Log($"[UIManager] UpdateHealthBar called: {current}/{max}");*/
        if (healthBar != null)
            healthBar.fillAmount = current / max;
        else
            Debug.LogWarning("HealthBar is not assigned in UIManager!");
    }
    
    // ---------- Ammo ----------
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
            ammoText.text = $"Ammo: {currentAmmo}/{maxAmmo}";
    }

    // ---------- Reload ----------
    public void UpdateReload(float progress)
    {
        if (reloadBar != null)
            reloadBar.fillAmount = progress;
    }
    
    // ---------- Base ----------
    public void UpdateBaseHealth(float current, float max)
    {
        if (baseHealthBar != null)
            baseHealthBar.fillAmount = current / max;
    }
    
    // ---------- Wave ----------
    public void UpdateWave(int wave)
    {
        if (waveText != null)
            waveText.text = $"Wave {wave}";
    }

    // ---------- Game State ----------
    public void UpdateGameState(string state)
    {
        if (stateText != null)
            stateText.text = state;
    }
}