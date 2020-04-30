using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    [HideInInspector]
    public float speed;
    private int division;
    [HideInInspector]
    public int type;
    [HideInInspector]
    public int step;
    private bool isPuny;
    AudioManager audioManager;
    ModuleSpawner moduleSpawner;
    private GameObject[] frames;
    GameObject myFrame;
    GameManager gameManager;
    private bool isSelected;
    [HideInInspector]
    public bool isThirdAlignment; 
    [HideInInspector]
    public bool isSecondAlignment; 
    private bool isClearable; 
    private bool hasReachedPlayer;
    private bool rotatingClockwise = true;
    private bool rotatingCounterclockwise = true;
    private bool rotationStop = true;
    private bool rotationVelocityChange;
    [HideInInspector]
    public int thisModSelectionIndex = -1; 
    public float clearableWindowSize;
    public float[] clearableAngles;
    public static int starPowerEndCountdown;
    protected float degreesRemaining = 0F;
    [HideInInspector]
    public float degreesPerSecond = 100F;

    private void Start()
    {
        moduleSpawner = FindObjectOfType<ModuleSpawner>();
        audioManager = FindObjectOfType<AudioManager>();
        frames = moduleSpawner.frames;
    }
    public bool CheckIfClearable()
    {
        bool clearable = false;
        this.isClearable = false;
        for (int i=0; i < clearableAngles.Length; i++)
        {
            Quaternion currentRot = this.transform.rotation;
            Vector3 otherAngleEuler = new Vector3(0f, 0f, clearableAngles[i]);
            Quaternion otherRot = Quaternion.Euler(otherAngleEuler);
            float angle = Quaternion.Angle(currentRot, otherRot);
            if(Mathf.Abs(angle) < clearableWindowSize)
            {
                clearable = true;
                this.isClearable = true;
//                print(this.gameObject.name + " is clearable");
                break;
            }
         
        }
        return clearable;
    }
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    public bool SetAsSecondAllignment()
    {
        bool didSet = false;
        if (!isSecondAlignment)
        {
            isSecondAlignment = true;
            didSet = true;
            //dostuff
        }
        return didSet;
    }
    public bool SetAsThirdAlligment()
    {
        bool didSet = false;
        if (!isThirdAlignment)
        {
            isThirdAlignment = true;
            didSet = true;
            //dostuff
        }
        return didSet;
    }
    public void clearAlignmentStatuses()
    {
        isSecondAlignment = false;
        isThirdAlignment = false;
    }

    private void FixedUpdate()
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
                //fix float imprecision
           
                float fixedAngle = (360 / division) * step;
                if (fixedAngle > 360)
                    fixedAngle = fixedAngle % 360;
              
                this.transform.eulerAngles = new Vector3(0f, 0f, fixedAngle);

                //check if clearable
                CheckIfClearable();

                rotationStop = true;
                rotationVelocityChange = true;
                if (isThirdAlignment)
                {
                    moduleSpawner.TriggerStarPower();
                }
            }
            else // otherwise, rotate the amount necessary and subtract that from the counter
            {
                if (degreesRemaining > 0)
                {
                    this.transform.Rotate(new Vector3(0F, 0F, rotationThisFrame));
                    this.degreesRemaining -= rotationThisFrame;
                    if (rotationStop)
                        rotationVelocityChange = true;
                    if (rotatingCounterclockwise)
                        rotationVelocityChange = true;
                    rotatingCounterclockwise = false;
                    rotatingClockwise = true;
                    rotationStop = false;
                }
                else
                {
                    this.transform.Rotate(new Vector3(0F, 0F, -rotationThisFrame));
                    this.degreesRemaining += rotationThisFrame;
                    if (rotationStop)
                        rotationVelocityChange = true;
                    if (rotatingClockwise)
                        rotationVelocityChange = true;
                    rotatingCounterclockwise = true;
                    rotatingClockwise = false;
                    rotationStop = false;
                }
            }
        }

        PlaySounds();
    }

    private void PlaySounds()
    {
        if (hasReachedPlayer)
        {
        }
        else if (rotationVelocityChange)
        {
            rotationVelocityChange = false;
            if (rotationStop == true)
            {
                audioManager.UpdateEventVolumeDuckingAppliance(thisModSelectionIndex, isSelected);
                audioManager.RotationStop(thisModSelectionIndex);
                if (isThirdAlignment)
                    audioManager.RotationStopThreeAligned();
                else if (isSecondAlignment)
                    audioManager.RotationStopTwoAligned();
                else if (isClearable)
                    audioManager.RotationStopClearable(thisModSelectionIndex);
            }
            else if (rotatingClockwise)
            {
                audioManager.Rotation(thisModSelectionIndex, true);
                audioManager.UpdateLoopVolumeDuckingAppliance(thisModSelectionIndex);
            }
            else if (rotatingCounterclockwise)
            {
                audioManager.Rotation(thisModSelectionIndex, false);
                audioManager.UpdateLoopVolumeDuckingAppliance(thisModSelectionIndex);
            }
        }
    }

    public void HasReachedPlayer()
    {
        hasReachedPlayer = true;
        GetComponentInChildren<Renderer>().materials[1].SetColor("_Color", new Color(0, 1, 0.2f, 0.1f));
        if (isPuny)
        {
            starPowerEndCountdown--;
            audioManager.ObliteratePunyModule();
            if (moduleSpawner.deaccelerationPoint == starPowerEndCountdown)
            {

                print("enter if");
                moduleSpawner.StarPowerDeacceleration();
            }
        }
        else
        {
            if (isClearable)
            {
                //skal nok ændres 
                gameManager.addToScore(100);
            }
            else
            {
                gameManager.Death();
                moduleSpawner.Death();
                this.speed = 0;
                GetComponentInChildren<Renderer>().materials[1].SetColor("_Color", Color.red * 0.5f);
            }
        }
        //audioManager.Rotation(true, thisModSelectionIndex, false);
    }
    public void SelectThisModule()
    {
        isSelected = true;
        if (!isPuny)
            GetComponentInChildren<Renderer>().materials[1].SetColor("_Color", new Color(0f,0.5f,1f,0.5f));
        else
        {
            GetComponentInChildren<Renderer>().materials[1].SetColor("_Color", new Color(1f, 1f, 0f, 0.1f));
        }
    }
    public void UnselectThisModule()
    {
        isSelected = false;
        if(!isPuny)
        GetComponentInChildren<Renderer>().materials[1].SetColor("_Color", new Color(1f, 0f,0f,0.5f));
        else
        {
            GetComponentInChildren<Renderer>().materials[1].SetColor("_Color", new Color(1f, 1f, 0f, 0.1f));
        }
    }

    public void Init(float speed, float rotationSpeed, int division, int initialRotationSteps, bool isPuny )
    {
        Start();
        this.speed = speed;
        this.degreesPerSecond = rotationSpeed;
        this.division = division;
        this.isPuny = isPuny;
        addStep(initialRotationSteps);

        float fixedAngle = (360 / division) * step;
        if (fixedAngle > 360)
            fixedAngle = fixedAngle % 360;

        if (frames[division - 3] != null)
            myFrame = Instantiate(frames[division - 3], this.transform);
        //else
           // print("du har glemt at sætte et prefab til frames");

        this.transform.eulerAngles = new Vector3(0f, 0f, fixedAngle);

        //check is clearable
        CheckIfClearable();

       // transform.Rotate(new Vector3(0f, 0f, (360 / division) * initialRotationSteps));
       
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
            
            int rest = this.step % division;
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
