using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance;

    [SerializeField] private GameObject upgradeCanvas;
    [SerializeField] private Button upgradeButton1;
    [SerializeField] private Button upgradeButton2;
    [SerializeField] private Button upgradeButton3;

    private UpgradeSystem upgradeSystem;

    private void Awake()
    {
        Instance = this;
        upgradeSystem = FindAnyObjectByType<UpgradeSystem>();
        
        if (upgradeSystem == null)
            Debug.LogError("No upgrade system found");
        
        
    }

    
    public void ShowUpgradeOptions()
    {   
        Debug.Log("🟢 Upgrade UI Activated");

        if (upgradeCanvas == null)
        {
            Debug.LogError("❌ UpgradeCanvas is not assigned!");
            return;
        }

        upgradeCanvas.SetActive(true);

        upgradeButton1.onClick.RemoveAllListeners();
        upgradeButton2.onClick.RemoveAllListeners();
        upgradeButton3.onClick.RemoveAllListeners();

        upgradeButton1.onClick.AddListener(() => SelectUpgrade(UpgradeType.IncreaseDamage));
        upgradeButton2.onClick.AddListener(() => SelectUpgrade(UpgradeType.IncreaseFireRate));
        upgradeButton3.onClick.AddListener(() => SelectUpgrade(UpgradeType.RepairBase));
    }

    private void SelectUpgrade(UpgradeType type)
    {
        if (upgradeSystem = null)
        {
            Debug.LogError($"upgradeSystem is Null!");
            return;
        }
        
        upgradeSystem.ApplyUpgrade(type);
        CloseUI();
    }

    private void CloseUI()
    {
        upgradeCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}