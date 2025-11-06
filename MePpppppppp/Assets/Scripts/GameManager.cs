using System;
using UnityEngine;
using System.Collections;

public enum GameState
{
    Running,
    Paused,
    CardSelection,
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
    
    public GameState CurrentGameState { get; private set; } = GameState.Running;
    public BaseController BaseCtrl => baseCtrl;
    
    public event Action<GameState> OnStateChanged;
    
    void Awake()
    {
        Instance = this;
        if(GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleGameStateChanged;
    }
    
    void Start()
    {
        StartCoroutine(StartWaveRoutine());
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeGameStage(GameState.CardSelection);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentGameState == GameState.Running)
                pauseGame();
            else if (CurrentGameState == GameState.Paused)
                ResumeGame();
        }
    }
    
    //State Machine
    
    public void ChangeGameStage(GameState newGameState)
    {
        CurrentGameState = newGameState;
        OnStateChanged?.Invoke(newGameState);
    }
    
    private void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Running:
                CardManager.Instance.HideCardSelection();
                break;

            case GameState.CardSelection:
                CardManager.Instance.ShowCardSelection();
                CardManager.Instance.RandomizeNewCards();  
                break;

            case GameState.Paused:
                break;

            case GameState.GameOver:
                break;
        }
    }
    
    //Wave System
    
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
            Transform spawnPoint = UnityEngine.Random.value < 0.5f ? spawnLeft : spawnRight;
            GameObject e = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            RegisterEnemySpawn();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
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
            Debug.Log("Wave cleared!");
            StartCoroutine(StartWaveRoutine());
        }
    }
    
    //Game Control
    
    public void pauseGame()
    {
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
    
    public void OnPlayerDeath()
    {
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0f;
        Debug.Log("Game over: Player Died");
        // TODO: เรียก UI GameOver ถ้ามี
    }
    
    public void OnBaseDestroyed()
    {
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0f;
        Debug.Log("Game over: Base Destroyed");
        // TODO: เรียก UI GameOver ถ้ามี
    }
}
