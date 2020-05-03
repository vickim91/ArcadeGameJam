using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PointMovement : MonoBehaviour
{
    ModuleSpawner moduleSpawner;
    GameManager gameManager;
    TextMeshProUGUI tGUI;
    Vector3 destination;
    [HideInInspector]
    public int thisPointAddition;
    GameObject panel;
    // spawn parameters:
    public float threshold; // ylocation of the score. Set this to a value that scales correctly with the aspect ratio 
    public float xPosMin;
    public float xPosMax;
    public float yPosMin;
    public float yPosMax;
    public float lerpSpeedMax;
    public float lerpSpeedMin;
    public float lerpAcceleration;
    float lerpSpeed;
    public static bool prevXisPositive;

    void Awake()
    {
        panel = gameObject.transform.Find("Panel").gameObject;
        gameManager = FindObjectOfType<GameManager>();
        moduleSpawner = FindObjectOfType<ModuleSpawner>();
        destination = new Vector3(0, threshold + 10, 0);
        tGUI = GetComponentInChildren<TextMeshProUGUI>();
        float x = Random.Range(xPosMin, xPosMax);
        if (prevXisPositive)
        {
            x = -Mathf.Abs(x);
            prevXisPositive = false;
        }
        else
        {
            x = Mathf.Abs(x);
            prevXisPositive = true;
        }
            
        float y = Random.Range(yPosMin, yPosMax);
        panel.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
        lerpSpeed = lerpSpeedMin;


        if (moduleSpawner.starPower) // here
        {
           
            StartCoroutine(flashCoroutine());

        }
        else
        {
            tGUI.color = new Color(0.5f, 0.5f, 1, 1);
        }
        switch(moduleSpawner.difficultyLevel) // here
        {
            case 0:
                break;
            case 1:
                break;
            case 2: 
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
                //....... (how high does number actually go?)
        }
    }

    
    IEnumerator flashCoroutine()
    {
        Color color1 = new Color(100f, 50f, 0f);
        Color color2 = new Color(0, 100f, 100f);
        Color color3 = new Color(100f, 0f, 100f);
        for(int i=0; i<30; i++)
        {
            tGUI.color = color1;
            yield return new WaitForSeconds(0.05f);
            tGUI.color = color2;
            yield return new WaitForSeconds(0.05f);
            tGUI.color = color3;
            yield return new WaitForSeconds(0.05f);

        }
    }
    void Update()
    {
        lerpSpeed += lerpAcceleration;
        if (lerpSpeed > lerpSpeedMax)
            lerpSpeed = lerpSpeedMax;
        panel.GetComponent<RectTransform>().localPosition = Vector3.Lerp(panel.GetComponent<RectTransform>().localPosition, destination, lerpSpeed);
        tGUI.text = thisPointAddition.ToString();
        if (panel.GetComponent<RectTransform>().localPosition.y > threshold)
        {
            gameManager.AddPointToScore(thisPointAddition);
            Destroy(this.gameObject);
        }
    }
}