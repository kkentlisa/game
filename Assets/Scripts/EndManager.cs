using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    [Header("Ïàíåëè Ôèíàëîâ")]
    public GameObject badEndingPanel;
    public GameObject goodEndingPanel;

    [Header("Òåêñò äëÿ ïëîõîé êîíöîâêè")]
    public TextMeshProUGUI missingStudentText; 

    [Header("Ôîòî ïğîïàâøåãî ñòóäåíòà (õîğîøàÿ êîíöîâêà)")]
    public GameObject missingStudentPhoto;

    public void CloseGradebookForever()
    {
        Debug.Log("CloseGradebookForever ÂÛÇÂÀÍ");
        if (LevelBridgeManager.instance == null) return;

        float averageGrade = LevelBridgeManager.instance.GetAverageGrade();

        if (averageGrade >= 5.0f || averageGrade <= 3.0f)
        {
            TriggerBadEnding();
        }
        else 
        {
            TriggerGoodEnding();
        }
    }

    private void TriggerBadEnding()
    {
        if (badEndingPanel != null)
            badEndingPanel.SetActive(true);

        if (missingStudentText != null && LevelBridgeManager.instance != null)
        {
            missingStudentText.text = "ÍÎÂÛÉ ÇÀËÎÆÍÈÊ: " + LevelBridgeManager.instance.playerName;
        }
    }

    private void TriggerGoodEnding()
    {
        if (goodEndingPanel != null)
            goodEndingPanel.SetActive(true);

        if (missingStudentPhoto != null)
            missingStudentPhoto.SetActive(true);
    }

    public void RestartGame()
    {
        if (LevelBridgeManager.instance != null)
        {
            Destroy(LevelBridgeManager.instance.gameObject);
        }
        SceneManager.LoadScene("HubScene");
    }
}