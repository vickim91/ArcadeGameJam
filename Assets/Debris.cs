using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    
    public Transform center;
    Vector3 randomRotation;
    public float velocity = 5;
    float angle = 1;
    float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {

        randomRotation = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        rotationSpeed = Random.Range(100f, 300f);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 direction = transform.position - center.position;
        direction.Normalize();
        transform.position = transform.position + direction * Time.deltaTime * 5;


        transform.Rotate(randomRotation, Time.deltaTime * rotationSpeed);

        print(angle);
    }

  
}
