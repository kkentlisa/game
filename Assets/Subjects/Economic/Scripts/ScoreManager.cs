using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI leaderboardText;

    public event Action OnScoreChanged;

    private Dictionary<string, int> scores = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OnScoreChanged += UpdateLeaderboardUI;
        AddScore("Player", 0);
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

    private void UpdateLeaderboardUI()
    {
        if (leaderboardText == null) return;

        leaderboardText.text = "LEADERBOARD:\n";
        var ranking = GetRanking();

        for (int i = 0; i < ranking.Count; i++)
        {
            leaderboardText.text += $"{i + 1}. {ranking[i].name}: {ranking[i].score}\n";
        }
    }

}
