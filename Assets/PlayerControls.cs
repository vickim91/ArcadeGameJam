using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    ModuleSpawner moduleSpawner;
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        moduleSpawner = FindObjectOfType<ModuleSpawner>();
    }

    private void Update()
    {
        if (!gameManager.dead && !moduleSpawner.controlsDisabled)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                moduleSpawner.SelectNextModule();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                moduleSpawner.SelectPreviousModule();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moduleSpawner.RotateSelectedModuleCounterclockwise();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                moduleSpawner.RotateSelectedModuleClockwise();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // open menu
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // retry, if player is dead
        }
    }
}
