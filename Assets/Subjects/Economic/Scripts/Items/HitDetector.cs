using UnityEngine;

public class HitDetector : MonoBehaviour
{
    [SerializeField] private int baseDamage = 3;
    [SerializeField] private float knockbackForce = 4f;

    [HideInInspector] public string ownerName = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Destructible"))
        {
            collision.GetComponent<FurnitureItem>()?.Break();
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            EnemyAI enemy = collision.GetComponentInParent<EnemyAI>();
            if (enemy == null || enemy.EnemyName == ownerName) return;

            ScoreManager.Instance.TransferScore(enemy.EnemyName, ownerName, baseDamage);

            Rigidbody2D enemyRb = collision.GetComponentInParent<Rigidbody2D>();

            if (enemyRb != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }

            enemy.OnHitReceived();
            return;
        }

        if (collision.CompareTag("Player") && ownerName != "Player")
        {
            ScoreManager.Instance.TransferScore("Player", ownerName, baseDamage);

            Rigidbody2D playerRb = collision.GetComponentInParent<Rigidbody2D>();

            if (playerRb != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
