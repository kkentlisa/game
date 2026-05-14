using UnityEngine;

public class NoteInteraction : MonoBehaviour
{
    [HideInInspector] public NotesSpawner spawner;

    private SpriteRenderer sr;
    private Vector3 originalScale;
    private bool isMouseOver = false;
    private Camera mainCam;

    [Header("Настройки подсветки")]
    public GameObject outlineObject; 
    public float scaleUpFactor = 1.1f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        mainCam = Camera.main;

        if (outlineObject != null) outlineObject.SetActive(false);

        if (spawner == null)
            spawner = Object.FindFirstObjectByType<NotesSpawner>();
    }

    void Update()
    {
        CheckMouseHover();

        if (isMouseOver && Input.GetMouseButtonDown(0))
        {
            CollectNote();
        }
    }

    void CheckMouseHover()
    {
        if (mainCam == null) return;

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);


        Collider2D hit = Physics2D.OverlapPoint(mousePos2D);

        if (hit != null && hit.gameObject == gameObject)
        {
            if (!isMouseOver) SetHighlight(true);
        }
        else
        {
            if (isMouseOver) SetHighlight(false);
        }
    }

    void SetHighlight(bool highlight)
    {
        isMouseOver = highlight;

        if (outlineObject != null)
        {
            outlineObject.SetActive(highlight);
        }

        transform.localScale = highlight ? originalScale * scaleUpFactor : originalScale;

        if (sr != null)
        {
            sr.sortingOrder = highlight ? 100 : 5;
        }
    }

    void CollectNote()
    {
        if (spawner == null) spawner = Object.FindFirstObjectByType<NotesSpawner>();

        if (spawner != null)
        {
            Debug.Log("Записка собрана мышкой!");
            spawner.OnNoteCollected();
            gameObject.SetActive(false);
        }
    }
}