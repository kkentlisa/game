using UnityEngine;

public class NoteInteraction : MonoBehaviour
{
    [HideInInspector] public NotesSpawner spawner;

    private SpriteRenderer sr;
    private Vector3 originalScale;
    private bool isMouseOver = false;
    private Camera mainCam;
    private AudioSource audioSource; 

    [Header("Настройки подсветки")]
    public GameObject outlineObject;
    public float scaleUpFactor = 1.1f;

    [Header("Настройки звука")]
    [Tooltip("Перетащи сюда аудио-файл шуршания бумаги")]
    public AudioClip collectSound;

    // Инициализация компонентов и начальных состояний
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        mainCam = Camera.main;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        if (outlineObject != null) outlineObject.SetActive(false);

        if (spawner == null)
            spawner = Object.FindFirstObjectByType<NotesSpawner>();
    }

    // Проверка наведения мыши и кликов для взаимодействия с запиской
    void Update()
    {
        CheckMouseHover();

        if (isMouseOver && Input.GetMouseButtonDown(0))
        {
            CollectNote();
        }
    }

    // Метод для проверки наведения мыши на записку и управления подсветкой
    void CheckMouseHover()
    {
        if (mainCam == null) return;

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        Collider2D hit = Physics2D.OverlapPoint(mousePos2D);

        // Если курсор находится над запиской, включаем подсветку, иначе отключаем
        if (hit != null && hit.gameObject == gameObject)
        {
            if (!isMouseOver) SetHighlight(true);
        }
        else
        {
            if (isMouseOver) SetHighlight(false);
        }
    }

    // Метод для управления подсветкой записки при наведении мыши
    void SetHighlight(bool highlight)
    {
        isMouseOver = highlight;

        if (outlineObject != null)
        {
            outlineObject.SetActive(highlight);
        }
        // Увеличиваем размер записки при наведении и возвращаем к оригинальному при отведении
        transform.localScale = highlight ? originalScale * scaleUpFactor : originalScale;

        if (sr != null)
        {
            sr.sortingOrder = highlight ? 100 : 5;
        }
    }

    // Метод для обработки сбора записки: воспроизведение звука, уведомление спавнера, отключение коллайдера и визуальных компонентов
    void CollectNote()
    {
        if (spawner == null) spawner = Object.FindFirstObjectByType<NotesSpawner>();

        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        if (SoundNoiseController.Instance != null)
        {
            SoundNoiseController.Instance.AddNoise();
        }

        if (spawner != null)
        {
            Debug.Log("Записка собрана мышкой!");
            spawner.OnNoteCollected();
        }

        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (outlineObject != null) outlineObject.SetActive(false);

        this.enabled = false;
    }
}