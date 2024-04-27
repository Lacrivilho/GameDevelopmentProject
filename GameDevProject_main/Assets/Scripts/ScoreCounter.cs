using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    private Text textComponent;
    private void Awake()
    {
        if (GameManager.Instance.gameModeSurvive)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        textComponent = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        textComponent.text = GameManager.Instance.killScore.ToString();
    }
}
