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
    float threshold = 160;
    public int thisPointAddition;
    GameObject panel;

    // Start is called before the first frame update
    void Awake()
    {
        panel = gameObject.transform.Find("Panel").gameObject;
        gameManager = FindObjectOfType<GameManager>();
        destination = new Vector3(0, threshold + 10, 0);
        tGUI = GetComponentInChildren<TextMeshProUGUI>();

        float x = Random.Range(-150, 150);
        float y = Random.Range(10, 40);
        panel.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
    }

    void Update()
    {
        panel.GetComponent<RectTransform>().localPosition = Vector3.Lerp(panel.GetComponent<RectTransform>().localPosition, destination, 0.05f);
        tGUI.text = thisPointAddition.ToString();
        if (panel.GetComponent<RectTransform>().localPosition.y > threshold)
        {
            gameManager.AddPointToScore(thisPointAddition);
            Destroy(this.gameObject);
        }
    }
}