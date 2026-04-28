using UnityEngine;

public class ClickableBlank : MonoBehaviour
{
    public GameObject authPanel;
    public AuthAndGradebook authManager;
    void OnMouseDown()
    {
        if (authPanel != null)
        {
            authPanel.SetActive(true);
            authManager?.LoadSavedName();
        }
    }
}