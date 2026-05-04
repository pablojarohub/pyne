using UnityEngine;

/// <summary>
/// GameManager is the central hub that connects all systems.
/// Handles game state (Menu, Playing, GameOver) and high-level events.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Systems")]
    public PlayerController player;
    public DeliveryManager deliveryManager;
    public UIManager uiManager;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip pickupSound;
    public AudioClip deliverySound;
    public AudioClip gameOverSound;

    public enum GameState { Menu, Playing, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Menu;

    private int highScore;
    private const string HighScoreKey = "DeliveryHighScore";

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    void Start()
    {
        // Subscribe to delivery events
        if (deliveryManager != null)
        {
            deliveryManager.OnOrderPickedUp += HandleOrderPickedUp;
            deliveryManager.OnOrderDelivered += HandleOrderDelivered;
            deliveryManager.OnGameOver += HandleGameOver;
        }

        ShowMainMenu();
    }

    void OnDestroy()
    {
        if (deliveryManager != null)
        {
            deliveryManager.OnOrderPickedUp -= HandleOrderPickedUp;
            deliveryManager.OnOrderDelivered -= HandleOrderDelivered;
            deliveryManager.OnGameOver -= HandleGameOver;
        }
    }

    // -------------------- State Management --------------------

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        uiManager?.ShowHUD();
        player?.ResetPlayer();
        deliveryManager?.ResetDeliveries();

        if (musicSource != null && !musicSource.isPlaying)
            musicSource.Play();
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void ReturnToMenu()
    {
        CurrentState = GameState.Menu;
        uiManager?.ShowMainMenu(highScore);
        player?.StopBike();
    }

    void ShowMainMenu()
    {
        CurrentState = GameState.Menu;
        uiManager?.ShowMainMenu(highScore);
    }

    void HandleGameOver()
    {
        CurrentState = GameState.GameOver;
        int finalScore = deliveryManager != null ? deliveryManager.GetScore() : 0;
        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }
        PlaySFX(gameOverSound);
    }

    // -------------------- Event Handlers --------------------

    void HandleOrderPickedUp()
    {
        player?.PickupOrder();
        PlaySFX(pickupSound);
    }

    void HandleOrderDelivered(int score)
    {
        player?.DeliverOrder();
        PlaySFX(deliverySound);
    }

    // -------------------- Audio Helpers --------------------

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // -------------------- Utility --------------------

    /// <summary>
    /// Quits the application (works in builds and editor).
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
