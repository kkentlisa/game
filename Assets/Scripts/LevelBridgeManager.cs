using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelBridgeManager : MonoBehaviour
{
    public static LevelBridgeManager instance;

    public int mathGrade = 2;
    public int programmingGrade = 2;
    public int economyGrade = 2;

    public string playerName = "";
    [Range(0f, 1f)] public float distortionValue = 0f;

    [HideInInspector]
    public string currentActiveSubject;

    public bool isAuthorized = false;

    void Start()
    {
        BindButtons();
    }

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
            updateUiDisplays();
            BindButtons();
        }
    }

    void BindButtons()
    {
        var mathBtn = GameObject.Find("MathButton")?.GetComponent<UnityEngine.UI.Button>();
        if (mathBtn != null)
        {
            mathBtn.onClick.RemoveAllListeners();
            mathBtn.onClick.AddListener(() => clickSubject("Math"));
            Debug.Log("MathButton привязана");
        }

        var progBtn = GameObject.Find("ProgrammingButton")?.GetComponent<UnityEngine.UI.Button>();
        if (progBtn != null)
        {
            progBtn.onClick.RemoveAllListeners();
            progBtn.onClick.AddListener(() => clickSubject("Programming"));
            Debug.Log("ProgrammingButton привязана");
        }

        var econBtn = GameObject.Find("EconomyButton")?.GetComponent<UnityEngine.UI.Button>();
        if (econBtn != null)
        {
            econBtn.onClick.RemoveAllListeners();
            econBtn.onClick.AddListener(() => clickSubject("Economy"));
            Debug.Log("EconomyButton привязана");
        }
    }

    public void updateUiDisplays()
    {
        var mathText = GameObject.Find("MathGradeText")?.GetComponent<TextMeshProUGUI>();
        var progText = GameObject.Find("ProgrammingGradeText")?.GetComponent<TextMeshProUGUI>();
        var econText = GameObject.Find("EconomyGradeText")?.GetComponent<TextMeshProUGUI>();

        if (mathText != null) mathText.text = mathGrade.ToString();
        if (progText != null) progText.text = programmingGrade.ToString();
        if (econText != null) econText.text = economyGrade.ToString();
    }

    public void clickSubject(string subjectName)
    {
        currentActiveSubject = subjectName;

        if (subjectName == "Math") SceneManager.LoadScene("MathAnalysisGameScene");
        else if (subjectName == "Programming") SceneManager.LoadScene("ProgrammingGameScene");
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

            updateUiDisplays();
        }

        SceneManager.LoadScene("HubScene");
    }
    public float GetAverageGrade()
    {
        return (mathGrade + programmingGrade + economyGrade) / 3f;
    }
}