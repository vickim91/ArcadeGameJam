using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

     Animator anim;
    bool dead;
    Vector3 deathDestination;
    public float deathSpeed;
    Transform myTransform;
    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        myTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {

            transform.position = transform.position + deathDestination * deathSpeed * Time.deltaTime;
        }
    }
    public void TriggerDeathAnim()
    {
        dead = true;
        anim.SetTrigger("Death");
        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(0f, 1f);
        float randZ = Random.Range(0f, 1f);
        deathDestination = new Vector3(randX, randY, randZ);
        print(deathDestination);
    }
 
}
