using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Player UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image reloadBar;

    [Header("Base UI")]
    [SerializeField] private Image baseHealthBar;

    [Header("Game Info")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI stateText;

    private void Awake()
    {
        Instance = this;
    }

    // ---------- Health ----------
    public void UpdateHealthBar(float current, float max)
    {
        Debug.Log($"[UIManager] UpdateHealthBar called: {current}/{max}");
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