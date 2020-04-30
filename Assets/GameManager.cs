using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static int scoreBest;
    public int score;
    public float scoreModifier;
    public bool godMode;
    public float difficultyModifier;
    public float starPowerMultiplier;

    void Start()
    {
        
    }
    public void addToScore(int score)
    {
        
        this.score += Mathf.RoundToInt( score*scoreModifier);
        print("add to score" + this.score);
    }
    public void setDifficultyMultiplier(float multiplier)
    {
        print("multiplier " + multiplier);
        this.difficultyModifier = multiplier;
    }
    // Update is called once per frame
    void Update()
    {
        scoreModifier = difficultyModifier * starPowerMultiplier;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMenu();
        }
        if (death)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartGame();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
        }
    }

    private static void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    bool setButtonEventHere;
    public void GoToMenu()
    {
        if (setButtonEventHere)
            StartGame();
        if (setButtonEventHere)
            HowToPlay();
        if (setButtonEventHere)
            ExitGame();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void HowToPlay()
    {

    }

    bool death;
    public void Death()
    {        
        if (godMode == false)
        {
            death = true;
            //hvis vi gerne vil fortælle spilleren score inden reset
            int yourScore = this.score;
            if (scoreBest < yourScore)
                scoreBest = yourScore;

            //show yourScore and scoreBest with big numbers in middle of screen!!
        }
    }
}