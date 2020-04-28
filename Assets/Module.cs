using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    public float speed;
    public int division;
    public int type;
    public int step;
    public bool isPuny;
    Vector3 targetRotationEuler;
    AudioManager audioManager;

    private bool isSelected; 
    public bool isThirdAlignment; // controlled by modulespawner (tbc)
    public bool isSecondAlignment; // controlled by modulespawner (tbc)
    public bool isClearable; // controlled by modulespawner (tbc)
    private bool hasReachedPlayer; 
    public bool rotatingClockwise;
    public bool rotatingCounterclockwise;
    public bool rotationStop = true;
    bool rotationVelocityChange;
    public int thisModSelectionIndex = -1; // controlled by modulespawner


    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (hasReachedPlayer)
        {
            
        }
        else if (rotationVelocityChange)
        {
            rotationVelocityChange = false;
            if (rotationStop == true)
            {
                audioManager.Rotation(true, thisModSelectionIndex, false);
                audioManager.RotationStop(thisModSelectionIndex);
                if (isThirdAlignment)
                    audioManager.ThreeAligned();
                else if (isSecondAlignment)
                    audioManager.TwoAligned();
                else if (isClearable)
                    audioManager.RotationStopClearable(thisModSelectionIndex);
            }
            if (rotatingClockwise)
            {
                audioManager.Rotation(false, thisModSelectionIndex, true);
                Debug.Log("clockwise:" + thisModSelectionIndex);
            }
            else if (rotatingCounterclockwise)
            {
                audioManager.Rotation(false, thisModSelectionIndex, false);
                Debug.Log("counterclockwise:" + thisModSelectionIndex);
            }
        }


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
                rotationStop = true;
                rotationVelocityChange = true;
            }
            else // otherwise, rotate the amount necessary and subtract that from the counter
            {
                rotationStop = false;
                if (degreesRemaining > 0)
                {
                    this.transform.Rotate(new Vector3(0F, 0F, rotationThisFrame));
                    this.degreesRemaining -= rotationThisFrame;
                    if (rotatingCounterclockwise || rotationStop)
                        rotationVelocityChange = true;
                    rotatingCounterclockwise = false;
                    rotatingClockwise = true;
                    rotationStop = false;
                }
                else
                {
                    this.transform.Rotate(new Vector3(0F, 0F, -rotationThisFrame));
                    degreesRemaining += rotationThisFrame;
                    if (rotatingClockwise || rotationStop)
                        rotationVelocityChange = true;
                    rotatingCounterclockwise = true;
                    rotatingClockwise = false;
                    rotationStop = false;
                }
            }
        }
    }

    public void HasReachedPlayer()
    {
        Debug.Log("reachedPlayer:" + thisModSelectionIndex);
        hasReachedPlayer = true;
        GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        audioManager.Rotation(true, thisModSelectionIndex, false);
    }
    public void SelectThisModule()
    {
        isSelected = true;
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }
    public void UnselectThisModule()
    {
        isSelected = false;
        GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
    }

    public void Init(float speed, float rotationSpeed, int division, int initialRotationSteps, bool isPuny )
    {
        this.speed = speed;
        this.degreesPerSecond = rotationSpeed;
        this.division = division;
        this.isPuny = isPuny;
        addStep(initialRotationSteps);
        
        transform.Rotate(new Vector3(0f, 0f, (360 / division) * initialRotationSteps));
       
    }
    //til superpower
    public void SetPuny(bool isPuny)
    {
        this.isPuny = isPuny;
    }
    public void addStep(int step)
    {
        this.step += step;
        while (this.step > division-1)
        {
            int rest = step % division;
            this.step = rest;
        }
        while (this.step < 0)
        {
            this.step = step + division;
        }
    }



    public void Rotate(bool clockwise, int selectionIndex)
    {
        thisModSelectionIndex = selectionIndex;
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
        if (degrees > 0)
            addStep(1);
        else
            addStep(-1);

        
    }
    public void OnTriggerEnter(Collider other)
    {
      

        if (other.tag == "ModuleDestroyer")
        {
            Destroy(this.gameObject);
        }
    }
}
