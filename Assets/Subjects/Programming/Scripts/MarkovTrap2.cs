using UnityEngine;
using System.Collections.Generic;

public class MarkovTrapAI2 : MonoBehaviour
{
    public Transform player;
    public GameObject trapPrefab;
    public float tickRate = 1.5f; 
    public float cellSize = 0.1f;

    private Vector2Int lastCell;
    private string penultMove = ""; 
    private string lastMove = "";   

    private Dictionary<string, Dictionary<string, int>> markovChain2ndOrder = new Dictionary<string, Dictionary<string, int>>();

    void Start()
    {
        lastCell = GetPlayerCell();
        StartCoroutine(MarkovAICycleRoutine());    }

    System.Collections.IEnumerator MarkovAICycleRoutine()
    {
        while (true)
        {
            AnalyzeAndTrap();

            float dynamicTickRate = 1.5f;
            if (GameManager.Instance != null)
            {
                dynamicTickRate = GameManager.Instance.EvaluateDecisionTree().aiTickRate;
            }

            yield return new WaitForSeconds(dynamicTickRate);
        }
    }
    
    void AnalyzeAndTrap()
    {
        Vector2Int currentCell = GetPlayerCell();
        string currentMove = DetermineMove(lastCell, currentCell);

        if (currentMove != "Stay")
        {
            if (penultMove != "" && lastMove != "")
            {
                string stateKey = penultMove + "_" + lastMove;

                if (!markovChain2ndOrder.ContainsKey(stateKey))
                    markovChain2ndOrder[stateKey] = new Dictionary<string, int>();

                if (!markovChain2ndOrder[stateKey].ContainsKey(currentMove))
                    markovChain2ndOrder[stateKey][currentMove] = 0;

                markovChain2ndOrder[stateKey][currentMove]++;
            }

            penultMove = lastMove;
            lastMove = currentMove;
        }

        string currentState = penultMove + "_" + lastMove;
        string predictedMove = PredictNextMove(currentState);
        
        if (predictedMove != "Random")
        {
            SpawnTrap(currentCell, predictedMove);
        }

        lastCell = currentCell;
    }

    string PredictNextMove(string stateKey)
    {
        if (!markovChain2ndOrder.ContainsKey(stateKey)) return "Random";

        string bestMove = "Random";
        int maxCount = -1;

        foreach (var entry in markovChain2ndOrder[stateKey])
        {
            if (entry.Value > maxCount)
            {
                maxCount = entry.Value;
                bestMove = entry.Key;
            }
        }
        return bestMove;
    }

    void SpawnTrap(Vector2Int currentCell, string direction)
    {
        Vector3 spawnPos = new Vector3(currentCell.x * cellSize, currentCell.y * cellSize, 0);

        float step = cellSize*3;
        if (direction == "Right") spawnPos += new Vector3(step, 0, 0);
        else if (direction == "Left") spawnPos -= new Vector3(step, 0, 0);
        else if (direction == "Up") spawnPos += new Vector3(0, step, 0);
        else if (direction == "Down") spawnPos -= new Vector3(0, step, 0);

        GameObject trap = Instantiate(trapPrefab, spawnPos, Quaternion.identity);
        Destroy(trap, 2.5f); 
    }

    string DetermineMove(Vector2Int from, Vector2Int to)
    {
        if (to.x > from.x) return "Right";
        if (to.x < from.x) return "Left";
        if (to.y > from.y) return "Up";
        if (to.y < from.y) return "Down";
        return "Stay";
    }

    Vector2Int GetPlayerCell()
    {
        return new Vector2Int(Mathf.RoundToInt(player.position.x / cellSize), Mathf.RoundToInt(player.position.y / cellSize));
    }

    public void ResetMemory()
    {
        markovChain2ndOrder.Clear();
        penultMove = "";
        lastMove = "";
    }
}