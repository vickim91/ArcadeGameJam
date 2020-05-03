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
        tGUI.text = "X" + multiplier.ToString();
        SizingMethod();
    }

    public void StarPower()// here
    {
        StartCoroutine(flashCoroutine());
    }
    public void NoStarPower()// here
    {
        StopAllCoroutines();
        tGUI.color = new Color(1, 1, 1, 1);
    }

    float fontSizingSpeed = 2f;
    float fontSizeMax = 50f;
    float fontSizeNormal = 47f;
    bool isSizing;
    bool isShrinking;
    public void UpgradeDifficultyModifier(int difficultyLevel)// here (1,2,3...)
    {
        if (!isSizing)
        {
            isSizing = true;
        }
    }
    private void SizingMethod()
    {
        if (isShrinking)
        {
            if (tGUI.fontSize > fontSizeNormal)
            {
                tGUI.fontSize -= fontSizingSpeed;
            }
            else
            {
                isSizing = false;
                isShrinking = false;
            }
        }
        else if (isSizing)
        {
            if (tGUI.fontSize < fontSizeMax)
            {
                tGUI.fontSize += fontSizingSpeed * 2;
            }
            else
                isShrinking = true;
        }
    }

    IEnumerator flashCoroutine()
    {
        Color color1 = new Color(100f, 50f, 0f);
        Color color2 = new Color(0, 100f, 100f);
        Color color3 = new Color(100f, 0f, 100f);
        for (int i = 0; i < 30; i++)
        {
            tGUI.color = color1;
            yield return new WaitForSeconds(0.05f);
            tGUI.color = color2;
            yield return new WaitForSeconds(0.05f);
            tGUI.color = color3;
            yield return new WaitForSeconds(0.05f);

        }
    }

}
