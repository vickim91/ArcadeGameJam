using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveMeColorsToo : MonoBehaviour
{
    Renderer parentRenderer;
    Renderer thisRenderer;

    // Start is called before the first frame update
    void Start()
    {
        parentRenderer = GetComponentInParent<Renderer>();
        thisRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        parentRenderer.material.GetColor("_Color");
        //thisRenderer.material.SetColor("_Color", new Color()); // LOL virker slet ikke det her
    }
}
