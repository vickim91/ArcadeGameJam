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
