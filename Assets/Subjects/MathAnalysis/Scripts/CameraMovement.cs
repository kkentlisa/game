using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 8f;
    public SpriteRenderer backgroundSprite;

    [Header("Эффекты (Тряска - Grade 5)")]
    public float shakeIntensity = 0f;

    [Header("Эффекты (Покачивание - Сон)")]
    public float swayAmountX = 0f;
    public float swayAmountY = 0f;

    private float minX, maxX, minY, maxY;

    // Инициализация границ движения камеры на основе размера фона
    void Start()
    {
        if (backgroundSprite == null) return;
        CalculateBoundaries();
    }

    //Настройки камеры для ограничения движения в пределах фона
    void CalculateBoundaries()
    {
        Camera cam = GetComponent<Camera>();
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        minX = backgroundSprite.bounds.min.x + camWidth;
        maxX = backgroundSprite.bounds.max.x - camWidth;
        minY = backgroundSprite.bounds.min.y + camHeight;
        maxY = backgroundSprite.bounds.max.y - camHeight;

        Vector3 bgPos = backgroundSprite.bounds.center;
        if (minX > maxX) minX = maxX = bgPos.x;
        if (minY > maxY) minY = maxY = bgPos.y;
    }

    // Движение камеры и эффекты
    void LateUpdate()
    {
        if (backgroundSprite == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(moveX, moveY, 0).normalized;
        Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;

        Vector3 offset = Vector3.zero;

        if (shakeIntensity > 0)
        {
            offset += (Vector3)Random.insideUnitCircle * shakeIntensity * Time.deltaTime;
        }

        offset += new Vector3(swayAmountX, swayAmountY, 0);

        targetPosition += offset;

        float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    // Вызов этого метода извне для обновления границ, например, при изменении размера фона
    public void RefreshBoundaries()
    {
        CalculateBoundaries();
    }
}