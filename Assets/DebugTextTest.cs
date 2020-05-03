using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DebugTextTest : MonoBehaviour
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
        //tGUI.text =
        //    "ScoreBest: " + GameManager.scoreBest.ToString() + "\n" 
        //+ "Score: " + gameManager.score.ToString() + "\n" 
        //+ "Multiplier: " + gameManager.scoreModifier.ToString();
        if (gameManager.displayFPS)
        {
            tGUI.text =
            //                tGUI.text + "\n" +
            "FPS: " + avgFrameRate.ToString();


        }
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    float deltaTime = 0.0f;

    //void OnGUI()
    //{

    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, w, h * 2 / 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = h * 2 / 50;
    //    style.normal.textColor = new Color(1, 1, 1, 1);
    //    float msec = deltaTime * 1000.0f;
    //    int fps = Mathf.RoundToInt(1.0f / deltaTime);
    //    //        string text = "fps:" + fps;
    //    string text = "test";
    //    GUI.Label(rect, text, style);
    //}
}
