using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool menu;
    AudioManager audioManager;
    public GameObject panel;
    public GameObject start;
    public GameObject howToPlay;
    public GameObject settings;


    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        panel.SetActive(menu);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (panel.activeInHierarchy)
        {
            Time.timeScale = 1;
            menu = false;
            audioManager.ToggleMenu(menu);
        }
        else
        {
            Time.timeScale = 0;
            menu = true;
            audioManager.ToggleMenu(menu);
        }
        panel.SetActive(menu);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        menu = false;
        audioManager.ToggleMenu(menu);
        audioManager.RestartFromMenu();
        panel.SetActive(menu);
        GameManager.StartGame();
    }
    public void MuteSound()
    {
        print("testS");
        audioManager.ToggleMuteSounds();
    }
    public void MuteMusic()
    {
        print("testM");
        audioManager.ToggleMuteMusic();
    }
}