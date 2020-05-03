using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DeathScreen : MonoBehaviour
{
    TextMeshProUGUI text;
    GameManager gm;
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        panel.SetActive(false);
    }
    public void ShowDeathScreen(bool show)
    {


        panel.SetActive(show);            
        text.text = "Your score: " + gm.score.ToString() + "\n" + "Personal best: " + GameManager.scoreBest.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
