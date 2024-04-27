using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Score
{
    public string playerName;
    public int score;

    public Score(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}
