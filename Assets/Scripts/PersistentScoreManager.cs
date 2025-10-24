using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TeamScore
{
    public string teamName;
    public int highscore;
}

[System.Serializable]
public class ScoreData
{
    public List<TeamScore> teams = new List<TeamScore>();
}

public class PersistentScoreManager : MonoBehaviour
{
    private string savePath;
    private ScoreData data = new ScoreData();

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "scores.json");
        LoadScores();
    }

    public void RecordScore(string teamName, int score)
    {
        var existing = data.teams.Find(t => t.teamName == teamName);
        if (existing != null)
        {
            if (score > existing.highscore)
                existing.highscore = score;
        }
        else
        {
            data.teams.Add(new TeamScore { teamName = teamName, highscore = score });
        }
        SaveScores();
    }

    public void SaveScores()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadScores()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<ScoreData>(json);
        }
    }
    
    public List<TeamScore> GetLeaderboard()
    {
        return data.teams;
    }

    public void PrintLeaderboard()
    {
        foreach (var team in data.teams)
            Debug.Log($"{team.teamName}: {team.highscore}");
    }
}