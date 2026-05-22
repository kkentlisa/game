using UnityEngine;

public class SoundNoiseController : MonoBehaviour
{
    public static SoundNoiseController Instance;

    [Header("Настройки сетки квадратиков")]
    [Tooltip("Перетащи сюда объект Grid, внутри которого лежат все твои кубики")]
    public Transform gridTransform;

    [Header("Параметры шума")]
    [Tooltip("На сколько процентов заполняется шкала за одну записку (0.5f = ровно половина шкалы)")]
    public float noisePerNote = 0.5f;

    [Tooltip("Скорость, с которой шкала сползает вниз. Было 0.05f, сделали 0.02f (в 2.5 раза медленнее!)")]
    public float decreaseSpeed = 0.02f;

    private float currentNoise = 0f; // Текущий уровень шума от 0.0 до 1.0
    private GameObject[] allSegments; // Массив всех кубиков

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentNoise = 0f;

        // Собираем все квадратики из сетки в массив
        if (gridTransform != null)
        {
            allSegments = new GameObject[gridTransform.childCount];
            for (int i = 0; i < gridTransform.childCount; i++)
            {
                allSegments[i] = gridTransform.GetChild(i).gameObject;
                allSegments[i].SetActive(false); // Выключаем при старте
            }
        }
    }

    void Update()
    {
        // Шум медленно сползает (уменьшается) со временем
        if (currentNoise > 0f)
        {
            currentNoise -= decreaseSpeed * Time.deltaTime;
            currentNoise = Mathf.Clamp01(currentNoise); // Чтобы не ушло в минус

            UpdateEqualizerVisual();
        }
    }

    // Метод, который прибавляет шум при сборе записки
    public void AddNoise()
    {
        if (allSegments == null || allSegments.Length == 0) return;

        currentNoise += noisePerNote;
        currentNoise = Mathf.Clamp01(currentNoise);

        UpdateEqualizerVisual();

        Debug.Log($"Шум увеличился! Текущий уровень: {currentNoise * 100}%");

        // Если шкала заполнилась полностью — проигрыш
        if (currentNoise >= 0.99f)
        {
            GameOver();
        }
    }

    // Метод динамически включает/выключает кубики под текущий уровень шума
    void UpdateEqualizerVisual()
    {
        if (allSegments == null || allSegments.Length == 0) return;

        // Считаем, сколько именно кубиков должно гореть прямо сейчас
        int targetActiveCount = Mathf.RoundToInt(currentNoise * allSegments.Length);

        // Включаем кубики с инверсией (чтобы цвета шли от зеленого к красному снизу вверх)
        for (int i = 0; i < allSegments.Length; i++)
        {
            int invertedIndex = allSegments.Length - 1 - i;
            allSegments[invertedIndex].SetActive(i < targetActiveCount);
        }
    }

    void GameOver()
    {
        Debug.LogError("Шум на максимуме! Выход из игры.");

        // Если мы играем прямо в редакторе Unity (нажали Play)
#if UNITY_EDITOR
        string runtimePlatform = "Редактор Unity";
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Если игра запущена как скомпилированный билд (.exe файл)
        string runtimePlatform = "Скомпилированная игра";
        Application.Quit();
#endif
    }
}