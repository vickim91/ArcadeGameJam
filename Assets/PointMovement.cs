using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMovement : MonoBehaviour
{
    Vector3 destination;
    float threshold = 12;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(-3, 3), 7, 0); 
        destination = new Vector3(0, threshold + 1, 0);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, destination, 0.05f);
        if (transform.position.y > threshold)
        {
            print("kill");
            Destroy(this.gameObject);
        }
    }
}