using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIScore : MonoBehaviour
{
    GameManager gameManager;
    TextMeshProUGUI tGUI;
    public int avgFrameRate;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        tGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
        tGUI.text =
            "ScoreBest: " + GameManager.scoreBest.ToString() + "\n" +
            "Score: " + gameManager.score.ToString() + "\n" +
            "Multiplier: " + gameManager.scoreModifier.ToString();
        if (gameManager.displayFPS)
        {
            tGUI.text = tGUI.text + "\n" +
            "FPS: " + avgFrameRate.ToString();
        }
    }
}
