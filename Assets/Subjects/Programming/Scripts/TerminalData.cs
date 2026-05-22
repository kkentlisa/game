using UnityEngine;

[System.Serializable]
public struct TaskInfo
{
    [TextArea(3, 5)]
    public string questionText;   
    public string correctAnswer;    
    public string wrongAnswer;    
}

public class TerminalData : MonoBehaviour
{
    [Header("Название терминала")]
    public string terminalName;

    [Header("Задачи для разных сложностей")]
    public TaskInfo easyTask;
    public TaskInfo mediumTask;
    public TaskInfo hardTask;

    public TaskInfo GetTaskForCurrentDifficulty()
    {
        if (GameManager.Instance != null)
        {
            switch (GameManager.Instance.baseDifficulty)
            {
                case LevelDifficulty.Easy: return easyTask;
                case LevelDifficulty.Medium: return mediumTask;
                case LevelDifficulty.Hard: return hardTask;
            }
        }
        return easyTask; 
    }
}