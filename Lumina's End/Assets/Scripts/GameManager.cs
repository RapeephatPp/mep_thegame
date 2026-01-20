using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Running, Paused, CardSelection, GameOver }
public enum SpawnSide { Left, Right, Both }

[System.Serializable]
public class WaveData
{
    [Header("Wave Settings")]
    public string waveName;          // ตั้งชื่อกันลืม เช่น "Intro Left"
    public SpawnSide side;           // มาทางไหน
    public int enemyCount;           // กี่ตัว
    public float timeBetweenSpawns = 1.0f; 
    
    [Header("Enemies")]
    // ถ้ายังไม่มีมอนหลายแบบ ใช้แบบนี้ไปก่อนก็ได้ครับ
    public GameObject[] specificEnemies; // ลาก Prefab มาใส่ตรงนี้เลยก็ได้
}



public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Scene Names")]
    [SerializeField] private string baseSceneName = "BaseScene";

    
    [Header("Base Settings")]
    [SerializeField] private BaseController baseCtrl;
    
    [Header("Enemy Prefabs (For Generator)")]
    public GameObject prefabNormal;
    public GameObject prefabSpeed;
    public GameObject prefabTank;
    public GameObject prefabFlying;
    public GameObject prefabBomber;
    public GameObject prefabShield;

    [Header("Wave Configuration")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float timeBetweenSpawns = 1.2f;
    [SerializeField] private int baseEnemyCount = 5;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnLeft;
    [SerializeField] private Transform spawnRight;
    [SerializeField] private float warningDuration = 2.0f;
    
    [Header("Warning Settings")]
    [SerializeField] private float warningTime = 1.5f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip battleBGM;
    [SerializeField] private AudioClip battleAmbience;
    [SerializeField] private AudioClip waveStartClip;
    [SerializeField] private AudioClip waveClearClip;
    [SerializeField] private AudioClip pauseClip;
    [SerializeField] private AudioClip resumeClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip openDoorClip;
    
    public GameState CurrentGameState { get; private set; } = GameState.Running;
    public BaseController BaseCtrl => baseCtrl;

    int currentWave = 0;
    public int CurrentWave => currentWave;
    private int currentWaveIndex = 0;
    int aliveEnemies = 0;
    int enemiesToSpawn = 0;

    bool isSpawning = false;        // กำลังปล่อยมอนอยู่ไหม
    bool waitingForCard = false;    // กำลังรอให้ผู้เล่นเลือกการ์ดหรือไม่
    Coroutine waveRoutine;
    
    void Awake() 
    {
        Instance = this;
        // เพิ่มบรรทัดนี้ครับ: บังคับเวลาให้เดินเสมอเมื่อ GameManager ตื่นขึ้นมา
        Time.timeScale = 1f; 
    }

    void Start()
    {   
        Time.timeScale = 1f;
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBGM(battleAmbience, true);
        
        GenerateWaves();
        
        if (RunData.HasData)
        {
            LoadRunData();
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBGM(battleBGM, true);
        StartNextWave(); // เริ่มเวฟแรก
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentGameState == GameState.Running)
            {
                PauseGame();
            }
            else if (CurrentGameState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    // ---------- เวฟ ----------
    void StartNextWave()
    {   
        if (waveRoutine != null) StopCoroutine(waveRoutine);
        waitingForCard = false;
        CurrentGameState = GameState.Running;

        // เช็คว่ายังมีข้อมูลเวฟเหลือไหม
        if (currentWaveIndex < waves.Count)
        {
            currentWave = currentWaveIndex + 1; // โชว์ใน UI (เริ่มที่ 1)
            Debug.Log($"--- Starting Wave {currentWave}: {waves[currentWaveIndex].waveName} ---");
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateWave(currentWave);
                UIManager.Instance.UpdateGameState("Incoming!");
            }

            waveRoutine = StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
        else
        {
            Debug.Log("All waves cleared! You Win! (Or Loop)");
            // จบเกม หรือ วนลูป (Endless Mode logic) ตรงนี้
            // ตัวอย่าง: วนเวฟสุดท้ายแต่เพิ่มจำนวน
            // ...
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(waveStartClip);
    }

    IEnumerator SpawnWave(WaveData data)
    {
        isSpawning = true;
        aliveEnemies = data.enemyCount; // นับจำนวนที่จะต้องฆ่าในเวฟนี้

        // 1. แจ้งเตือน (Warning Phase)
        ShowWarning(data.side);
        yield return new WaitForSeconds(warningDuration);
        
        if (UIManager.Instance != null) UIManager.Instance.UpdateGameState("Battle!");

        // 2. ปล่อยมอนสเตอร์ (Spawning Phase)
        for (int i = 0; i < data.enemyCount; i++)
        {
            SpawnEnemyBasedOnSide(data.side, data);
            yield return new WaitForSeconds(data.timeBetweenSpawns);
        }

        isSpawning = false;
        // ไม่ต้องเรียก TryFinishWave() ตรงนี้ รอ Enemy ตายค่อยเช็คเหมือนเดิม
    }

    // ฟังก์ชันช่วยเลือกจุดเกิด
    void SpawnEnemyBasedOnSide(SpawnSide side, WaveData data)
    {
        Transform spawnPoint;

        if (side == SpawnSide.Left)
        {
            spawnPoint = spawnLeft;
        }
        else if (side == SpawnSide.Right)
        {
            spawnPoint = spawnRight;
        }
        else // SpawnSide.Both
        {
            // สุ่มซ้ายขวา หรือจะทำแบบ Left->Right->Left ก็ได้
            spawnPoint = (Random.value < 0.5f) ? spawnLeft : spawnRight;
        }

        // เลือก Prefab (ถ้าใน WaveData มีใส่ไว้ให้สุ่มจากในนั้น ถ้าไม่มีใช้ตัว Default)
        GameObject prefabToSpawn = enemyPrefab; 
        if (data.specificEnemies != null && data.specificEnemies.Length > 0)
        {
            prefabToSpawn = data.specificEnemies[Random.Range(0, data.specificEnemies.Length)];
        }

        Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
    }

    void SpawnEnemy(bool isLeft)
    {
        Transform p = isLeft ? spawnLeft : spawnRight;
        GameObject enemy = Instantiate(enemyPrefab, p.position, Quaternion.identity);
        aliveEnemies++;
    }
    
    void ShowWarning(SpawnSide side)
    {
        if (UIManager.Instance == null) return;

        // ส่ง enum ไปให้ UIManager จัดการ
        UIManager.Instance.ShowWarning(side, warningDuration);
    }

    // เรียกจาก EnemyController.Die()
    public void RegisterEnemyDeath()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        TryFinishWave();
    }

    void TryFinishWave()
    {   
        bool isPlayerAlive = (PlayerHealth.Instance != null && PlayerHealth.Instance.CurrentHealth > 0);
        
        // จบเวฟได้เมื่อ: ไม่ได้สปอว์นแล้ว และ ศัตรูเหลือ 0 และ เกมอยู่สถานะ Running
        if (!isSpawning && aliveEnemies <= 0 && CurrentGameState == GameState.Running && !waitingForCard)
        {
            Debug.Log("Wave cleared!");
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(waveClearClip, 0.95f, 1.05f);
            
            ShowCardSelection();
        }
    }

    // ---------- การ์ด ----------
    void ShowCardSelection()
    {
        waitingForCard = true;
        CurrentGameState = GameState.CardSelection;
        
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.TriggerCardTutorial();
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateGameState("Card Selection");

        // เปิด UI และสุ่มการ์ด 3 ใบ
        CardManager.Instance.ShowCardSelection();
        CardManager.Instance.RandomizeNewCards();
    }

    // ถูกเรียกโดย CardManager หลังผู้เล่นกดเลือกการ์ดแล้ว
    public IEnumerator StartNextWaveAfterCard()
    {
        // ... (Logic เดิม) ...
        waitingForCard = false;
        yield return new WaitForSeconds(0.2f);

        currentWaveIndex++; // ⭐ ขยับไปเวฟถัดไปใน List

        // ทุก ๆ 5 เวฟ เข้า BaseScene
        if (currentWave > 0 && currentWave % 5 == 0)
        {
            SaveRunData();

            if (SceneFader.Instance != null)
            {
                SceneFader.Instance.FadeToScene(baseSceneName);
            }
            else
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(openDoorClip, 0.95f, 1.05f);
                
                SceneManager.LoadScene(baseSceneName);
            }
        }
        else
        {
            StartNextWave();
        }

    }
    
    // ---------- Pause / Resume (ถ้าอยากใช้กด ESC) ----------
    public void PauseGame()
    {
        if (CurrentGameState == GameState.Paused) return;

        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f;

        if (UIManager.Instance != null)
            UIManager.Instance.TogglePausePanel(true);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(pauseClip);
        
        Debug.Log("Game Paused");
    }
    public void ResumeGame()
    {
        CurrentGameState = GameState.Running;
        Time.timeScale = 1f;

        if (UIManager.Instance != null)
            UIManager.Instance.TogglePausePanel(false);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(resumeClip);

        Debug.Log("Game Resumed");
    }
    //restart on death
    public void OnRestartPressed()
    {
        Time.timeScale = 1f;

        var scene = SceneManager.GetActiveScene();
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(scene.name);
        else
            SceneManager.LoadScene(scene.name);
    }
    
    // ---------- Save / Load RunData สำหรับข้ามฉาก ----------
    void SaveRunData()
    {
        RunData.HasData = true;

        RunData.currentWave = currentWave;
        RunData.currentWaveIndex = currentWaveIndex;

        if (PlayerHealth.Instance != null)
        {
            RunData.playerCurrentHp = PlayerHealth.Instance.CurrentHealth;
            RunData.playerMaxHp = PlayerHealth.Instance.MaxHealth;
        }

        if (baseCtrl != null)
        {
            RunData.baseCurrentHp = baseCtrl.CurrentHealth;
            RunData.baseMaxHp = baseCtrl.MaxHealth;
        }

        if (WeaponSystem.Instance != null)
        {
            RunData.bulletDamage = WeaponSystem.Instance.BulletDamage;
            RunData.bulletSpeed  = WeaponSystem.Instance.BulletSpeed;
            RunData.fireCooldown = WeaponSystem.Instance.FireCooldown;
            RunData.maxAmmo      = WeaponSystem.Instance.MaxAmmo;
            RunData.reloadTime   = WeaponSystem.Instance.ReloadTime;

            // ---- Special Effects ----
            RunData.vampireShot        = WeaponSystem.Instance.HasVampireShot;
            RunData.lifeStealAmount    = WeaponSystem.Instance.LifeStealAmount;
            RunData.adrenalineRush     = WeaponSystem.Instance.HasAdrenalineRush;
            RunData.adrenalineMultiplier = WeaponSystem.Instance.AdrenalineMultiplier;
            RunData.adrenalineDuration   = WeaponSystem.Instance.AdrenalineDuration;
        }
    }

    void LoadRunData()
    {
        if (!RunData.HasData) return;

        currentWave = RunData.currentWave;
        currentWaveIndex = RunData.currentWaveIndex;

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.SetHealthFromRun(
                RunData.playerCurrentHp,
                RunData.playerMaxHp
            );
        }

        if (baseCtrl != null)
        {
            baseCtrl.SetHealthFromRun(
                RunData.baseCurrentHp,
                RunData.baseMaxHp
            );
        }

        if (WeaponSystem.Instance != null)
        {
            WeaponSystem.Instance.SetWeaponFromRun(
                RunData.bulletDamage,
                RunData.bulletSpeed,
                RunData.fireCooldown,
                RunData.maxAmmo,
                RunData.reloadTime
            );

            // ---- Restore Special Effects ----
            if (RunData.vampireShot)
                WeaponSystem.Instance.EnableVampireShot(RunData.lifeStealAmount);

            if (RunData.adrenalineRush)
                WeaponSystem.Instance.EnableAdrenalineRush(
                    RunData.adrenalineMultiplier,
                    RunData.adrenalineDuration
                );
        }
    }


    // ---------- Game Over ----------
    public void OnPlayerDeath()
    {   
        if (CurrentGameState == GameState.CardSelection) 
        {
            CardManager.Instance.HideCardSelection(); // ปิดหน้าการ์ดก่อน
        }
        
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0f;
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateGameState("Game Over");
            UIManager.Instance.TogglePausePanel(false);   // เผื่อกำลัง pause อยู่
            UIManager.Instance.ToggleDeathPanel(true);    // ⭐ เปิด Death UI
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(gameOverClip);
        
        Debug.Log("Game over: Player Died");
    }
    public void OnBaseDestroyed()
    {
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0f;
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateGameState("Game Over");
            UIManager.Instance.TogglePausePanel(false);
            UIManager.Instance.ToggleDeathPanel(true);    // ⭐ เปิด Death UI
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(gameOverClip);
        
        Debug.Log("Game over: Base Destroyed");
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");
    }
    
    [ContextMenu("Generate 30 Waves (Mixed Enemies)")]
    public void GenerateWaves()
    {
        waves = new List<WaveData>();

        for (int i = 1; i <= 30; i++)
        {
            WaveData w = new WaveData();
            w.waveName = "Wave " + i;

            // --- 1. คำนวณจำนวนและ Spawn Rate ---
            // จำนวนเริ่มที่ 5 ตัว เพิ่มขึ้นเรื่อยๆ
            w.enemyCount = 5 + Mathf.RoundToInt(i * 1.5f);
            
            // ความเร็วในการออกมอน (ยิ่งเวฟสูง ยิ่งออกถี่)
            // เวฟ 1 = 1.8วิ, เวฟ 30 = 0.4วิ
            w.timeBetweenSpawns = Mathf.Clamp(1.8f - (i * 0.05f), 0.4f, 2.0f);

            // --- 2. กำหนดทิศทาง (Side) ---
            if (i % 5 == 0) 
            {
                w.side = SpawnSide.Both; // เวฟลงท้ายด้วย 5 (บอส/Swarm) มา 2 ทาง
                w.enemyCount += 5;       // แถมจำนวนให้อีก
            }
            else
            {
                // สลับ ซ้าย -> ขวา -> สุ่ม
                int sideRoll = i % 3;
                if (sideRoll == 1) w.side = SpawnSide.Left;
                else if (sideRoll == 2) w.side = SpawnSide.Right;
                else w.side = SpawnSide.Both;
            }

            // --- 3. เลือกมอนสเตอร์ที่จะใส่ในเวฟนี้ (Enemy Mix) ---
            List<GameObject> possibleEnemies = new List<GameObject>();

            // Wave 1-5: พื้นฐาน (Normal, Speed)
            if (prefabNormal) possibleEnemies.Add(prefabNormal);
            if (i >= 3 && prefabSpeed) possibleEnemies.Add(prefabSpeed);

            // Wave 6-10: เริ่มมีตัวกันและตัวบิน (Shield, Flying)
            if (i >= 6 && prefabShield) possibleEnemies.Add(prefabShield);
            if (i >= 8 && prefabFlying) possibleEnemies.Add(prefabFlying);

            // Wave 11-15: ตัวถึกและตัวระเบิด (Tank, Bomber)
            if (i >= 11 && prefabTank) possibleEnemies.Add(prefabTank);
            if (i >= 13 && prefabBomber) possibleEnemies.Add(prefabBomber);

            // Wave 20+: โอกาสเจอตัวโหดเยอะขึ้น (ใส่ Tank/Bomber เบิ้ลเข้าไปในสระสุ่ม)
            if (i >= 20)
            {
                if (prefabTank) possibleEnemies.Add(prefabTank);
                if (prefabBomber) possibleEnemies.Add(prefabBomber);
                if (prefabSpeed) possibleEnemies.Add(prefabSpeed); // Speed เยอะๆ ก็น่ารำคาญช่วงหลัง
            }

            // แปลง List เป็น Array ใส่ใน WaveData
            if (possibleEnemies.Count > 0)
            {
                w.specificEnemies = possibleEnemies.ToArray();
            }
            else
            {
                // กันเหนียว: ถ้าไม่ได้ใส่ Prefab อะไรมาเลย ให้ใช้ Normal หรือตัวเดิมที่มี
                if (enemyPrefab) w.specificEnemies = new GameObject[] { enemyPrefab };
            }

            waves.Add(w);
        }

        Debug.Log($"Generated 30 Waves with {waves.Count} stages! Good luck!");
    }
}

