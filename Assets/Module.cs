using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    public float speed;
    public int division;
    public bool selected;
    public float rotationSpeed;
    Vector3 targetRotationEuler;
    
    private void Awake()
    {
        
    }
   public void Init(float speed, float rotationSpeed, int division)
    {
        this.speed = speed;
        this.rotationSpeed = rotationSpeed;
        this.division = division;
    }
    public void Rotate(bool clockwise)
    {
       
        if (clockwise)
        {

            // targetRotationEuler = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 360 / division);
            AddRotation(360 / division);

        }
        // temp.x = temp.x + 360 / division;
        else
        {
            AddRotation((360 / division)*-1);
            
            // targetRotationEuler = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 360 / division);
            // transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 360 / division), rotationSpeed);
        }
           
     
    }
    protected float degreesRemaining = 0F;

    public  float degreesPerSecond = 100F;

    public void AddRotation(float degrees)
    {
        this.degreesRemaining += degrees;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + Vector3.forward * speed * Time.deltaTime;

        if (Mathf.Abs(this.degreesRemaining) > 0F)
        {
            // how much the object should rotate this frame
            // degrees = degrees per second * seconds, and Time.deltaTime is a fraction of a second
            float rotationThisFrame = degreesPerSecond * Time.deltaTime;

            // if object would finish rotating this frame, finish rotating
            if (rotationThisFrame >= Mathf.Abs(degreesRemaining))
            {
                this.transform.Rotate(new Vector3(0F, 0F, degreesRemaining));
                this.degreesRemaining = 0F;
            }
            else // otherwise, rotate the amount necessary and subtract that from the counter
            {
               
                if (degreesRemaining > 0)
                {
                    this.transform.Rotate(new Vector3(0F, 0F, rotationThisFrame));
                    this.degreesRemaining -= rotationThisFrame;
                }
                else
                {
                    this.transform.Rotate(new Vector3(0F, 0F, -rotationThisFrame));
                    degreesRemaining += rotationThisFrame;
                }
            }
        }

    }
}
