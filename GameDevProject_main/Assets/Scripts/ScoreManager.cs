using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ScoreData
{
    public List<Score> scores;

    public ScoreData()
    {
        scores = new List<Score>();
    }
}

public class ScoreManager : MonoBehaviour
{
    ScoreData scoreData;

    private void Awake()
    {
        // Deserialize ScoreData from PlayerPrefs
        string json = PlayerPrefs.GetString("scores", "{}");
        scoreData = JsonUtility.FromJson<ScoreData>(json);
    }

    public bool noScores()
    {
        return scoreData.scores.Count == 0;
    }

    public IEnumerable<Score> GetScores()
    {
        return scoreData.scores.OrderByDescending(x => x.score);
    }

    public void AddScore(Score score)
    {
        if (scoreData.scores.Count >= 7)
        {
            scoreData.scores = scoreData.scores.OrderByDescending(x => x.score).ToList();
            scoreData.scores.RemoveAt(scoreData.scores.Count - 1);
        }
        scoreData.scores.Add(score);

        // Serialize ScoreData and save to PlayerPrefs
        string json = JsonUtility.ToJson(scoreData);
        PlayerPrefs.SetString("scores", json);
    }
}
