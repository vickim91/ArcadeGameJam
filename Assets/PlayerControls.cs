using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    ModuleSpawner moduleSpawner;
    GameManager gameManager;
    PauseMenu pauseMenu;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        moduleSpawner = FindObjectOfType<ModuleSpawner>();
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    private void Update()
    {
        if (!gameManager.dead && !moduleSpawner.controlsDisabled && !pauseMenu.menu)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                moduleSpawner.SelectNextModule();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)|| Input.GetKeyDown(KeyCode.S))
            {
                moduleSpawner.SelectPreviousModule();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                moduleSpawner.RotateSelectedModuleCounterclockwise();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
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
