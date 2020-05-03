using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject[] backgrounds;
    public static bool firstLoad = true;

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
        Destroy(GameObject.FindGameObjectWithTag("Background"));
        GameObject go = Instantiate(backgrounds[Random.Range(0, backgrounds.Length)], transform.position, Quaternion.identity) as GameObject;
        go.transform.parent = transform;
    }
}
