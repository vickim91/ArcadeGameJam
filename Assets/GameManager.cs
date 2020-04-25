using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score;
    void Start()
    {
        
    }
    public void addToScore(int score)
    {
        this.score += score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
