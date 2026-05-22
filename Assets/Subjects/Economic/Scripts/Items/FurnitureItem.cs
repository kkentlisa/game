using System.Collections;
using UnityEngine;

public class FurnitureItem : MonoBehaviour
{
    public bool canBeDestroyed = false;
    public bool isBroken = false;

    private int minCoins = 2;
    private int maxCoins = 6;
    public GameObject coinPrefab;

    private SpriteRenderer spriteRenderer;
    private Collider2D itemCollider;
    private Color originalColor;

    private float pulseSpeed = 2.2f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollider = GetComponent<Collider2D>();
        if (spriteRenderer != null ) originalColor = spriteRenderer.color;
    }

    public void SetAsTarget(bool isTarget)
    {
        canBeDestroyed = isTarget;
        if (!isTarget && spriteRenderer != null ) spriteRenderer.color = originalColor;
    }

    private void Update()
    {
        if (canBeDestroyed && !isBroken && spriteRenderer != null)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            float brightnessBoost = 1.8f;
            Color superBright = new Color(1f * brightnessBoost, 1f * brightnessBoost, 0.8f * brightnessBoost, 1f);

            spriteRenderer.color = Color.Lerp(originalColor, superBright, t);
        }
    }

    public void Break()
    {
        if (!canBeDestroyed || isBroken) return;
        isBroken = true;

        if (itemCollider != null) itemCollider.enabled = false;

        SpawnCoins();
        StartCoroutine(DestroyAnimation());
    }

    private void SpawnCoins()
    {
        if (coinPrefab == null) return;

        int wallLayerMask = LayerMask.GetMask("Walls");

        int count = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < count; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPosition = transform.position + (Vector3)randomDirection * 0.25f; ;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDirection, 0.6f, wallLayerMask);

            if (hit.collider != null)
            {
                randomDirection = -randomDirection;
                spawnPosition = transform.position + (Vector3)randomDirection * 0.3f;
            }


            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                float randomForce = Random.Range(1.0f, 2.2f);
                rb.AddForce(randomDirection * randomForce, ForceMode2D.Impulse);
            }
        }
    }

    private IEnumerator DestroyAnimation()
    {
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 6; i++)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(0.05f);
            }
        }
        gameObject.SetActive(false);
        NavMeshSurfaceManagement.Instance.RebakeNavMeshSurface();
    }


}
