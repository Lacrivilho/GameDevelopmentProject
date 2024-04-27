using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject defaultScreen;
    public GameObject winScreen;
    public GameObject looseScreen;
    public GameObject huntRecordScreen;
    public GameObject huntWinScreen;
    public GameObject huntFailScreen;
    public ScoreManager scoreManager;
    public Text scoreLabelWin;
    public Text scoreLabelRecord;
    public Text scoreLabelFail;

    private List<Score> scores;

    private void Awake()
    {
        scores = scoreManager.GetScores().ToList();
        
        if (GameManager.Instance.gameOver)
        {
            if(GameManager.Instance.lastGameMode == 3)
            {
                if(scoreManager.noScores())
                {
                    defaultScreen.SetActive(false);
                    winScreen.SetActive(false);
                    looseScreen.SetActive(false);
                    huntRecordScreen.SetActive(true);
                    huntWinScreen.SetActive(false);
                    huntFailScreen.SetActive(false);
                    scoreLabelRecord.text = "Killcount: " + GameManager.Instance.killScore;
                }
                else
                {
                    if (GameManager.Instance.killScore > scores.Last().score || scoreManager.GetScores().Count() < 7)
                    {
                        if (GameManager.Instance.killScore > scores.First().score)
                        {
                            defaultScreen.SetActive(false);
                            winScreen.SetActive(false);
                            looseScreen.SetActive(false);
                            huntRecordScreen.SetActive(true);
                            huntWinScreen.SetActive(false);
                            huntFailScreen.SetActive(false);
                            scoreLabelRecord.text = "Killcount: " + GameManager.Instance.killScore;
                        }
                        else
                        {
                            defaultScreen.SetActive(false);
                            winScreen.SetActive(false);
                            looseScreen.SetActive(false);
                            huntRecordScreen.SetActive(false);
                            huntWinScreen.SetActive(true);
                            huntFailScreen.SetActive(false);
                            scoreLabelWin.text = "Killcount: " + GameManager.Instance.killScore;
                        }
                    }
                    else
                    {
                        defaultScreen.SetActive(false);
                        winScreen.SetActive(false);
                        looseScreen.SetActive(false);
                        huntRecordScreen.SetActive(false);
                        huntWinScreen.SetActive(false);
                        huntFailScreen.SetActive(true);
                        scoreLabelFail.text = "Killcount: " + GameManager.Instance.killScore;
                    }
                }
                
            }
            else if (GameManager.Instance.win)
            {
                defaultScreen.SetActive(false);
                winScreen.SetActive(true);
                looseScreen.SetActive(false);
                huntRecordScreen.SetActive(false);
                huntWinScreen.SetActive(false);
                huntFailScreen.SetActive(false);
            }
            else
            {
                defaultScreen.SetActive(false);
                winScreen.SetActive(false);
                looseScreen.SetActive(true);
                huntRecordScreen.SetActive(false);
                huntWinScreen.SetActive(false);
                huntFailScreen.SetActive(false);
            }
        }
        else
        {
            defaultScreen.SetActive(true);
            winScreen.SetActive(false);
            looseScreen.SetActive(false);
            huntRecordScreen.SetActive(false);
            huntWinScreen.SetActive(false);
            huntFailScreen.SetActive(false);
        }
    }

    public void setNewRecord(string input)
    {
        if(scores.Count >= 7)
        {
            scores.RemoveAt(scores.Count-1);
        }
        scores.Add(new Score(input, GameManager.Instance.killScore));
        scoreManager.AddScore(new Score(input, GameManager.Instance.killScore));
    }

    public void playGameSEasy()
    {
        GameManager.Instance.setSEasy();
        SceneManager.LoadSceneAsync(1);
    }
    public void playGameSMid()
    {
        GameManager.Instance.setSMid();
        SceneManager.LoadSceneAsync(1);
    }
    public void playGameSHard()
    {
        GameManager.Instance.setSHard();
        SceneManager.LoadSceneAsync(1);
    }
    public void playGameHunt()
    {
        GameManager.Instance.setHunt();
        SceneManager.LoadSceneAsync(1);
    }
    public void playGameSame()
    {
        switch(GameManager.Instance.lastGameMode)
        {
            case 0:
                GameManager.Instance.setSEasy();
                SceneManager.LoadSceneAsync(1);
                break;
            case 1:
                GameManager.Instance.setSMid();
                SceneManager.LoadSceneAsync(1);
                break;
            case 2:
                GameManager.Instance.setSHard();
                SceneManager.LoadSceneAsync(1);
                break;
            case 3:
                GameManager.Instance.setHunt();
                SceneManager.LoadSceneAsync(1);
                break;
            default:
                print("Mode not implemented!");
                break;
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
