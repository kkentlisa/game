using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action OnScoreChanged;

    private Dictionary<string, int> scores = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(string participant, int amount)
    {
        if (!scores.ContainsKey(participant)) scores[participant] = 0;
        scores[participant] += amount;
        OnScoreChanged?.Invoke();
    }

    public List<(string name, int score)> GetRanking()
    {
        var list = scores.Select(kv => (kv.Key, kv.Value)).ToList();
        list.Sort((a, b) => b.Item2.CompareTo(a.Item2));
        return list;
    }

    public int GetPlayerRank()
    {
        var ranking = GetRanking();
        return ranking.FindIndex(x => x.name == "Player") + 1;
    }

}
