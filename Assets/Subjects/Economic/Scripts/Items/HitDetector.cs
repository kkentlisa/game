using UnityEngine;

public class HitDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Destructible"))
        {
            FurnitureItem furniture = collision.GetComponent<FurnitureItem>();

            if (furniture != null)
            {
                furniture.Break();
            }
        }
    }
}
