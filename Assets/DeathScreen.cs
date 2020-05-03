using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DeathScreen : MonoBehaviour
{
    TextMeshProUGUI text;
    GameManager gm;
    public GameObject panel;
    public bool playerIsDead;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        panel.SetActive(false);
    }
    public void ShowDeathScreen(bool show)
    {
        playerIsDead = show;
        DeathScreenDisplay(show);
    }
    private void DeathScreenDisplay(bool show)
    {
        panel.SetActive(show);
        text.text = "Your score: " + gm.score.ToString() + "\n\n\n" + "Personal best: " + GameManager.scoreBest.ToString() + "\n\n\n Post your highscore in the comments!";


    }

    public void HideDeathScreenWhenMenu(bool show)
    {
        if (playerIsDead && show)
            DeathScreenDisplay(show);
        if (playerIsDead && !show)
            DeathScreenDisplay(show);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
