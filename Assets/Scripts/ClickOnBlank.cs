using UnityEngine;

public class ClickOnBlank : MonoBehaviour
{
    public GameObject authPanel;

    void OnMouseDown()
    {
        if (authPanel != null)
        {
            authPanel.SetActive(true);
        }
    }
}
