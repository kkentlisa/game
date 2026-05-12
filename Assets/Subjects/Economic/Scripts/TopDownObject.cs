using UnityEngine;

public class TopDownObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public int sortingOrderOffset = 0;
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateSorting();
    }

    void LateUpdate()
    {
        UpdateSorting();
    }

    public void UpdateSorting()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100) + 5000 + sortingOrderOffset;
        }
    }
}
