using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
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
        this.difficultyModifier = multiplier;
    }
    // Update is called once per frame
    void Update()
    {
        scoreModifier = difficultyModifier * starPowerMultiplier;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
        if (death)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    bool death;
    public void Death()
    {        
        if (godMode == false)
        {
            death = true;
            //hvis vi gerne vil fortælle spilleren score inden reset
            int yourScore = this.score;
        }
    }
}
