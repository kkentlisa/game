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

    public int GetScore(string participant)
    {
        return scores.TryGetValue(participant, out int val) ? val : 0;
    }

    public int TransferScore(string from, string to, int amount)
    {
        int available = GetScore(from);
        int actualTransfer = Mathf.Min(available, amount);

        if (actualTransfer <= 0) return 0;

        AddScore(from, -actualTransfer);
        AddScore(to, actualTransfer);

        return actualTransfer;
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
            string line = $"{i + 1}. {ranking[i].name}: {ranking[i].score}";
            if (ranking[i].name == "Player")
                line = $"<color=yellow>{line}</color>";

            leaderboardText.text += line + "\n";
        }
    }

}
