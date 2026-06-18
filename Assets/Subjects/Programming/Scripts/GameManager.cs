using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum LevelDifficulty { Easy, Medium, Hard }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Настройки Сложности")]
    public LevelDifficulty baseDifficulty = LevelDifficulty.Easy;

    [Header("Параметры Здоровья")]
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("UI Элементы")]
    public Slider healthSlider;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Объекты на Сцене")]
    public GameObject firewallBarrier;

    [Header("Панели Терминалов")]
    public GameObject mainTerminalPanel;
    public GameObject selectionTaskPanel;
    public GameObject sortingTaskPanel;
    private GameObject activeTerminal;

    [Header("Прогресс Терминалов")]
    public int totalTerminals = 3;
    private int repairedTerminalsCount = 0;

    private float timeSinceLastDamage = 0f;

    [Header("Ссылки на Тексты UI Терминалов")]
    public TMPro.TMP_Text selectionQuestionText;
    public TMPro.TMP_Text selectionCorrectBtnText;
    public TMPro.TMP_Text selectionWrongBtnText;

    public TMPro.TMP_Text sortingQuestionText;

    private TerminalData currentTerminalData;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (LevelBridgeManager.instance != null)
        {
            int grade = LevelBridgeManager.instance.programmingGrade;
            if (grade == 2) baseDifficulty = LevelDifficulty.Easy;
            else if (grade == 3) baseDifficulty = LevelDifficulty.Medium;
            else if (grade == 4) baseDifficulty = LevelDifficulty.Hard;
        }

        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (firewallBarrier != null) firewallBarrier.SetActive(true);
        if (mainTerminalPanel != null) mainTerminalPanel.SetActive(false);
    }

    void Update()
    {
        timeSinceLastDamage += Time.deltaTime;
    }

    public (float damage, float aiTickRate) EvaluateDecisionTree()
    {
        float targetDamage = 20f;
        float targetTickRate = 1.5f;

        if (baseDifficulty == LevelDifficulty.Easy)
        {
            // Easy: урон минимальный, ловушки появляются редко
            targetDamage = 1f;
            targetTickRate = 3.5f;

            if (currentHealth < maxHealth * 0.3f)
            {
                targetDamage = 0.5f;
                targetTickRate = 4.0f;
            }
        }
        else if (baseDifficulty == LevelDifficulty.Medium)
        {
            // Medium: умеренный урон, средняя частота ловушек
            targetDamage = 2f;
            targetTickRate = 2.5f;

            if (currentHealth < maxHealth * 0.3f)
            {
                targetDamage = 1f;
                targetTickRate = 3.0f;
            }
            else if (currentHealth > maxHealth * 0.8f && timeSinceLastDamage > 20f)
            {
                targetDamage = 3f;
                targetTickRate = 2.0f;
            }
        }
        else
        {
            // Hard: ощутимый урон, но всё ещё проходимо
            targetDamage = 3f;
            targetTickRate = 2.0f;

            if (currentHealth > maxHealth * 0.7f && timeSinceLastDamage > 15f)
            {
                targetDamage = 5f;
                targetTickRate = 1.5f;
            }
        }

        return (targetDamage, targetTickRate);
    }

    public void TakeDamage(float damage = -1f)
    {
        if (damage < 0f)
        {
            var currentRules = EvaluateDecisionTree();
            damage = currentRules.damage;
        }

        currentHealth -= damage;
        timeSinceLastDamage = 0f;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    public void ReachExit()
    {
        Time.timeScale = 0f;
        if (victoryPanel != null) victoryPanel.SetActive(true);

        if (LevelBridgeManager.instance != null)
        {
            LevelBridgeManager.instance.finishLevel(true);
        }
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (LevelBridgeManager.instance != null)
        {
            LevelBridgeManager.instance.finishLevel(false);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OpenTerminalTask(GameObject terminal, TerminalData data)
    {
        activeTerminal = terminal;
        currentTerminalData = data;
        Time.timeScale = 0f;

        if (mainTerminalPanel != null) mainTerminalPanel.SetActive(true);

        TaskInfo currentTask = currentTerminalData.GetTaskForCurrentDifficulty();

        // Все уровни используют панель выбора вариантов
        if (selectionTaskPanel != null) selectionTaskPanel.SetActive(true);
        if (sortingTaskPanel != null) sortingTaskPanel.SetActive(false);

        if (selectionQuestionText != null) selectionQuestionText.text = currentTask.questionText;
        if (selectionCorrectBtnText != null) selectionCorrectBtnText.text = currentTask.correctAnswer;
        if (selectionWrongBtnText != null) selectionWrongBtnText.text = currentTask.wrongAnswer;
    }

    public void OnCorrectAnswer()
    {
        CloseTerminal(true);
    }

    public void OnWrongAnswer()
    {
        float penalty = baseDifficulty switch
        {
            LevelDifficulty.Easy => 5f,
            LevelDifficulty.Medium => 10f,
            LevelDifficulty.Hard => 15f,
            _ => 5f
        };
        TakeDamage(penalty);
        CloseTerminal(false);
    }

    public void CheckSortingAnswer()
    {
        TMPro.TMP_InputField inputField = sortingTaskPanel.GetComponentInChildren<TMPro.TMP_InputField>();
        TaskInfo currentTask = currentTerminalData.GetTaskForCurrentDifficulty();

        if (inputField != null && inputField.text == currentTask.correctAnswer)
        {
            inputField.text = "";
            CloseTerminal(true);
        }
        else
        {
            OnWrongAnswer();
        }
    }

    public void CloseTerminal(bool isSuccess)
    {
        if (mainTerminalPanel != null) mainTerminalPanel.SetActive(false);
        Time.timeScale = 1f;

        if (isSuccess && activeTerminal != null)
        {
            Destroy(activeTerminal);
            TerminalRepaired();
        }
    }

    public void TerminalRepaired()
    {
        repairedTerminalsCount++;

        if (repairedTerminalsCount >= totalTerminals)
        {
            if (firewallBarrier != null)
                firewallBarrier.SetActive(false);
        }
    }
}