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
        this.gameObject.SetActive(true);
    }
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        tGUI.text = gameManager.score.ToString();
        SizingMethod();
    }

    private void SizingMethod()
    {
        if (isShrinking)
        {
            if (tGUI.fontSize > fontSizeNormal)
            {
                tGUI.fontSize -= fontSizingSpeed;
            }
            else
            {
                isSizing = false;
                isShrinking = false;
            }
        }
        else if (isSizing)
        {
            if (tGUI.fontSize < fontSizeMax)
            {
                tGUI.fontSize += fontSizingSpeed * 2;
            }
            else
                isShrinking = true;
        }
    }
    float fontSizingSpeed = 2f;
    float fontSizeMax = 50f;
    float fontSizeNormal = 47f;
    bool isSizing;
    bool isShrinking;
    public void AddPointToScore()// here
    {
        if (!isSizing)
        {
            isSizing = true;
        }
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
