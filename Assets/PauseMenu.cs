using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool menu;
    AudioManager audioManager;
    public GameObject panel;
    public GameObject start;
    public GameObject howToPlay;
    public GameObject settings;
    public static bool firstLoad = true;
    DeathScreen deathScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (!firstLoad)
            Destroy(this.gameObject);
        if (firstLoad)
        {
            DontDestroyOnLoad(this);
            firstLoad = false;
        }
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
        deathScreen = FindObjectOfType<DeathScreen>();
        if (panel.activeInHierarchy)
        {
            deathScreen.HideDeathScreenWhenMenu(true);
            Time.timeScale = 1;
            menu = false;
            audioManager.ToggleMenu(menu);
        }
        else
        {
            deathScreen.HideDeathScreenWhenMenu(false);
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
        audioManager.ToggleMuteSounds();
    }
    public void MuteMusic()
    {
        audioManager.ToggleMuteMusic();
    }
}