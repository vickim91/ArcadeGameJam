using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PointMovement : MonoBehaviour
{
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