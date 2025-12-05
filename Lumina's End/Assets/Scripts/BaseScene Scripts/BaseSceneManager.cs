using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseSceneManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Scenes")]
    [SerializeField] private string battleSceneName = "SampleScene"; // ชื่อฉากต่อสู้หลัก
    
    [Header("Audio")]
    [SerializeField] private AudioClip baseBGM;
    
    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBGM(baseBGM, true);

        SpawnPlayerAndApplyStats();
        
        Invoke("CallBaseTutorial", 0.5f);
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateGameState("Resting");
            UIManager.Instance.UpdateWave(RunData.currentWave);   // โชว์ wave เดิมที่เล่นอยู่
        }
    }
    
    void CallBaseTutorial()
    {
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.TriggerBaseTutorial();
    }

    void SpawnPlayerAndApplyStats()
    {
        if (playerPrefab == null || playerSpawnPoint == null)
        {
            Debug.LogError("BaseSceneManager: missing playerPrefab or spawnPoint");
            return;
        }

        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

        // โหลดค่า HP / อาวุธ จาก RunData
        if (RunData.HasData)
        {
            var ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.SetHealthFromRun(RunData.playerCurrentHp, RunData.playerMaxHp);
            }

            var weapon = WeaponSystem.Instance;
            if (weapon == null)
                weapon = player.GetComponentInChildren<WeaponSystem>();
            if (weapon != null)
            {
                WeaponSystem.Instance.SetWeaponFromRun(
                    RunData.bulletDamage,
                    RunData.bulletSpeed,
                    RunData.fireCooldown,
                    RunData.maxAmmo,
                    RunData.reloadTime
                );
            }
        }
    }

    public void ReturnToBattle()
    {
        // เซฟค่าใหม่กลับไปก่อนออก
        if (PlayerHealth.Instance != null)
        {
            RunData.playerCurrentHp = PlayerHealth.Instance.CurrentHealth;
            RunData.playerMaxHp = PlayerHealth.Instance.MaxHealth;
        }

        if (WeaponSystem.Instance != null)
        {
            RunData.bulletDamage = WeaponSystem.Instance.BulletDamage;
            RunData.bulletSpeed  = WeaponSystem.Instance.BulletSpeed;
            RunData.fireCooldown = WeaponSystem.Instance.FireCooldown;
            RunData.maxAmmo      = WeaponSystem.Instance.MaxAmmo;
            RunData.reloadTime   = WeaponSystem.Instance.ReloadTime;

            RunData.vampireShot        = WeaponSystem.Instance.HasVampireShot;
            RunData.lifeStealAmount    = WeaponSystem.Instance.LifeStealAmount;
            RunData.adrenalineRush     = WeaponSystem.Instance.HasAdrenalineRush;
            RunData.adrenalineMultiplier = WeaponSystem.Instance.AdrenalineMultiplier;
            RunData.adrenalineDuration   = WeaponSystem.Instance.AdrenalineDuration;
        }


        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(battleSceneName);
        else
            SceneManager.LoadScene(battleSceneName);
    }
}
