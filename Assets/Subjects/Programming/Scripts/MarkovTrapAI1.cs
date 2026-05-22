using UnityEngine;
using System.Collections.Generic;

public class MarkovTrapAI1 : MonoBehaviour
{
    public Transform player;
    public GameObject trapPrefab;
    public float tickRate = 2f; 
    
    private Vector2Int lastCell;
    private string lastMove = "";
    
    private Dictionary<string, Dictionary<string, int>> markovChain = new Dictionary<string, Dictionary<string, int>>();

    void Start()
    {
        lastCell = GetPlayerCell();
        InvokeRepeating("AnalyzeAndTrap", tickRate, tickRate);
    }

    void AnalyzeAndTrap()
    {
        Vector2Int currentCell = GetPlayerCell();
        string currentMove = DetermineMove(lastCell, currentCell);

        if (currentMove != "Stay" && lastMove != "")
        {
            if (!markovChain.ContainsKey(lastMove)) markovChain[lastMove] = new Dictionary<string, int>();
            if (!markovChain[lastMove].ContainsKey(currentMove)) markovChain[lastMove][currentMove] = 0;
            
            markovChain[lastMove][currentMove]++;
        }

        string predictedMove = PredictNextMove(currentMove);
        SpawnTrap(currentCell, predictedMove);

        lastCell = currentCell;
        lastMove = currentMove;
    }

    string DetermineMove(Vector2Int from, Vector2Int to)
    {
        if (to.x > from.x) return "Right";
        if (to.x < from.x) return "Left";
        if (to.y > from.y) return "Up";
        if (to.y < from.y) return "Down";
        return "Stay";
    }

    string PredictNextMove(string currentMove)
    {
        if (!markovChain.ContainsKey(currentMove)) return "Random";

        string bestMove = "Random";
        int maxCount = -1;

        foreach (var entry in markovChain[currentMove])
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
        Vector3 spawnPos = new Vector3(currentCell.x * 0.1f, currentCell.y * 0.1f, 0); 
        
        if (direction == "Right") spawnPos += new Vector3(0.1f, 0, 0);
        else if (direction == "Left") spawnPos -= new Vector3(0.1f, 0, 0);
        else if (direction == "Up") spawnPos += new Vector3(0, 0.1f, 0);
        else if (direction == "Down") spawnPos -= new Vector3(0, 0.1f, 0);
        else return; 

        GameObject trap = Instantiate(trapPrefab, spawnPos, Quaternion.identity);
        Destroy(trap, 3f); 
    }

    Vector2Int GetPlayerCell()
    {
        return new Vector2Int(Mathf.RoundToInt(player.position.x / 0.1f), Mathf.RoundToInt(player.position.y / 0.1f));
    }
}