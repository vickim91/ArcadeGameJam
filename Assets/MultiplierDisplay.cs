using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplierDisplay : MonoBehaviour
{
    ModuleSpawner moduleSpawner;
    TextMeshProUGUI tGUI;
    public int multiplier;

    // Start is called before the first frame update
    void Start()
    {
        moduleSpawner = FindObjectOfType<ModuleSpawner>();
        tGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        tGUI.text = multiplier.ToString();
        //if (moduleSpawner.starPower)
        //    else
    }
}
