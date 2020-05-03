using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject[] backgrounds;
    public GameObject starPowerBackground;
    public static bool firstLoad = true;
    int oldIndex;

    private void Start()
    {
        if (!firstLoad)
            Destroy(this.gameObject);
        if (firstLoad)
        {
            DontDestroyOnLoad(this);
            firstLoad = false;
            ChangeBackground();
        }
    }

    public void ChangeBackground()
    {
        oldIndex = Random.Range(0, backgrounds.Length);
        Destroy(GameObject.FindGameObjectWithTag("Background"));
        GameObject go = Instantiate(backgrounds[oldIndex], transform.position, Quaternion.identity) as GameObject;
        go.transform.parent = transform;
        
    }
    public void SetStarPowerBackground()
    {
        Destroy(GameObject.FindGameObjectWithTag("Background"));
        GameObject go = Instantiate(starPowerBackground, transform.position, Quaternion.identity) as GameObject;
        go.transform.parent = transform;

    }
    public void setOldBackground()
    {
     
        Destroy(GameObject.FindGameObjectWithTag("Background"));
        GameObject go = Instantiate(backgrounds[oldIndex], transform.position, Quaternion.identity) as GameObject;
        go.transform.parent = transform;

    }
}
