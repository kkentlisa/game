using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int lives = 3;
    public GameObject quizPanel;

    void Start()
    {
        quizPanel.SetActive(false); 
    }

    public void StartQuiz()
    {
        lives--;
        Debug.Log("Попался! Осталось жизней: " + lives);

        if (lives <= 0)
        {
            Debug.Log("Game Over! Пересдача.");
            return;
        }

        quizPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void AnswerQuestion()
    {
        quizPanel.SetActive(false);
        Time.timeScale = 1f;
        
    }
}