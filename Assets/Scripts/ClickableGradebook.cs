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
            LevelBridgeManager.instance?.BindButtons();
        }
    }

    void OnEnable()
    {
        if (LevelBridgeManager.instance != null && LevelBridgeManager.instance.isAuthorized)
            isActive = true;
        else
            isActive = false;
    }

    public void CloseGradebook()
    {
        gradebookUI.SetActive(false);
    }
}