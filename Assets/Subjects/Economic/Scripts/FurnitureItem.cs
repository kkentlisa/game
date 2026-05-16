using UnityEngine;

public class FurnitureItem : MonoBehaviour
{
    public bool canBeDestroyed = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private float pulseSpeed = 2.2f;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null ) originalColor = spriteRenderer.color;
    }

    public void SetAsTarget(bool isTarget)
    {
        canBeDestroyed = isTarget;
        if (!isTarget && spriteRenderer != null ) spriteRenderer.color = originalColor;
    }

    private void Update()
    {
        if (canBeDestroyed && spriteRenderer != null)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

            float brightnessBoost = 1.8f;
            Color superBright = new Color(1f * brightnessBoost, 1f * brightnessBoost, 0.8f * brightnessBoost, 1f);

            spriteRenderer.color = Color.Lerp(originalColor, superBright, t);
        }
    }
}
