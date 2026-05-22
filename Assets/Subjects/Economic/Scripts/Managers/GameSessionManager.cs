using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [SerializeField] float gameDuration = 120f;
    [SerializeField] private TextMeshProUGUI timerText;

    private float timeRemaining;
    private bool isGameActive = true;
    private LevelManager levelManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        timeRemaining = gameDuration;

        levelManager = FindAnyObjectByType<LevelManager>();
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

        if (ScoreManager.Instance == null) return;

        int playerRank = ScoreManager.Instance.GetPlayerRank();

        if (playerRank == 1)
        {
            Debug.Log("<color=green>œŒ¡≈ƒ¿!</color>");
            if (LevelBridgeManager.instance != null)
            {
                LevelBridgeManager.instance.finishLevel(true);
            }
        }
        else
        {
            Debug.Log($"<color=red>œŒ–¿∆≈Õ»≈!</color> »„ÓÍ Á‡ÌˇÎ {playerRank} ÏÂÒÚÓ");
            if (LevelBridgeManager.instance != null)
            {
                LevelBridgeManager.instance.finishLevel(false);
            }
        }

        Time.timeScale = 0f;
    }

    private void UpdateDifficultyOnWin()
    {
        if (levelManager == null) return;

        int currentDifficulty = levelManager.difficulty;

        if (currentDifficulty == 3)
        {
            levelManager.difficulty = 4;
        }
        else if (currentDifficulty == 4)
        {
            levelManager.difficulty = 5;
        }

    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
