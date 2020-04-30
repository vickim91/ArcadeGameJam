using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public int score;
    public float scoreModifier;
    public bool godMode;

    void Start()
    {
        
    }
    public void addToScore(int score)
    {
        this.score += Mathf.RoundToInt( score*scoreModifier);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Death()
    {
        if (godMode == false)
        {
            //hvis vi gerne vil fortælle spilleren score inden reset
            int yourScore = this.score;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }
}
