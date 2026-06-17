using UnityEngine;
using TMPro;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [SerializeField] float gameDuration = 120f;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private GameObject rulesOverlay;
    [SerializeField] private GameObject gameplayUIContainer;

    private float timeRemaining;
    private bool isGameActive = false;
    private LevelManager levelManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        timeRemaining = gameDuration;

        levelManager = FindAnyObjectByType<LevelManager>();

        if (rulesOverlay != null)
        {
            rulesOverlay.SetActive(true);
        }

        if (gameplayUIContainer != null)
        {
            gameplayUIContainer.SetActive(false);
        }
        UpdateTimerUI();

        Time.timeScale = 0f;
        isGameActive = false;
    }

    private void Update()
    {
        if (!isGameActive) return;
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0;
            UpdateTimerUI();
            EndGameSession();
        }
    }

    public void StartLesson()
    {
        Time.timeScale = 1f;
        isGameActive = true;

        if (rulesOverlay != null)
        {
            rulesOverlay.SetActive(false);
        }
        if (gameplayUIContainer != null)
        {
            gameplayUIContainer.SetActive(true);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void EndGameSession()
    {
        isGameActive = false;
        Time.timeScale = 0f;

        if (ScoreManager.Instance == null) return;

        int playerRank = ScoreManager.Instance.GetPlayerRank();

        if (playerRank == 1)
        {
            if (LevelBridgeManager.instance != null)
            {
                LevelBridgeManager.instance.finishLevel(true);
            }
        }
        else
        {
            if (LevelBridgeManager.instance != null)
            {
                LevelBridgeManager.instance.finishLevel(false);
            }
        }

    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
