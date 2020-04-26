using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSpawner : MonoBehaviour
{
    GameManager gameManager;
    LevelDesigner levelDesigner;
    LevelDesigner.SpawnModule[] modSpawnParams;
    LevelDesigner.Sequence[] seqSpawnParams;
    
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
    // level design tools:
    public int divisionStep;
    public int playerPosition;
    public bool autoSpawn;
    public float autoSpawnSpeed;
    public float gameSpeed;
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
            spawnQueue[i] = new Vector3(-1, -1);
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
        modSpawnParams = levelDesigner.spawnModule;
        foreach (LevelDesigner.SpawnModule sM in modSpawnParams)
        {
            if (sM.divApplication[divisionStep])
            {
                float modSpawnProb = sM.modTypeRotProb.z;
                totalModSpawnProbs += Mathf.RoundToInt(modSpawnProb);
            }
        }
        int totalSeqSpawnProbs = 0;
        seqSpawnParams = levelDesigner.spawnSequence;
        foreach (LevelDesigner.Sequence sE in seqSpawnParams)
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
        foreach (LevelDesigner.SpawnModule sM in modSpawnParams)
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
        foreach (LevelDesigner.Sequence sE in seqSpawnParams)
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
        if (autoSpawn)
        {
            autoSpawnTimer += Time.deltaTime;
            if (autoSpawnTimer > 1 / autoSpawnSpeed)
            {
                PrepareModuleThenSpawn();
                autoSpawnTimer = 0;
            }
        }
    }

    private void InputMethods()
    {
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


        if (spawnQueue[0].x > -2)
//        if (moduleQueue[0].x == -1)
        {
            int rollSeqSpawnVsModSpawn = Random.Range(0, 100);
            if (rollSeqSpawnVsModSpawn <= levelDesigner.chanceForSequence)
            {
                int rollSeqSpawnID = Random.Range(0, seqSpawnProbabilities.Length);
                int seqSpawnID = seqSpawnProbabilities[rollSeqSpawnID];
                LevelDesigner.Sequence thisSeqSpawn = seqSpawnParams[seqSpawnID];
                //thisSequnce.seqTypeRot.Length må ikke være over queueLength
                for (int seqSpawnElem = 0; seqSpawnElem < thisSeqSpawn.seqTypeRot.Length; seqSpawnElem++)
                {
                    int seqSpawnElemID = Mathf.RoundToInt(thisSeqSpawn.seqTypeRot[seqSpawnElem].x);
                    int seqSpawnElemRotation = Mathf.RoundToInt(thisSeqSpawn.seqTypeRot[seqSpawnElem].y / (360 / div));
                    spawnQueue[seqSpawnElem] = new Vector2(seqSpawnElemID, seqSpawnElemRotation);
                    spawnNameModOrSeq = "S" + seqSpawnID + " ";
                }
            }
            else
            {
                int rollModSpawnID = Random.Range(0, modSpawnProbabilities.Length);
                int modSpawnID = modSpawnProbabilities[rollModSpawnID];
                int modSpawnRotation = Mathf.RoundToInt(modSpawnParams[modSpawnID].modTypeRotProb.y / (360 / div));
                spawnQueue[0] = new Vector2(modSpawnProbabilities[rollModSpawnID], modSpawnRotation);
                spawnNameModOrSeq = "M" + modSpawnID + " ";
            }
        }

        int spawnID = Mathf.RoundToInt(spawnQueue[0].x);
        int moduleType = Mathf.RoundToInt(modSpawnParams[spawnID].modTypeRotProb.x);
        spawnedModsIndex++;
        spawnedMods[spawnedModsIndex] = SpawnModule(gameSpeed, rotationSpeed, div, Mathf.RoundToInt(spawnQueue[0].y), moduleType);
        spawnedMods[spawnedModsIndex].name = spawnNameModOrSeq + spawnedMods[spawnedModsIndex].name + moduleNumber.ToString();
        moduleNumber++;

        //        moduleQueue[0] = new Vector3(-1, -1);
        for (int i=0; i < spawnQueue.Length-1;i++)
        {
            spawnQueue[i] = spawnQueue[i +1];
        }
        for (int i = 0; i < numberOfSelectableMods; i++)
        {
            currentSelectables[i] = spawnedMods[i];
        }

        if (selectedModule == null)
        {
            SetSelectedModule(spawnedModsIndex);
        }
    }

    public GameObject SpawnModule(float speed, float rotationSpeed, int division, int initialRotationSteps, int moduleType)
    {
      return  gameObject.Instantiate(modules[moduleType], transform.position, modules[moduleType].transform.rotation, transform, speed, rotationSpeed, division, initialRotationSteps);
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
                            TriggerSuperPower(currentIndex - 2);
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
    public void TriggerSuperPower(int startIndex)
    {
//        print("lineUp starting at " + startIndex);
    }
}