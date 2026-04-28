using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthAndGradebook : MonoBehaviour
{
    public GameObject authPanel;
    public TMP_InputField nameInput;
    public Button authButton;
    public ClickableGradebook gradebookClickable;
    public GameObject gradebookUI;

    void Start()
    {
        authPanel.SetActive(false);
        if (gradebookUI != null) gradebookUI.SetActive(false);
        if (gradebookClickable != null) gradebookClickable.SetActive(false);

        authButton.onClick.AddListener(Authorize);

        LoadSavedName();
    }

    public void LoadSavedName()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", "");
        if (!string.IsNullOrEmpty(savedName))
        {
            nameInput.text = savedName;
        }
    }

    void Authorize()
    {
        if (string.IsNullOrEmpty(nameInput.text))
        {
            Debug.Log("¬ведите им€!");
            return;
        }

        PlayerPrefs.SetString("PlayerName", nameInput.text);
        PlayerPrefs.Save();

        if (authPanel != null)
            authPanel.SetActive(false);

        if (gradebookClickable != null)
            gradebookClickable.SetActive(true);

    }
}