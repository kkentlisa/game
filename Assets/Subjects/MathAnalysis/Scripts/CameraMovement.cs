using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 8f;
    public SpriteRenderer backgroundSprite;
    [Header("Эффекты искажения")]
    public float shakeIntensity = 0f;

    private float minX, maxX, minY, maxY;

    void Start()
    {
        if (backgroundSprite == null)
        {
            return;
        }

        CalculateBoundaries();
    }

    void CalculateBoundaries()
    {
        Camera cam = GetComponent<Camera>();
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float bgWidth = backgroundSprite.bounds.size.x;
        float bgHeight = backgroundSprite.bounds.size.y;
        Vector3 bgPos = backgroundSprite.bounds.center;

        minX = backgroundSprite.bounds.min.x + camWidth;
        maxX = backgroundSprite.bounds.max.x - camWidth;
        minY = backgroundSprite.bounds.min.y + camHeight;
        maxY = backgroundSprite.bounds.max.y - camHeight;

        if (minX > maxX) minX = maxX = bgPos.x;
        if (minY > maxY) minY = maxY = bgPos.y;
    }

    void LateUpdate() 
    {
        if (backgroundSprite == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(moveX, moveY, 0).normalized;
        Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;

        float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        if (shakeIntensity > 0)
        {
            transform.position += (Vector3)Random.insideUnitCircle * shakeIntensity * Time.deltaTime;
        }
    }

    public void RefreshBoundaries()
    {
        CalculateBoundaries();
    }
}
