using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject[] backgrounds;

    private void Start()
    {
        ChangeBackground();
    }

    public void ChangeBackground()
    {
        Destroy(GameObject.FindGameObjectWithTag("Background"));
        GameObject go = Instantiate(backgrounds[Random.Range(0, backgrounds.Length)], transform.position, Quaternion.identity) as GameObject;
        go.transform.parent = transform;
    }
}
