using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelBridgeManager : MonoBehaviour
{
    public static LevelBridgeManager instance;

    public int mathGrade = 2;
    public int programmingGrade = 2;
    public int economyGrade = 2;

    public string playerName = "";
    [Range(0f, 1f)] public float distortionValue = 0f;

    [HideInInspector] public string currentActiveSubject;
    public bool isAuthorized = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "HubScene")
        {
            BindButtons();
        }
    }

    public void BindButtons()
    {
        var mathBtn = GameObject.Find("MathButton")?.GetComponent<UnityEngine.UI.Button>();
        if (mathBtn != null)
        {
            mathBtn.onClick.RemoveAllListeners();
            mathBtn.onClick.AddListener(() => clickSubject("Math"));
        }

        var progBtn = GameObject.Find("ProgrammingButton")?.GetComponent<UnityEngine.UI.Button>();
        if (progBtn != null)
        {
            progBtn.onClick.RemoveAllListeners();
            progBtn.onClick.AddListener(() => clickSubject("Programming"));
        }

        var econBtn = GameObject.Find("EconomyButton")?.GetComponent<UnityEngine.UI.Button>();
        if (econBtn != null)
        {
            econBtn.onClick.RemoveAllListeners();
            econBtn.onClick.AddListener(() => clickSubject("Economy"));
        }

        var closeBtn = GameObject.Find("Button")?.GetComponent<UnityEngine.UI.Button>();
        if (closeBtn != null)
        {
            closeBtn.onClick.RemoveAllListeners();
            var endManager = FindObjectOfType<EndManager>();
            if (endManager != null)
            {
                closeBtn.onClick.AddListener(endManager.CloseGradebookForever);
                Debug.Log("CloseForeverButton прив€зана");
            }
            else
            {
                Debug.LogError("EndManager не найден в сцене!");
            }
        }
        else
        {
            Debug.LogWarning(" нопка CloseForeverButton не найдена!");
        }
    }

    public void clickSubject(string subjectName)
    {
        currentActiveSubject = subjectName;
        if (subjectName == "Math") SceneManager.LoadScene("MathAnalysisGameScene");
        else if (subjectName == "Programming") SceneManager.LoadScene("ProgrammingScene");
        else if (subjectName == "Economy") SceneManager.LoadScene("EconimicGameScene");
    }

    public void finishLevel(bool isWin)
    {
        Time.timeScale = 1f;

        if (isWin)
        {
            if (currentActiveSubject == "Math" && mathGrade < 5) mathGrade++;
            if (currentActiveSubject == "Programming" && programmingGrade < 5) programmingGrade++;
            if (currentActiveSubject == "Economy" && economyGrade < 5) economyGrade++;

            distortionValue += 0.25f;
            distortionValue = Mathf.Clamp01(distortionValue);

        }

        SceneManager.LoadScene("HubScene");
    }

    public float GetAverageGrade()
    {
        return (mathGrade + programmingGrade + economyGrade) / 3f;
    }
}