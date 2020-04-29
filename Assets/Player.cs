using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        print("collision");
        Module m = collision.gameObject.GetComponentInParent<Module>();
        if (m != null)
        {
            if (!m.isPuny)
            {
                gameManager.Death();
            }
        }
        else
            print("collision fuck");
    }
}
