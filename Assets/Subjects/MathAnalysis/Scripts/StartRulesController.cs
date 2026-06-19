using UnityEngine;
using UnityEngine.SceneManagement;

public class StartRulesController : MonoBehaviour
{
    [Header("ќбща€ панель правил")]
    [Tooltip(" орневой объект StartRulesPanel Ч картинка с правилами")]
    public GameObject startRulesPanel;

    [Header(" нопки внутри StartRulesPanel")]
    [Tooltip(" нопка 'Ќј„ј“№ »√–”' Ч видна только в самом начале")]
    public GameObject startButton;

    [Tooltip(" нопка 'ѕ–ќƒќЋ∆»“№ »√–”' Ч видна только во врем€ паузы")]
    public GameObject resumeButton;

    [Tooltip(" нопка 'Ќј ѕ≈–≈—ƒј„”' Ч видна только во врем€ паузы")]
    public GameObject retakeButton;

    [Header(" нопка паузы (отдельный объект ¬Ќ≈ StartRulesPanel, например пр€мо в Canvas)")]
    [Tooltip(" нопка с иконкой 'II' Ч видна только во врем€ игры, открывает панель правил как паузу")]
    public GameObject pauseButton;

    [Header("—цена дл€ перезапуска")]
    [Tooltip("»м€ сцены дл€ 'Ќа пересдачу', если LevelBridgeManager не используетс€")]
    public string hubSceneName = "HubScene";

    void Start()
    {
        ShowAsStartScreen();
        Time.timeScale = 0f;
    }

    void ShowAsStartScreen()
    {
        if (startRulesPanel != null) startRulesPanel.SetActive(true);
        if (startButton != null) startButton.SetActive(true);
        if (resumeButton != null) resumeButton.SetActive(false);
        if (retakeButton != null) retakeButton.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(false);
    }

    void ShowAsPauseScreen()
    {
        if (startRulesPanel != null) startRulesPanel.SetActive(true);
        if (startButton != null) startButton.SetActive(false);
        if (resumeButton != null) resumeButton.SetActive(true);
        if (retakeButton != null) retakeButton.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);
    }

    public void OnStartGameButton()
    {
        if (startRulesPanel != null) startRulesPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);

        Time.timeScale = 1f;
    }

    public void OnPauseButton()
    {
        ShowAsPauseScreen();
        Time.timeScale = 0f;
    }

    public void OnResumeButton()
    {
        if (startRulesPanel != null) startRulesPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);

        Time.timeScale = 1f;
    }

    public void OnRetakeButton()
    {
        Time.timeScale = 1f;

        if (LevelBridgeManager.instance != null)
        {
            LevelBridgeManager.instance.finishLevel(false);
        }
        else
        {
            SceneManager.LoadScene(hubSceneName);
        }
    }
}