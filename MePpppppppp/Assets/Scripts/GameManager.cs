using UnityEngine;
using System.Collections;

public enum GameState
{
    Running,
    Paused,
    Upgrading,
    GameOver
}

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
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private int baseEnemyCount = 5;
   
    private int currentWave = 0;
    private int aliveEnemies = 0;
    private bool waitingForUpgrade = false;
    
    public GameState CurrentGameState { get; private set; } = GameState.Running;
    public BaseController BaseCtrl => baseCtrl;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartWaveRoutine());
    }

    public void RegisterEnemySpawn()
    {
        aliveEnemies++;
    }

    public void RegisterEnemyDeadth()
    {
        aliveEnemies--;
        if (aliveEnemies <= 0 && CurrentGameState == GameState.Running)
        {
            Debug.Log("Wave cleared > Show upgrade UI");
            StartCoroutine(HandleWaveCleared());
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentGameState == GameState.Running)
                pauseGame();
            else if (CurrentGameState == GameState.Paused)
                ResumeGame();
        }
    }

    public void pauseGame()
    {
        if (CurrentGameState == GameState.Paused) return;
        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f;
        Debug.Log("Game paused");
    }

    public void ResumeGame()
    {
        CurrentGameState = GameState.Running;
        Time.timeScale = 1f;
        Debug.Log("Game resumed");
    }
    
    
    private IEnumerator StartWaveRoutine()
    {
        yield return new WaitForSeconds(1f);
        currentWave++;
        Debug.Log($"--- Wave {currentWave} ---");
        StartCoroutine(SpawnEnemies(currentWave));
    }

    private IEnumerator SpawnEnemies(int wave)
    {
        int totalEnemies = baseEnemyCount + wave * 2;
        for (int i = 0; i < totalEnemies; i++)
        {
            Transform spawnPoint = Random.value < 0.5f ? spawnLeft : spawnRight;
            GameObject e = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            RegisterEnemySpawn();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private IEnumerator HandleWaveCleared()
    {
        CurrentGameState = GameState.Upgrading;
        Time.timeScale = 0f;
        waitingForUpgrade = true;
        
        UpgradeUIManager.Instance.ShowUpgradeOptions();
        
        yield return new WaitUntil(() => waitingForUpgrade == false);
        
        yield return new WaitForSeconds(timeBetweenWaves);
        Time.timeScale = 1f;
        CurrentGameState = GameState.Running;
        StartCoroutine(StartWaveRoutine());
    }

    private void ShowUpgradeMenu()
    {
        Debug.Log("✅ Wave cleared — Showing Upgrade UI");
        CurrentGameState = GameState.Upgrading;
        Time.timeScale = 0f;
        waitingForUpgrade = true;
        UpgradeUIManager.Instance.ShowUpgradeOptions();
    }
    
    public void OnUpgradeChosen()
    {
        waitingForUpgrade = false;
        ResumeGame();
    }

    public void OnEnemyKilled()
    {
        RegisterEnemyDeadth();
    }

    void SpawnEnemy()
    {
        Transform spawnPoint = Random.value < 0.5f ? spawnLeft : spawnRight;
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }
    public void OnPlayerDeath()
    {
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0f;
        Debug.Log("Game over: Player Died");
    }

    public void OnBaseDestroyed()
    {
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0f;
        Debug.Log("Game over: Base Destroyed");
        // TODO: เรียก UI GameOver ถ้ามี
    }
    
}
