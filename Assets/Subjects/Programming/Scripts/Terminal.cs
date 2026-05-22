using UnityEngine;

public class Terminal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TerminalData data = GetComponent<TerminalData>();
            if (GameManager.Instance != null && data != null)
            {
                GameManager.Instance.OpenTerminalTask(gameObject, data);
            }
        }
    }
}
