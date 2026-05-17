using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage();
            }

            //Destroy(gameObject);
        }
    }
}