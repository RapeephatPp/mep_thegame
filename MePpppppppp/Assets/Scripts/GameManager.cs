using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Running, Paused, CardSelection, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Base Settings")]
    [SerializeField] private BaseController baseCtrl;

    [Header("Wave System")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnLeft;
    [SerializeField] private Transform spawnRight;
    [SerializeField] private float timeBetweenSpawns = 1.2f;
    [SerializeField] private int baseEnemyCount = 5;
    
    
    public GameState CurrentGameState { get; private set; } = GameState.Running;
    public BaseController BaseCtrl => baseCtrl;

    int currentWave = 0;
    public int CurrentWave => currentWave;
    int aliveEnemies = 0;
    int enemiesToSpawn = 0;

    bool isSpawning = false;        // กำลังปล่อยมอนอยู่ไหม
    bool waitingForCard = false;    // กำลังรอให้ผู้เล่นเลือกการ์ดหรือไม่
    Coroutine waveRoutine;

    void Awake() => Instance = this;

    void Start()
    {
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

        currentWave++;
        enemiesToSpawn = baseEnemyCount + (currentWave - 1) * 2;

        Debug.Log($"--- Wave {currentWave} ---");
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateWave(currentWave);
            UIManager.Instance.UpdateGameState("Running");
        }

        waveRoutine = StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false;
        TryFinishWave(); // อาจจบได้เลยถ้าไม่มีศัตรูเหลือ
    }

    void SpawnEnemy()
    {
        Transform p = (Random.value < 0.5f) ? spawnLeft : spawnRight;
        Instantiate(enemyPrefab, p.position, Quaternion.identity);
        aliveEnemies++;
    }

    // เรียกจาก EnemyController.Die()
    public void RegisterEnemyDeath()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        TryFinishWave();
    }

    void TryFinishWave()
    {
        // จบเวฟได้เมื่อ: ไม่ได้สปอว์นแล้ว และ ศัตรูเหลือ 0 และ เกมอยู่สถานะ Running
        if (!isSpawning && aliveEnemies <= 0 && CurrentGameState == GameState.Running && !waitingForCard)
        {
            Debug.Log("Wave cleared!");
            ShowCardSelection();
        }
    }

    // ---------- การ์ด ----------
    void ShowCardSelection()
    {
        waitingForCard = true;
        CurrentGameState = GameState.CardSelection;
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateGameState("Card Selection");

        // เปิด UI และสุ่มการ์ด 3 ใบ
        CardManager.Instance.ShowCardSelection();
        CardManager.Instance.RandomizeNewCards();
    }

    // ถูกเรียกโดย CardManager หลังผู้เล่นกดเลือกการ์ดแล้ว
    public IEnumerator StartNextWaveAfterCard()
    {
        // กันกดซ้ำหรือโดนเรียกซ้อน
        if (!waitingForCard) yield break;

        waitingForCard = false;
        yield return new WaitForSeconds(0.2f); // กันเฟรมชน

        StartNextWave();
    }

    // ---------- Pause / Resume (ถ้าอยากใช้กด ESC) ----------
    public void PauseGame()
    {
        if (CurrentGameState == GameState.Paused) return;

        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f;

        if (UIManager.Instance != null)
            UIManager.Instance.TogglePausePanel(true);

        Debug.Log("Game Paused");
    }
    public void ResumeGame()
    {
        CurrentGameState = GameState.Running;
        Time.timeScale = 1f;

        if (UIManager.Instance != null)
            UIManager.Instance.TogglePausePanel(false);

        Debug.Log("Game Resumed");
    }

    // ---------- Game Over ----------
    public void OnPlayerDeath()
    {
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0f;
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateGameState("Game Over");
        
        Debug.Log("Game over: Player Died");
    }
    public void OnBaseDestroyed()
    {
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0f;
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateGameState("Game Over");
        
        Debug.Log("Game over: Base Destroyed");
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}