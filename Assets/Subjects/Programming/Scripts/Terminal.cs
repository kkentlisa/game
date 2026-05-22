using UnityEngine;

public class Terminal : MonoBehaviour
{
    private bool isPlayerNearby = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Кто-то зашел в триггер: " + other.name); 
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок обнаружен!"); 
       
            TerminalData data = GetComponent<TerminalData>();
            
            if (GameManager.Instance != null && data != null)
            {
                GameManager.Instance.OpenTerminalTask(gameObject , data);
            }
        }
    }
}