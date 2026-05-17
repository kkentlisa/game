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

    private float timeSinceLastDamage = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (firewallBarrier != null) firewallBarrier.SetActive(true); 
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
            targetDamage = 15f;
            targetTickRate = 2.0f; 

            if (currentHealth < 30f) 
            {
                targetDamage = 10f;    
                targetTickRate = 2.5f; 
            }
        }
        else if (baseDifficulty == LevelDifficulty.Medium)
        {
            targetDamage = 25f;
            targetTickRate = 1.5f;

            if (currentHealth < 25f)
            {
                targetDamage = 15f; 
                targetTickRate = 1.8f;
            }
            else if (currentHealth > 80f && timeSinceLastDamage > 15f)
            {
                targetDamage = 30f;
                targetTickRate = 1.2f;
            }
        }
        else 
        {
            targetDamage = 40f;
            targetTickRate = 1.0f;

            if (currentHealth > 70f && timeSinceLastDamage > 10f)
            {
                targetDamage = 50f;     
                targetTickRate = 0.7f;  
            }
        }

        return (targetDamage, targetTickRate);
    }

    public void TakeDamage()
    {
        var currentRules = EvaluateDecisionTree();
        
        currentHealth -= currentRules.damage;
        timeSinceLastDamage = 0f; 

        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth; 
        }
        
        Debug.Log($"Получен урон: {currentRules.damage}. Текущее здоровье: {currentHealth}");

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    public void ReachExit()
    {
        Time.timeScale = 0f;
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}