using UnityEngine;
using TMPro; // Обязательно для работы с TextMeshPro

public class EyeContactController : MonoBehaviour
{
    [Header("Ссылки на объекты игры")]
    [Tooltip("Перетащи сюда игровой объект Учителя (Пола)")]
    public Transform teacherTransform;

    [Header("UI Тексты (Раздельные)")]
    [Tooltip("Сюда перетащи текст с НАДПИСЬЮ 'Долгий зрительный контакт'")]
    public TextMeshProUGUI warningText;

    [Tooltip("Сюда перетащи второй текст, который создан ТИПA ТAЙМЕРА")]
    public TextMeshProUGUI timerText;

    [Header("Настройки времени")]
    [Tooltip("Через сколько накопленных секунд надпись начнет мигать (4 секунды)")]
    public float timeToShowText = 4f;

    [Tooltip("Общее время контакта до проигрыша (6 секунд)")]
    public float maxContactTime = 6f;

    [Header("Скорость остывания")]
    [Tooltip("Во сколько раз медленнее таймер спадает обратно, когда взгляд уведен (0.7f = чуть медленнее)")]
    public float recoverySpeedMultiplier = 0.7f;

    private SpriteRenderer teacherSpriteRenderer;
    private Camera mainCamera;
    private float contactTimer = 0f;
    private bool isTextVisible = false;
    private float blinkTimer = 0f;

    [Tooltip("Скорость мигания надписи")]
    private float blinkSpeed = 0.25f;

    void Start()
    {
        mainCamera = Camera.main;

        if (teacherTransform != null)
        {
            teacherSpriteRenderer = teacherTransform.GetComponent<SpriteRenderer>();
        }

        if (warningText != null) warningText.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (teacherTransform == null || teacherSpriteRenderer == null || mainCamera == null) return;

        if (IsTargetVisibleByCamera(teacherSpriteRenderer))
        {
            contactTimer += Time.deltaTime;
            if (contactTimer > maxContactTime) contactTimer = maxContactTime;

            if (contactTimer > 0f)
            {
                UpdateTimerDigits();
            }

            if (contactTimer >= timeToShowText)
            {
                HandleTextBlinking();
            }

            if (contactTimer >= maxContactTime)
            {
                TriggerEyeContactGameOver();
            }
        }
        else
        {
            if (contactTimer > 0f)
            {
                contactTimer -= Time.deltaTime * recoverySpeedMultiplier;
                if (contactTimer < 0f) contactTimer = 0f;

                UpdateTimerDigits();

                if (contactTimer < timeToShowText && warningText != null && warningText.gameObject.activeSelf)
                {
                    warningText.gameObject.SetActive(false);
                }
            }
            else
            {
                if (warningText != null && warningText.gameObject.activeSelf) warningText.gameObject.SetActive(false);
                if (timerText != null && timerText.gameObject.activeSelf) timerText.gameObject.SetActive(false);
            }
        }
    }

    void UpdateTimerDigits()
    {
        if (timerText == null) return;

        if (!timerText.gameObject.activeSelf)
        {
            timerText.gameObject.SetActive(true);
        }

        int minutes = Mathf.FloorToInt(contactTimer / 60f);
        int seconds = Mathf.FloorToInt(contactTimer % 60f);
        int fraction = Mathf.FloorToInt((contactTimer * 100f) % 100f);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
    }

    void HandleTextBlinking()
    {
        if (warningText == null) return;

        if (!warningText.gameObject.activeSelf)
        {
            warningText.gameObject.SetActive(true);
            isTextVisible = true;
            warningText.enabled = true;
            blinkTimer = 0f;
        }

        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkSpeed)
        {
            isTextVisible = !isTextVisible;
            warningText.enabled = isTextVisible;
            blinkTimer = 0f;
        }
    }

    bool IsTargetVisibleByCamera(SpriteRenderer renderer)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    void TriggerEyeContactGameOver()
    {
        Debug.LogError("6 секунд истекли! Выход.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }
}