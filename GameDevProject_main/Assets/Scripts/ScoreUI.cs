using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public RowUI rowUI;
    public ScoreManager scoreManager;

    private void Start()
    {
        var scores = scoreManager.GetScores().ToArray();
        for(int i = 0; i < scores.Length; i++)
        {
            var row = Instantiate(rowUI, transform).GetComponent<RowUI>();
            row.rank.text = (i+1).ToString();
            row.playerName.text = scores[i].playerName;
            row.score.text = scores[i].score.ToString();
        }
    }
}
