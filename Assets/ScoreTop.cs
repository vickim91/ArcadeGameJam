using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTop : MonoBehaviour
{
    ModuleSpawner moduleSpawner;

    GameManager gameManager;
    TextMeshProUGUI tGUI;

    // Start is called before the first frame update
    void Start()
    {
        moduleSpawner = FindObjectOfType<ModuleSpawner>();
        gameManager = FindObjectOfType<GameManager>();
        tGUI = GetComponent<TextMeshProUGUI>();
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //if (moduleSpawner.starPower)
        //    else

        tGUI.text ="Score: " + gameManager.score.ToString();
    }
}
