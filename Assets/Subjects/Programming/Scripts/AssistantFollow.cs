using UnityEngine;

public class AssistantFollow : MonoBehaviour
{
    [Header("Настройки преследования")]
    public Transform playerTransform; 
    public float speed = 2f;        
    public float stoppingDistance = 1.2f; 

    void Update()
    {
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance > stoppingDistance)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, 
                    playerTransform.position, 
                    speed * Time.deltaTime
                );
            }
        }
    }
}