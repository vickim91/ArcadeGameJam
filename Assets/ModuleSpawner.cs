using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class ModuleSpawner : MonoBehaviour
{
    Background background;
    Animator playerAnim;
    public ScrollingTexture scroll1;
    public ScrollingTexture scroll2;
    Renderer scrollRend1;
    Renderer scrollRend2;
    Color originalColor1;
    Color originalColor2;

    GameManager gameManager;
    AudioManager audioManager;
    LevelDesigner levelDesigner;
    LevelDesigner.ModuleSpawn[] modSpawnParams;
    LevelDesigner.SequenceSpawn[] seqSpawnParams;
    
    private int[] modSpawnPossibilities; 
    private int[] seqSpawnPossibilities; 

    private GameObject[] currentSelectables; 
    private Module[] currentSelectablesScript; 
    public GameObject[] spawnedMods; // this does not include modules that have reached the player
    private Vector2[] spawnQueue; 
    private int spawnedModsIndex;
    public static int selectedModIndex; 
    private Module selectedModule;

    // behavior parameters
    public GameObject[] frames;
    public GameObject[] modules;
    [HideInInspector]
    public int queueLength; // note: set this to the length of the longest sequence at start
    public int numberOfSelectableMods;
    public int maxNumOfModsInGame;

    //public int playerPosition; // replace with real player object, maybe?
    public Transform playerPosition;
    //speed variables
    public float acceleration;
    private float gameSpeedRemaining;
    private float spawnRateRemaining;
    private float rotationSpeedRemaining;
    public float initialGameSpeed;
    public float initialSpawnRate;
    public float initialRotationSpeed;

    private int div;
    private int divFree = 360;
    private float autoSpawnTimer;
    private int moduleNumber;
    private string spawnNameModOrSeq;

    public float gameSpeed;
    private float spawnRate;
    private float rotationSpeed;

    public float speedAccumulator;
    public float spawnRateAccumulator;
    public float starPowGameSpeed;
    public float starPowSpawnrate;
    public float starPowRotSpeed;
    public float starPowAcc;
    public float starPowDeacc;
    public int punyModsPerStarPower;
    public int deaccelerationPoint;
    public int preDeaccelerationPoint;
    private int punyModsCounter;
    public int difficultyLevel;
    public float debugSpawnPositioning;

    // level design tools:
    public int divisionStep = 4;
    private bool starPower;

    public int divisionStepInterval;

    void Start()
    {
        background = GameObject.FindObjectOfType<Background>();
        playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        divisionStepChangeCountdown = divisionStepInterval;
        gameSpeed = initialGameSpeed;
        spawnRate = initialSpawnRate;
        rotationSpeed = initialRotationSpeed;
        levelDesigner = GetComponent<LevelDesigner>();
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        currentSelectables = new GameObject[numberOfSelectableMods];
        currentSelectablesScript = new Module[numberOfSelectableMods];
        spawnedMods = new GameObject[maxNumOfModsInGame]; // these does not include spawned modules that have reached the player
        spawnedModsIndex = -1;
        selectedModIndex = 0;
        queueLength = levelDesigner.queueLength;
        spawnQueue = new Vector2[queueLength];
        for (int i = 0; i < spawnQueue.Length; i++)
        {
            spawnQueue[i] = new Vector2(-1, -1);
        }
        SetProbabilities();

                //initial boost
            SetSpeed(initialGameSpeed * 10, 400, initialSpawnRate * 10, initialRotationSpeed);

        setMultiplier();
        
    }

    void Update()
    {
        CheckIfModuleHasReachedThePlayer();
        AutoSpawn();
        InputMethodsForTesting();
        UpdateSpeed();
        if (starPower)
        {
            gameManager.starPowerMultiplier = 2;
        }
        else
        {
            gameManager.starPowerMultiplier = 1;
        }
    }
    private void UpdateSpeed()
    {
        //game speed 
        if(Mathf.Abs(gameSpeedRemaining) >0f )
        {
            float gameSpeedChangeThisFrame = acceleration * Time.deltaTime;
            if(gameSpeedChangeThisFrame >= Mathf.Abs(gameSpeedRemaining)){
                gameSpeed += gameSpeedRemaining;
                gameSpeedRemaining = 0f;
            
            }
            else
            {
                //positive acceleration
                if(gameSpeedRemaining > 0f)
                {
                    gameSpeed += gameSpeedChangeThisFrame;
                    gameSpeedRemaining -= gameSpeedChangeThisFrame;
                }
                //negative acceleration
                else if(gameSpeedRemaining < 0f)
                {
                    gameSpeed -= gameSpeedChangeThisFrame;
                    gameSpeedRemaining += gameSpeedChangeThisFrame;
                }
            }
            foreach (GameObject g in spawnedMods)
            {
                if (g)
                {
                    Module m = g.GetComponent<Module>();
                    m.speed = gameSpeed;

                }
            }
        }
        //spawn speed
        if (Mathf.Abs(spawnRateRemaining) > 0f)
        {
            float spawnRateChangeThisFrame = acceleration * Time.deltaTime;
            if (spawnRateChangeThisFrame >= Mathf.Abs(spawnRateRemaining)){
                spawnRate += spawnRateRemaining;
                spawnRateRemaining = 0f;
              
            }
            else
            {
                //positive accel
                if(spawnRateRemaining > 0f)
                {
                    spawnRate += spawnRateChangeThisFrame;
                    spawnRateRemaining -= spawnRateChangeThisFrame;
                }
                //negative accel
                else if(spawnRateRemaining < 0f)
                {
                    spawnRate -= spawnRateChangeThisFrame;
                    spawnRateRemaining += spawnRateChangeThisFrame;
                }
            }
        }
        //rotation speed
        if(Mathf.Abs(rotationSpeedRemaining) > 0f)
        {
            //100 for at match scale med de andre speeds
            float rotationChangeThisFrame = acceleration * 100 * Time.deltaTime;
            if(rotationChangeThisFrame >= Mathf.Abs(rotationSpeedRemaining) || Mathf.Abs( rotationSpeedRemaining) < 1f)
            {
                rotationSpeed += rotationSpeedRemaining;
                rotationSpeedRemaining = 0f;
            }
            //positive accel
            if(rotationSpeedRemaining > 0f)
            {
                rotationSpeed += rotationChangeThisFrame;
                rotationSpeedRemaining -= rotationChangeThisFrame;
            }
            //negative accel
            else if(rotationSpeedRemaining < 0f)
            {
                rotationSpeed -= rotationChangeThisFrame;
                rotationSpeedRemaining += rotationChangeThisFrame;
            }
            foreach (GameObject g in spawnedMods)
            {
                if (g)
                {
                    Module m = g.GetComponent<Module>();
                    m.degreesPerSecond = rotationSpeed;
                }
            }
        }
    }

    public void SetProbabilities() // run this, when divisionStep changes!!
    {
        int totalModSpawnProbabilities = 0;
        modSpawnParams = levelDesigner.moduleSpawn;
        foreach (LevelDesigner.ModuleSpawn sM in modSpawnParams)
        {
            for (int i = 0; i < sM.theREALDivApplication.Length; i++)
            {
                if (sM.theREALDivApplication[i] == divisionStep)
                {
                    float modSpawnProbability = sM.modTypeRotProb.z;
                    totalModSpawnProbabilities += Mathf.RoundToInt(modSpawnProbability);
                }
            }
        }
        int totalSeqSpawnProbabilities = 0;
        seqSpawnParams = levelDesigner.sequenceSpawn;
        foreach (LevelDesigner.SequenceSpawn sE in seqSpawnParams)
        {
            for (int i = 0; i < sE.theREALDivApplication.Length; i++)
            {
                if (sE.theREALDivApplication[i] == divisionStep)
                {
                    float seqSpawnProbability = sE.probality;
                    totalSeqSpawnProbabilities += Mathf.RoundToInt(seqSpawnProbability);
                }
            }
        }
        // possibilities: this array includes the spawnID's for all spawns included by the divisionStep
        // its length is the sum of each spawnID's probability
        // spawnID is not the same as a modType
        // the spawnIndex is the index for the spawnID's probability, used to populate the possibilites with spawnID's
        modSpawnPossibilities = new int[totalModSpawnProbabilities];
        seqSpawnPossibilities = new int[totalSeqSpawnProbabilities];

        int modSpawnID = 0;
        int firstModSpawnIndex = 0;
        int lastModSpawnIndex = 0;
        foreach (LevelDesigner.ModuleSpawn sM in modSpawnParams)
        {
            for (int e = 0; e < sM.theREALDivApplication.Length; e++)
            {
                if (sM.theREALDivApplication[e] == divisionStep)
                {
                    float thisModSpawnProbability = sM.modTypeRotProb.z;
                    lastModSpawnIndex += Mathf.RoundToInt(thisModSpawnProbability);

                    for (int i = firstModSpawnIndex; i < lastModSpawnIndex; i++)
                    {
                        modSpawnPossibilities[i] = modSpawnID;
                    }
                    firstModSpawnIndex = lastModSpawnIndex;
                }
            }
            modSpawnID++;
        }
        int seqSpawnID = 0;
        int firstSeqSpawnIndex = 0;
        int lastSeqSpawnIndex = 0;
        foreach (LevelDesigner.SequenceSpawn sE in seqSpawnParams)
        {
            for (int e = 0; e < sE.theREALDivApplication.Length; e++)
            {
                if (sE.theREALDivApplication[e] == divisionStep)
                {
                    float thisSeqSpawnProbability = sE.probality;
                    lastSeqSpawnIndex += Mathf.RoundToInt(thisSeqSpawnProbability);

                    for (int i = firstSeqSpawnIndex; i < lastSeqSpawnIndex; i++)
                    {
                        seqSpawnPossibilities[i] = seqSpawnID;
                    }
                    firstSeqSpawnIndex = lastSeqSpawnIndex;
                }
            }
            seqSpawnID++;
        }
    }

    private void PrepareModuleThenSpawn()
    {
        div = divisionStep;
        if (levelDesigner.snapToDiv == false)
            div = divFree;
        UpdateDivisionChangeCountdown();
        if (spawnQueue[0].x == -1)
        {
            cueIsEmpty = true;
            CheckForDivisionChange();
            int rollSeqSpawnVsModSpawn = Random.Range(0, 100);
            if (rollSeqSpawnVsModSpawn < levelDesigner.chanceForSequence)
            {
                int rollSeqSpawnID = Random.Range(0, seqSpawnPossibilities.Length);
                int seqSpawnID = seqSpawnPossibilities[rollSeqSpawnID];
                LevelDesigner.SequenceSpawn thisSeqSpawn = seqSpawnParams[seqSpawnID];
                //thisSequnce.seqTypeRot.Length må ikke være over queueLength
                for (int seqSpawnElemID = 0; seqSpawnElemID < thisSeqSpawn.seqTypeRot.Length; seqSpawnElemID++)
                {
                    int seqSpawnElemModType = Mathf.RoundToInt(thisSeqSpawn.seqTypeRot[seqSpawnElemID].x);
                    if (levelDesigner.CheckIfSpawnRotationIsZero(true, seqSpawnID, seqSpawnElemID))
                    {
                        thisSeqSpawn.seqTypeRot[seqSpawnElemID].y = Random.Range(0, 360);
                    }
                    int seqSpawnElemRotation = Mathf.RoundToInt(thisSeqSpawn.seqTypeRot[seqSpawnElemID].y / (360 / div));

                    spawnQueue[seqSpawnElemID] = new Vector2(seqSpawnElemModType, seqSpawnElemRotation); //here
                    spawnNameModOrSeq = "S" + seqSpawnID + " ";
                }
            }
            else
            {
                int rollModSpawnID = Random.Range(0, modSpawnPossibilities.Length);
                int modSpawnID = modSpawnPossibilities[rollModSpawnID];
                int modSpawnModType = Mathf.RoundToInt(modSpawnParams[modSpawnID].modTypeRotProb.x);
                if (levelDesigner.CheckIfSpawnRotationIsZero(false, -1, modSpawnID))
                {
                    modSpawnParams[modSpawnID].modTypeRotProb.y = Random.Range(0, 360);
                }
                int modSpawnRotation = Mathf.RoundToInt(modSpawnParams[modSpawnID].modTypeRotProb.y / (360 / div));

                spawnQueue[0] = new Vector2(modSpawnModType, modSpawnRotation); //here
                spawnNameModOrSeq = "M" + modSpawnID + " ";
            }
        }
        else
            cueIsEmpty = false;

        //Starpower
        bool spawnAsPuny = false;
        if (punyModsCounter > preDeaccelerationPoint)
            spawnAsPuny = true;


        int spawnID = Mathf.RoundToInt(spawnQueue[0].x);
        int moduleType = spawnID;
        int moduleRotation = Mathf.RoundToInt(spawnQueue[0].y);
        spawnedModsIndex++;
        spawnedMods[spawnedModsIndex] = SpawnModule(gameSpeed, rotationSpeed, div, moduleRotation, moduleType, spawnAsPuny);
        spawnedMods[spawnedModsIndex].name = spawnNameModOrSeq + spawnedMods[spawnedModsIndex].name + moduleNumber.ToString();
        moduleNumber++;
        //starpower count down
        punyModsCounter--;
        ////trigger PRE deaccelleration if relevant
        if (punyModsCounter == preDeaccelerationPoint)
        {
            StarPowerPreDeacceleration();
        }

        //ryk køen
        for (int i = 0; i < spawnQueue.Length - 1; i++)
        {
            spawnQueue[i] = spawnQueue[i + 1];
        }

        //gør det sidste element "tomt" hvis vi nu er i en sequence
        spawnQueue[spawnQueue.Length - 1] = new Vector2(-1, -1);

        for (int i = 0; i < numberOfSelectableMods; i++)
        {
            currentSelectables[i] = spawnedMods[i];
            if (currentSelectables[i] != null)
            {
                currentSelectablesScript[i] = currentSelectables[i].GetComponent<Module>();
                currentSelectablesScript[i].thisModSelectionIndex = i;
            }
        }

        if (selectedModule == null)
        {
            SetSelectedModule(spawnedModsIndex);
        }
    }

    private void UpdateDivisionChangeCountdown()
    {
        if (divisionStepChangeCountdown > 0)
        {
            divisionStepChangeCountdown--;
        }
        else if (divisionStepChangeCountdown == 0)
        {
            readyForDivisionStepChange = true;
        }
    }

    public void Death()
    {
        if (!gameManager.godMode)
            SetSpeed(0, 1000, 0, 0);
    }

    void CheckForDivisionChange()
    {
        if (readyForDivisionStepChange && cueIsEmpty)
        {
            if (starPower == false)
            {
                DivisionStepChange();
                divisionStepChangeCountdown = divisionStepInterval;
                readyForDivisionStepChange = false;
            }
        }
    }
    private int divisionStepChangeCountdown;
    private bool readyForDivisionStepChange;
    private bool cueIsEmpty;

    public GameObject SpawnModule(float speed, float rotationSpeed, int division, int initialRotationSteps, int moduleType, bool spawnAsPuny)
    {
        return gameObject.Instantiate(modules[moduleType], transform.position, modules[moduleType].transform.rotation, transform, speed, rotationSpeed, division, initialRotationSteps, spawnAsPuny);
    }
    //next div og speed increases?
    public float postStarPosition;
    public void IncreaseDifficulty()
    {
        difficultyLevel++;
        SetSpeed(initialGameSpeed + speedAccumulator *difficultyLevel, 400, initialSpawnRate + spawnRateAccumulator*difficultyLevel, initialRotationSpeed);
        postStarPosition -= difficultyLevel/4;
//        debugSpawnPositioning -= difficultyLevel/4;
        setMultiplier();

    }

    private bool godModeStart = true;
    private void CheckIfModuleHasReachedThePlayer()
    {
        if (currentSelectables[0] != null)
        {
            float positionOfClosestModule = currentSelectables[0].transform.position.z;
            if ( godModeStart)
            {
                if (positionOfClosestModule > debugSpawnPositioning)
                {
                    // SetSpeed(initialGameSpeed, 400, initialSpawnRate, initialRotationSpeed);
                    IncreaseDifficulty();
                    godModeStart = false;
                    SetSelectedModule(0);
                }
            }
            if (positionOfClosestModule > playerPosition.position.z)
            {
                //add score. 100 gange game speed for now . magic numbers men det er vel ligegyldigt
                //er flyttet til modules
               // gameManager.addToScore(Mathf.RoundToInt(gameSpeed * 100));
                currentSelectablesScript[0].HasReachedPlayer();
                audioManager.ShiftRotationVoices();
                
                for (int i = 0; i < maxNumOfModsInGame - 1; i++)
                {
                    spawnedMods[i] = spawnedMods[i + 1];
                }
                spawnedMods[maxNumOfModsInGame - 1] = null;
                if (spawnedModsIndex > -1)
                    spawnedModsIndex--;

                for (int i = 0; i < numberOfSelectableMods; i++)
                {
                    currentSelectables[i] = spawnedMods[i];
                    if (currentSelectables[i] != null)
                    {
                        currentSelectablesScript[i] = currentSelectables[i].GetComponent<Module>();
                        currentSelectablesScript[i].thisModSelectionIndex = i;
                    }
                }
                if (selectedModIndex < 0)
                {
                    Debug.Log("hey, does this ever happen?");
                    selectedModIndex = 0;
                }
                if (selectedModIndex > 0)
                    selectedModIndex--;
                SetSelectedModule(selectedModIndex);
            }
        }
    }

    private void AutoSpawn()
    {
        if (spawnRate > 0)
        {
            autoSpawnTimer += Time.deltaTime;
            if (autoSpawnTimer > 1 / spawnRate)
            {
                PrepareModuleThenSpawn();
                autoSpawnTimer = 0;
            }
        }
    }

    private void InputMethodsForTesting()
    {
        //change speed test
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //        //star power parameters
        //    SetSpeed(starPowGameSpeed, starPowAcc, starPowSpawnrate, starPowRotSpeed);
        //    TriggerStarPower();
        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    SetSpeed(initialGameSpeed, starPowDeacc, initialSpawnRate, initialRotationSpeed);
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    PrepareModuleThenSpawn();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //    DivisionStepChange();
    }

    private void DivisionStepChange()
    {
        divisionStep++;
        if (divisionStep < 3)
            divisionStep = 3; // we don't go below three!! that's forbidden!
        if (divisionStep == 7)
            divisionStep = 8; // and btw, 7 is also forbidden...
        if (divisionStep > 10)
            divisionStep = 10;
        //score multiplier
        setMultiplier();
        SetProbabilities();
        background.ChangeBackground();
    }
    public void setMultiplier()
    {
        gameManager.setDifficultyMultiplier(difficultyLevel);
    }

    public void SelectNextModule()
    {
        selectedModIndex++;
        if (selectedModIndex > numberOfSelectableMods - 1)
        {
            selectedModIndex = numberOfSelectableMods - 1;
        }
        else
            audioManager.SelectNextModule(selectedModIndex);
        SetSelectedModule(selectedModIndex);
    }

    public void SelectPreviousModule()
    {
        selectedModIndex--;
        if (selectedModIndex < 0)
            selectedModIndex = 0;
        else
            audioManager.SelectPrevMod(selectedModIndex);
        SetSelectedModule(selectedModIndex);
    }

    public void RotateSelectedModuleCounterclockwise()
    {
        if (selectedModule)
        {
            audioManager.UpdateLoopVolumeDuckingAppliance(selectedModIndex);
            audioManager.RotationCue(false, false, selectedModIndex);
            selectedModule.Rotate(false, selectedModIndex);
            CheckForLineup();
        }
    }

    public void RotateSelectedModuleClockwise()
    {
        if (selectedModule)
        {
            audioManager.UpdateLoopVolumeDuckingAppliance(selectedModIndex);
            audioManager.RotationCue(false, true, selectedModIndex);
            selectedModule.Rotate(true, selectedModIndex);
            CheckForLineup();
        }
    }

    private void SetSelectedModule(int index)
    {
        if(currentSelectables[index] != null)
        {
//            selectedModule = currentSelectables[index].GetComponent<Module>();
            selectedModule = currentSelectablesScript[index];
            selectedModule.SelectThisModule();
            for (int i = 0; i < numberOfSelectableMods; i++)
            {
                if (i != index && currentSelectablesScript[i] != null)
                    currentSelectablesScript[i].UnselectThisModule();
            }
        }
    }

    //brug til acceleration /deAcceleration
    public void SetSpeed(float targetGameSpeed, float acceleration, float targetSpawnRate, float targetRotationSpeed)
    {
       
        gameSpeedRemaining = targetGameSpeed - this.gameSpeed;
        spawnRateRemaining = targetSpawnRate - this.spawnRate;
        rotationSpeedRemaining = targetRotationSpeed - this.rotationSpeed;
        this.acceleration = acceleration;

    }

    public bool CheckForLineup()
    {
        bool lineUp = false;
        int lineUpCount = 0;
        int lastType = -1;
        int lastStep = -1;
        int currentIndex = 0;
        foreach (GameObject g in currentSelectables)
        {
            if (g)
            {
                Module m = g.GetComponent<Module>();
                if (lastType != -1 && lastStep != -1)
                {
                    if (m.type == lastType && m.step == lastStep)
                    {
                        lineUpCount++;
                        if(lineUpCount ==2)
                        {
                            lineUp = true;
                            ThreeAligned(currentIndex);
                            break;
                        }
                        // hvis det kun er 2 der liner op
                        else if (lineUpCount == 1)
                        {
                            TwoAligned(currentIndex);
                        }
                     
                    }
                    else
                    {
                        lineUpCount = 0;
                        //clear previous allignment state
                        NotAligned(currentIndex);
                    }
                }
                lastType = m.type;
                lastStep = m.step;
               
            }
            currentIndex++;
        }
        return lineUp;
    }
    public void NotAligned(int index)
    {
        Module thisModule = currentSelectables[index].GetComponent<Module>();
        thisModule.clearAlignmentStatuses();
    }
    public void TwoAligned(int indexForSecondModuleInAlignment)
    {
        Module secondModule = currentSelectablesScript[indexForSecondModuleInAlignment];
        if (!secondModule.isSecondAlignment)
        {
            secondModule.SetAsSecondAllignment();
        }
    }
    public void ThreeAligned(int indexForThirdModuleInAligment)
    {
        Module thirdModule = currentSelectablesScript[indexForThirdModuleInAligment];
        if (!thirdModule.isThirdAlignment)
        {
            thirdModule.SetAsThirdAlligment();
        }
    }

    public Light backgroundLight;
    public float intensity;
    public float defaultIntesity;
    public void TriggerStarPower()
    {
        if (!starPower)
        {
            //hard codet for ikke at clogge inspector
            playerAnim.SetTrigger("Starpower");

            //scroll1.scrollSpeed = -0.003f;
            //scroll1.scrollSpeed = -0.003f;
            //scroll2.scrollSpeed = -0.02f;
            //scroll2.scrollSpeed2 = -0.02f;
            //scroll1.setTexture(1);
            //scroll2.setTexture(1);
            //scrollRend1 = scroll1.gameObject.GetComponent<Renderer>();
            //scrollRend2 = scroll2.gameObject.GetComponent<Renderer>();
            //originalColor1 = scrollRend1.material.color;
            //originalColor2 = scrollRend2.material.color;
            //scrollRend1.material.color = new Color(0.8f, 0.8f, 1f) ;
            //scrollRend2.material.color = new Color(0.8f, 0.8f, 1f);
            SetSpeed(starPowGameSpeed, starPowAcc, starPowSpawnrate, starPowRotSpeed);
            audioManager.StarPower();
            punyModsCounter = punyModsPerStarPower;
            Module.starPowerEndCountdown = punyModsPerStarPower;
            foreach (GameObject g in spawnedMods)
            {
                if (g && punyModsCounter > 0)
                {
                    Module m = g.GetComponent<Module>();
                    m.SetPuny(true);
                    punyModsCounter--;
                }
            }
            starPower = true;
        }
        backgroundLight.intensity = intensity;
    }

    public void ConcludeStarPower()
    {
        
    }
    public void StarPowerPreDeacceleration()
    {
       // print("pre deacceleration event");
        SetSpeed(starPowGameSpeed, starPowAcc, 0f, starPowRotSpeed);
        

    }

    public float postStarReset;

    public void StarPowerDeacceleration()
    {
        //  print("deacceleration event");

        //SetSpeed(initialGameSpeed, starPowDeacc, initialSpawnRate , initialRotationSpeed);

        //visuals
        playerAnim.SetTrigger("Deaccelerate");
        //scroll1.scrollSpeed = -0.001f;
        //scroll1.scrollSpeed = -0.001f;
        //scroll2.scrollSpeed = -0.01f;
        //scroll2.scrollSpeed2 = -0.01f;
        //scroll1.setTexture(0);
        //scroll2.setTexture(0);
        //scrollRend1.material.color = originalColor1;
        //scrollRend2.material.color = originalColor2;
        backgroundLight.intensity = defaultIntesity;

        audioManager.DeactivateStarPower();
        godModeStart = true;
        starPower = false;
        SetSpeed(initialGameSpeed * postStarReset, 1000, initialSpawnRate * postStarReset, initialRotationSpeed);
    }
}