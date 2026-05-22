using UnityEngine;

public class ClickableGradebook : MonoBehaviour
{

    public GameObject gradebookUI;
    private bool isActive = false;

    public void SetActive(bool active)
    {
        isActive = active;
    }

    void OnMouseDown()
    {
        if (isActive && gradebookUI != null)
        {
            gradebookUI.SetActive(true);
        }
    }

    void OnEnable()
    {
        if (LevelBridgeManager.instance != null && LevelBridgeManager.instance.isAuthorized)
            isActive = true;
    }

    public void CloseGradebook()
    {
        gradebookUI.SetActive(false);
    }
}