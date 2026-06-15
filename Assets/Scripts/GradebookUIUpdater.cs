using TMPro;
using UnityEngine;

public class GradebookUIUpdater : MonoBehaviour
{
    public TextMeshProUGUI mathText;
    public TextMeshProUGUI programmingText;
    public TextMeshProUGUI economyText;

    void OnEnable()
    {
        if (LevelBridgeManager.instance == null)
        {
            Debug.LogError("LevelBridgeManager не найден!");
            return;
        }

        mathText.text = LevelBridgeManager.instance.mathGrade.ToString();
        programmingText.text = LevelBridgeManager.instance.programmingGrade.ToString();
        economyText.text = LevelBridgeManager.instance.economyGrade.ToString();

        Debug.Log($"UI обновлён: мат={LevelBridgeManager.instance.mathGrade}, прог={LevelBridgeManager.instance.programmingGrade}, экон={LevelBridgeManager.instance.economyGrade}");
    }
}