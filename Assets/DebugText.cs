using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DebugText : MonoBehaviour
{
    //GameManager gameManager;
    //TextMeshProUGUI tGUI;
    //public int avgFrameRate;

    void start()
    {
        //gamemanager = findobjectoftype<gamemanager>();
        //tgui = getcomponent<textmeshprougui>();
    }

    float deltaTime = 0.0f;
    bool pauseFPS;

    void Update()
    {
//        float current = 0;
//        current = Time.frameCount / Time.time;
//        avgFrameRate = (int)current;
//        //tGUI.text =
//        //    "ScoreBest: " + GameManager.scoreBest.ToString() + "\n" 
//            //+ "Score: " + gameManager.score.ToString() + "\n" 
//            //+ "Multiplier: " + gameManager.scoreModifier.ToString();
//        if (gameManager.displayFPS)
//        {
//            tGUI.text = 
////                tGUI.text + "\n" +
//            "FPS: " + avgFrameRate.ToString();
//        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pauseFPS)
                pauseFPS = false;
            else if (!pauseFPS)
                pauseFPS = true;
        }
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }


    void OnGUI()
    {

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(1, 1, 1, 1);
        float msec = deltaTime * 1000.0f;
        int fps = Mathf.RoundToInt(1.0f / deltaTime);
        if (!pauseFPS)
            text = "fps: " + fps;
        GUI.Label(rect, text, style);
    }
    string text;
}
