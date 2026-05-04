using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UIManager handles all UI screens: Main Menu, HUD, and Game Over.
/// Also manages the arrow indicator pointing toward the current target.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    [Header("HUD Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI taskText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI highScoreText;
    public Image timerFillImage;

    [Header("Game Over Elements")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI deliveriesText;
    public TextMeshProUGUI gameOverReasonText;
    public TextMeshProUGUI gameOverHighScoreText;

    [Header("Arrow Indicator")]
    public RectTransform arrowRect;
    public Transform playerTransform;
    public Camera mainCamera;
    public float arrowDistanceFromCenter = 120f;

    [Header("Animation")]
    public float taskTextDisplayTime = 2f;

    private float taskTextTimer;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        UpdateArrowIndicator();

        // Hide task text after a delay
        if (taskTextTimer > 0f)
        {
            taskTextTimer -= Time.deltaTime;
            if (taskTextTimer <= 0f && taskText != null)
                taskText.gameObject.SetActive(false);
        }
    }

    // -------------------- Screen Management --------------------

    public void ShowMainMenu(int highScore)
    {
        mainMenuPanel?.SetActive(true);
        hudPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }

    public void ShowHUD()
    {
        mainMenuPanel?.SetActive(false);
        hudPanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
    }

    public void ShowGameOver(int score, int deliveries, string reason)
    {
        mainMenuPanel?.SetActive(false);
        hudPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + score;
        if (deliveriesText != null)
            deliveriesText.text = "Deliveries: " + deliveries;
        if (gameOverReasonText != null)
            gameOverReasonText.text = reason;
    }

    // -------------------- HUD Updates --------------------

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void ShowTask(string message)
    {
        if (taskText != null)
        {
            taskText.text = message;
            taskText.gameObject.SetActive(true);
            taskTextTimer = taskTextDisplayTime;
        }
    }

    public void UpdateTimer(float remaining, float maxTime)
    {
        if (timerText != null)
            timerText.text = Mathf.Ceil(remaining).ToString("0") + "s";

        if (timerFillImage != null)
        {
            float ratio = Mathf.Clamp01(remaining / maxTime);
            timerFillImage.fillAmount = ratio;
            // Change color based on urgency
            timerFillImage.color = ratio > 0.3f ? Color.green : Color.red;
        }
    }

    // -------------------- Arrow Indicator --------------------

    void UpdateArrowIndicator()
    {
        if (arrowRect == null || playerTransform == null || mainCamera == null)
            return;

        // Find current target
        Vector3? targetPos = GetCurrentTargetPosition();
        if (!targetPos.HasValue)
        {
            arrowRect.gameObject.SetActive(false);
            return;
        }

        Vector3 target = targetPos.Value;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target);
        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

        // If target is on screen, hide arrow
        if (screenPos.z > 0f && screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height)
        {
            arrowRect.gameObject.SetActive(false);
            return;
        }

        arrowRect.gameObject.SetActive(true);

        // Calculate direction from center to target
        Vector3 dir = screenPos - center;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowRect.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // Position arrow at edge of screen
        Vector3 edgePos = center + dir.normalized * arrowDistanceFromCenter;
        arrowRect.position = edgePos;
    }

    Vector3? GetCurrentTargetPosition()
    {
        DeliveryManager dm = GameManager.Instance?.deliveryManager;
        if (dm == null) return null;
        Transform target = dm.GetCurrentTarget();
        return target != null ? target.position : (Vector3?)null;
    }

    // -------------------- Button Handlers --------------------

    public void OnStartButtonClicked()
    {
        GameManager.Instance?.StartGame();
    }

    public void OnRestartButtonClicked()
    {
        GameManager.Instance?.RestartGame();
    }

    public void OnMenuButtonClicked()
    {
        GameManager.Instance?.ReturnToMenu();
    }

    public void OnQuitButtonClicked()
    {
        GameManager.Instance?.QuitGame();
    }
}
