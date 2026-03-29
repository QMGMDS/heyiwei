using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("游戏状态")]
    public bool isGameRunning = true;
    public bool isPaused = false;
    
    [Header("玩家引用")]
    public GameObject player;
    public Transform playerSpawnPoint;
    
    [Header("游戏设置")]
    public float gameSpeed = 1f;
    public float speedIncreaseRate = 0.02f;
    public float maxGameSpeed = 2f;
    
    [Header("UI")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    
    private float currentSpeed = 1f;
    private float distanceTraveled = 0f;
    private Vector3 lastPlayerPosition;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeGame();
    }
    
    void Update()
    {
        if (!isGameRunning) return;
        
        HandlePause();
        
        if (!isPaused)
        {
            UpdateGameSpeed();
            UpdateDistance();
        }
    }
    
    void InitializeGame()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
        }
        
        currentSpeed = gameSpeed;
        Time.timeScale = 1f;
        
        // 隐藏菜单
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
    }
    
    void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    void UpdateGameSpeed()
    {
        // 逐渐增加游戏速度
        currentSpeed += speedIncreaseRate * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxGameSpeed);
    }
    
    void UpdateDistance()
    {
        if (player != null)
        {
            distanceTraveled = player.transform.position.z;
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }
        
        // 控制鼠标光标
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    public void GameOver()
    {
        isGameRunning = false;
        Time.timeScale = 0f;
        
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void RespawnPlayer()
    {
        if (player != null && playerSpawnPoint != null)
        {
            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;
            
            // 重置玩家速度
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    // 获取当前游戏速度
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    // 获取已跑距离
    public float GetDistanceTraveled()
    {
        return distanceTraveled;
    }
    
    // 增加分数
    public void AddScore(int points)
    {
        // 这里可以实现分数系统
    }
}
