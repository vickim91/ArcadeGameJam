using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplierDisplay : MonoBehaviour
{
    TextMeshProUGUI tGUI;
    public int multiplier;

    // Start is called before the first frame update
    void Start()
    {
        tGUI = GetComponent<TextMeshProUGUI>();
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        tGUI.text = multiplier.ToString();
    }

    public void StarPower()// here
    {

    }
    public void NoStarPower()// here
    {

    }
    public void UpgradeDifficultyModifier(int difficultyLevel)// here (1,2,3...)
    {

    }
}
