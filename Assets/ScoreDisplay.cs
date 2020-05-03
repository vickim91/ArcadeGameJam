using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    GameManager gameManager;
    TextMeshProUGUI tGUI;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        tGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        tGUI.text = gameManager.score.ToString();
    }

    public void AddPointToScore()// here
    {

    }
    public void StarPower()// here
    {

    }
    public void NoStarPower()// here
    {

    }
    public void UpgradeDifficultyModifier(int difficultyLevel)// here (1,2,3...)
    {

    }
}
