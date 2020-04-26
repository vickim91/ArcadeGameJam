using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class ModuleSpawner : MonoBehaviour
{
    GameManager gameManager;
    LevelDesigner levelDesigner;
    LevelDesigner.ModuleSpawn[] modSpawnParams;
    LevelDesigner.SequenceSpawn[] seqSpawnParams;
    
    public int[] modSpawnProbabilities; // read
    public int[] seqSpawnProbabilities; // read

    public GameObject[] currentSelectables; // read
    public GameObject[] spawnedMods; // read // this does not include module that has reached the player
    public Vector2[] spawnQueue; // read
    private int spawnedModsIndex;
    public int selectedModIndex; // read
    public Module selectedModule; // read
    public float positionOfClosestModule; // read

    // behavior parameters
    public GameObject[] modules; 
    public int queueLength; // note: set this to the length of the longest sequence at start
    public int numberOfSelectableMods;
    public int maxNumOfModsInGame;
    public int lightSpeedCounter;
    public int initialLightSpeedCounter;
    public int deAccellerationStartPoint;
    public float acceleration;
    public int playerPosition;
    // level design tools:
    public int divisionStep;
    public float gameSpeed;
    public float spawnRate;
    public float rotationSpeed;

    private int div;
    private int divFree = 360;
    private float autoSpawnTimer;
    private int moduleNumber;
    private string spawnNameModOrSeq;
    
    /*
     * naming convention: a mod(module) is not the same as a modSpawn(moduleSpawn)
         * a mod exists in the game, deriving from a modSpawn or seqSpawn.
         * a modSpawn is an instruction, that does not apply for sequences.
         * a seqSpawn is an instruction, that hosts an array of sequence elements
         * a seqElem is an instruction similar to modSpawn
         * so, what is a sequence? maybe a sequence could be a memory bank, that keeps track of the seqSpawn...
     */

    void Start()
    {
        levelDesigner = GetComponent<LevelDesigner>();
        gameManager = FindObjectOfType<GameManager>();
        currentSelectables = new GameObject[numberOfSelectableMods];
        spawnedMods = new GameObject[maxNumOfModsInGame]; // these does not include spawned modules that have reached the player
        spawnedModsIndex = -1;
        selectedModIndex = 0;
        spawnQueue = new Vector2[queueLength];
        for (int i = 0; i < spawnQueue.Length; i++)
        {
            spawnQueue[i] = new Vector2(-1, -1);
        }
        SetProbabilities();
    }

    void Update()
    {
        CheckIfModuleHasReachedThePlayer();
        AutoSpawn();
        InputMethods();
    }

    public void SetProbabilities()
    {
        int totalModSpawnProbs = 0;
        modSpawnParams = levelDesigner.moduleSpawn;
        foreach (LevelDesigner.ModuleSpawn sM in modSpawnParams)
        {
            if (sM.divApplication[divisionStep])
            {
                float modSpawnProb = sM.modTypeRotProb.z;
                totalModSpawnProbs += Mathf.RoundToInt(modSpawnProb);
            }
        }
        int totalSeqSpawnProbs = 0;
        seqSpawnParams = levelDesigner.sequenceSpawn;
        foreach (LevelDesigner.SequenceSpawn sE in seqSpawnParams)
        {
            if (sE.divApplication[divisionStep])
            {
                float seqSpawnProb = sE.probality;
                totalSeqSpawnProbs += Mathf.RoundToInt(seqSpawnProb);
            }
        }
        modSpawnProbabilities = new int[totalModSpawnProbs];  // this number represents the spawn module ID (not its type). its size represents the pool of possibilities
        seqSpawnProbabilities = new int[totalSeqSpawnProbs];

        int modSpawnID = 0;
        int firstModSpawnIndex = 0;
        int lastModSpawnIndex = 0;
        foreach (LevelDesigner.ModuleSpawn sM in modSpawnParams)
        {
            if (sM.divApplication[divisionStep])
            {
                float thisModSpawnProbability = sM.modTypeRotProb.z;
                lastModSpawnIndex += Mathf.RoundToInt(thisModSpawnProbability);

                for (int i = firstModSpawnIndex; i < lastModSpawnIndex; i++)
                {
                    modSpawnProbabilities[i] = modSpawnID;
                }
                modSpawnID++;
                firstModSpawnIndex = lastModSpawnIndex;
            }
        }
        int seqSpawnID = 0;
        int firstSeqSpawnIndex = 0;
        int lastSeqSpawnIndex = 0;
        foreach (LevelDesigner.SequenceSpawn sE in seqSpawnParams)
        {
            if (sE.divApplication[divisionStep])
            {
                float thisSeqSpawnProbability = sE.probality;
                lastSeqSpawnIndex += Mathf.RoundToInt(thisSeqSpawnProbability);

                for (int i = firstSeqSpawnIndex; i < lastSeqSpawnIndex; i++)
                {
                    seqSpawnProbabilities[i] = seqSpawnID;
                }
                seqSpawnID++;
                firstSeqSpawnIndex = lastSeqSpawnIndex;
            }
        }
    }

    private void CheckIfModuleHasReachedThePlayer()
    {
        if (currentSelectables[0] != null)
        {
            positionOfClosestModule = currentSelectables[0].transform.position.z;
            if (currentSelectables[0].transform.position.z > playerPosition)
            {
                //add score. 100 gange game speed for now . magic numbers men det er vel ligegyldigt
                gameManager.addToScore(Mathf.RoundToInt(gameSpeed * 100));
                currentSelectables[0].GetComponent<Renderer>().material.SetColor("_Color", Color.white);
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
        if (autoSpawnTimer > 0)
        {
            autoSpawnTimer += Time.deltaTime;
            if (autoSpawnTimer > 1 / spawnRate)
            {
                PrepareModuleThenSpawn();
                autoSpawnTimer = 0;
            }
        }
    }

    private void InputMethods()
    {
        //change speed test
        if (Input.GetKeyDown(KeyCode.P))
        {
                //star power parameters
            SetSpeed(starPowGameSpeed, starPowAcc, starPowSpawnrate, starPowRotSpeed);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetSpeed(gameSpeed, starPowDeacc, spawnRate, rotationSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PrepareModuleThenSpawn();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedModIndex++;
            if (selectedModIndex > numberOfSelectableMods - 1)
            {
                selectedModIndex = numberOfSelectableMods - 1;
            }
            SetSelectedModule(selectedModIndex);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedModIndex--;
            if (selectedModIndex < 0)
                selectedModIndex = 0;
            SetSelectedModule(selectedModIndex);
        }
        //rotér mod uret
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedModule)
            {
                selectedModule.Rotate(false);
                CheckForLineup();
            }
        }
        //rotér med uret
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedModule)
            {
                selectedModule.Rotate(true);
                CheckForLineup();
            }
        }
    }

    void SetSelectedModule(int index)
    {
        if(currentSelectables[index] != null)
        {
            if (selectedModule != null)
            {
                ColorTheUnselectedSelectables();
            }
            selectedModule = currentSelectables[index].GetComponent<Module>();
            selectedModule.GetComponent<Renderer>().material.SetColor("_Color", Color.red);                
        }
    }

    private void ColorTheUnselectedSelectables()
    {
        for (int i = 0; i < numberOfSelectableMods; i++)
        {
            if (currentSelectables[i] != null && currentSelectables[i].transform.position.z < playerPosition )
                currentSelectables[i].GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
    }

    private void PrepareModuleThenSpawn()
    {
        div = levelDesigner.divisionStepSequence[divisionStep];
        if (levelDesigner.snapToDiv == false)
            div = divFree;


        if (spawnQueue[0].x == -1)
        {
            int rollSeqSpawnVsModSpawn = Random.Range(0, 100);
            if (rollSeqSpawnVsModSpawn <= levelDesigner.chanceForSequence)
            {
                int rollSeqSpawnID = Random.Range(0, seqSpawnProbabilities.Length);
                int seqSpawnID = seqSpawnProbabilities[rollSeqSpawnID];
                LevelDesigner.SequenceSpawn thisSeqSpawn = seqSpawnParams[seqSpawnID];
                //thisSequnce.seqTypeRot.Length må ikke være over queueLength
                for (int seqSpawnElem = 0; seqSpawnElem < thisSeqSpawn.seqTypeRot.Length; seqSpawnElem++)
                {
                    int seqSpawnElemID = Mathf.RoundToInt(thisSeqSpawn.seqTypeRot[seqSpawnElem].x);
                    if (levelDesigner.CheckIfSpawnRotationIsZero(true, seqSpawnID, seqSpawnElemID))
                    {
                        thisSeqSpawn.seqTypeRot[seqSpawnElem].y = Random.Range(0, 360);
                    }
                    int seqSpawnElemRotation = Mathf.RoundToInt(thisSeqSpawn.seqTypeRot[seqSpawnElem].y / (360 / div));

                    spawnQueue[seqSpawnElem] = new Vector2(seqSpawnElemID, seqSpawnElemRotation); //here
                    spawnNameModOrSeq = "S" + seqSpawnID + " ";
                }
            }
            else
            {
                int rollModSpawnID = Random.Range(0, modSpawnProbabilities.Length);
                int modSpawnID = modSpawnProbabilities[rollModSpawnID];
                if (levelDesigner.CheckIfSpawnRotationIsZero(false, -1, modSpawnID))
                {
                    modSpawnParams[modSpawnID].modTypeRotProb.y = Random.Range(0, 360);
                }
                int modSpawnRotation = Mathf.RoundToInt(modSpawnParams[modSpawnID].modTypeRotProb.y / (360 / div));

                spawnQueue[0] = new Vector2(modSpawnID, modSpawnRotation); //here
                spawnNameModOrSeq = "M" + modSpawnID + " ";
            }
        }

        //Starpower
        bool spawnAsPuny = false;
        if (lightSpeedCounter > 0)
            spawnAsPuny = true;

        int spawnID = Mathf.RoundToInt(spawnQueue[0].x);
        int moduleType = spawnID;
        int moduleRotation = Mathf.RoundToInt(spawnQueue[0].y);
        spawnedModsIndex++;
        spawnedMods[spawnedModsIndex] = SpawnModule(gameSpeed, rotationSpeed, div, moduleRotation, moduleType, spawnAsPuny);
        spawnedMods[spawnedModsIndex].name = spawnNameModOrSeq + spawnedMods[spawnedModsIndex].name + moduleNumber.ToString();
        moduleNumber++;
        //starpower count down
        lightSpeedCounter--;
        //trigger deaccelleration if relevant
        if(lightSpeedCounter == deAccellerationStartPoint)
        {

        }

       //ryk køen
        for (int i=0; i < spawnQueue.Length-1;i++)
        {
            spawnQueue[i] = spawnQueue[i +1];
        }

        //gør det sidste element "tomt" hvis vi nu er i en sequence
        spawnQueue[spawnQueue.Length-1] = new Vector2(-1, -1);

        for (int i = 0; i < numberOfSelectableMods; i++)
        {
            currentSelectables[i] = spawnedMods[i];
        }

        if (selectedModule == null)
        {
            SetSelectedModule(spawnedModsIndex);
        }
    }

    public GameObject SpawnModule(float speed, float rotationSpeed, int division, int initialRotationSteps, int moduleType, bool spawnAsPuny)
    {
      return  gameObject.Instantiate(modules[moduleType], transform.position, modules[moduleType].transform.rotation, transform, speed, rotationSpeed, division, initialRotationSteps, spawnAsPuny);
    }

    /*
    public float gameSpeed;
    public float spawnRate;
    public float rotationSpeed;
    */
    //star power parameters
    public float starPowGameSpeed;
    public float starPowSpawnrate;
    public float starPowRotSpeed;
    public float starPowAcc;
    public float starPowDeacc;

    //star power calculation variables
    public float gameSpeedCalc; // Read
    public float spawnRateCalc; // Read
    public float rotSpeedCalc; // Read

    //brug til acceleration /deAcceleration
    private async Task SetSpeed(float targetGameSpeed, float acceleration, float targetSpawnRate, float targetRotationSpeed)
    {
        await Task.Yield();
        float gameSpeedDiff = targetGameSpeed - this.gameSpeed;
        float spawnSpeedDiff = targetGameSpeed - this.spawnRate;
        float rotationSpeedDiff = targetRotationSpeed - this.rotationSpeed;
        while (Mathf.Abs(gameSpeedDiff)> 0 || Mathf.Abs(spawnSpeedDiff) > 0 || Mathf.Abs(targetRotationSpeed)>0)
        {

            if (gameSpeedDiff > 0)
            {
                this.gameSpeed += 0.01f;
                gameSpeedDiff -= 0.01f;
            }
            else
            {
                this.gameSpeed -= 0.01f;
                gameSpeedDiff += 0.01f;
            }
            if (spawnSpeedDiff > 0)
            {
                this.spawnRate += 0.01f;
                spawnSpeedDiff -= 0.01f;
            }
            else
            {
                this.spawnRate -= 0.01f;
                spawnSpeedDiff += 0.01f;
            }
            if (rotationSpeedDiff > 0)
            {
                this.rotationSpeed += 1f;
                rotationSpeedDiff -= 1f;
            }
            else
            {
                this.rotationSpeed -= 1f;
                targetRotationSpeed += 1f;
            }
            //round to avoid imprecision
            if(Mathf.Abs(gameSpeedDiff) < 0.05f )
            {
                this.gameSpeed = targetGameSpeed;
            }
            if (Mathf.Abs(spawnSpeedDiff) < 0.05f)
            {
                this.spawnRate = targetSpawnRate;
            }
            if (Mathf.Abs(rotationSpeedDiff) < 0.05f)
            {
                this.rotationSpeed = targetRotationSpeed;
            }
            float delay = 1 / acceleration;
            print("delay" + delay);
            int miliseconds = Mathf.RoundToInt(delay * 1000);
            print(miliseconds + " miliseconds");
            await Task.Delay(miliseconds);

        }
       

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
                            TriggerStarPower(currentIndex - 2);
                            break;
                        }
                    }
                    else
                    {
                        lineUpCount = 0;
                    }
                }
                lastType = m.type;
                lastStep = m.step;
                currentIndex++;
            }
        }
        return lineUp;
    }
    public void TriggerStarPower(int startIndex)
    {
        lightSpeedCounter = initialLightSpeedCounter;
        foreach(GameObject g  in spawnedMods)
        {
            if (g)
            {
                Module m = g.GetComponent<Module>();
                m.SetPuny(true);
            }
            
        }
       // SetSpeed(2, 1, 2, 200);

    }
}