using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static int scoreBest;
    public int score;
    public float scoreModifier;
    public bool displayFPS;
    public bool godMode;
    public float difficultyModifier;
    public float starPowerMultiplier;
    AudioManager audioManager;

    void Start()
    {
        dead = false;
        audioManager = FindObjectOfType<AudioManager>();
    }
    public void addToScore(int score)
    {
        
        this.score += Mathf.RoundToInt( score*scoreModifier);
        //print("add to score" + this.score);
    }
    public void setDifficultyMultiplier(float multiplier)
    {
        //print("multiplier " + multiplier);
        this.difficultyModifier = multiplier;
    }
    // Update is called once per frame
    void Update()
    {
        scoreModifier = difficultyModifier * starPowerMultiplier;
        if (dead)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartGame();
                audioManager.GameStart();
            }
        }
    }

    public static void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void HowToPlay()
    {
        
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public bool dead;
    public void Death()
    {        
        if (godMode == false)
        {
            dead = true;
            //hvis vi gerne vil fortælle spilleren score inden reset
            int yourScore = this.score;
            if (scoreBest < yourScore)
                scoreBest = yourScore;

            //show yourScore and scoreBest with big numbers in middle of screen!!
        }
    }
}