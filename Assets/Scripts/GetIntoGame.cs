using UnityEngine;
using UnityEngine.SceneManagement;

public class GetIntoGame : MonoBehaviour
{
    public void LoadMathGameScene()
    {
        SceneManager.LoadScene("MathAnalysisGameScene");
    }

    public void LoadProgrammingGameScene()
    {
        SceneManager.LoadScene("ProgrammingGameScene");
    }

    public void LoadEconomyGameScene()
    {
        SceneManager.LoadScene("EconimicGameScene");
    }
}
