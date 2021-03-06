﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScoreCenter : MonoBehaviour
{
    GameManager gameManager;
    TextMeshProUGUI tGUI;
    public int avgFrameRate;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        tGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        tGUI.text =
            "Sc: " + gameManager.score.ToString() + "\n" +
            "Multi: " + gameManager.scoreModifier.ToString();
    }
}
