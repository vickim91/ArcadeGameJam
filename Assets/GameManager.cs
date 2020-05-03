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
    public GameObject scoreAddition;
    MultiplierDisplay multiplierDisplay;
    ScoreDisplay scoreDisplay;
    GameObject scoreAdditionSpawnPosition;
    DeathScreen deathScreen;

    void Start()
    {
        scoreAdditionSpawnPosition = scoreAddition;
        multiplierDisplay = FindObjectOfType<MultiplierDisplay>();
        scoreDisplay = FindObjectOfType<ScoreDisplay>();
        dead = false;
        audioManager = FindObjectOfType<AudioManager>();
        deathScreen = FindObjectOfType<DeathScreen>();
        deathScreen.ShowDeathScreen(false);
    }
    public void Point(int point)
    {
        GameObject scoreObject = Instantiate(scoreAddition, scoreAdditionSpawnPosition.transform.position, scoreAdditionSpawnPosition.transform.rotation) as GameObject;
        scoreObject.GetComponentInChildren<PointMovement>().thisPointAddition = Mathf.RoundToInt(point * scoreModifier);
    }
    public void AddPointToScore(int point)
    {
        this.score += Mathf.RoundToInt(point * scoreModifier);
        scoreDisplay.AddPointToScore();
    }
    public void SetDifficultyMultiplier(float multiplier)
    {
        //print("multiplier " + multiplier);
        this.difficultyModifier = multiplier;
    }
    // Update is called once per frame
    void Update()
    {
        scoreModifier = difficultyModifier * starPowerMultiplier;
        multiplierDisplay.multiplier = Mathf.RoundToInt(scoreModifier);
        if (dead)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartGame();
                audioManager.GameStart();
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            scoreModifier = Random.Range(1, 5);
            Point(100);
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
            deathScreen.ShowDeathScreen(true);
            scoreDisplay.gameObject.SetActive(false);
            multiplierDisplay.gameObject.SetActive(false);
        }
    }
}