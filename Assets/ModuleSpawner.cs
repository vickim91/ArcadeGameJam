using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSpawner : MonoBehaviour
{
    GameManager gameManager;
    LevelDesigner levelDesigner;
    LevelDesigner.SpawnModule[] moduleParams;
    LevelDesigner.Sequence[] sequenceParams;
    
    public int[] modProbabilities; // read
    public int[] seqProbabilities; // read

    public GameObject[] currentSelectables; // read
    public GameObject[] upcomingModules; // read
    public Vector2[] moduleQueue; // read
    private int upcomingModuleIndex;
    public int selectedIndex; // read
    public Module selectedModule; // read
    public float closestModulePositionRead; // read

    // behavior parameters
    public GameObject[] modules; 
    public int chanceForSequence;
    public int queueLength; // note: set this to the length of the longest sequence at start
    public int numberOfSelectableModules;
    public int numberOfModules;
    public int divisionStep;
    public int playerPosition;
    public bool autoSpawn;
    public float autoSpawnSpeed;
    public float gameSpeed;
    public float rotationSpeed;

    private int div;
    private float autoSpawnTimer;
    private int moduleNumber;
    private string spawnNameModOrSeq;

    void Start()
    {
        levelDesigner = GetComponent<LevelDesigner>();
        div = levelDesigner.divisionStepSequence[divisionStep];
        gameManager = FindObjectOfType<GameManager>();
        currentSelectables = new GameObject[numberOfSelectableModules];
        upcomingModules = new GameObject[numberOfModules];
        upcomingModuleIndex = -1;
        selectedIndex = 0;
        moduleQueue = new Vector2[queueLength];
        for (int i = 0; i < moduleQueue.Length; i++)
        {
            moduleQueue[i] = new Vector3(-1, -1);
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
        int totalModuleProbs = 0;
        moduleParams = levelDesigner.spawnModule;
        foreach (LevelDesigner.SpawnModule sM in moduleParams)
        {
            if (sM.divisionApplication[divisionStep])
            {
                float thisProb = sM.modTypeRotProb.z;
                totalModuleProbs += Mathf.RoundToInt(thisProb);
            }
        }
        int totalSequenceProbs = 0;
        sequenceParams = levelDesigner.spawnSequence;
        foreach (LevelDesigner.Sequence sE in sequenceParams)
        {
            if (sE.divisionApplication[divisionStep])
            {
                float thisProb = sE.probality;
                totalSequenceProbs += Mathf.RoundToInt(thisProb);
            }
        }
        modProbabilities = new int[totalModuleProbs];
        seqProbabilities = new int[totalSequenceProbs];

        int modMin = 0;
        int modIndex = 0;
        int modMax = 0;
        foreach (LevelDesigner.SpawnModule sM in moduleParams)
        {
            if (sM.divisionApplication[divisionStep])
            {
                float thisProb = sM.modTypeRotProb.z;
                modMax += Mathf.RoundToInt(thisProb);

                for (int i = modMin; i < modMax; i++)
                {
                    modProbabilities[i] = modIndex;
                }
                modIndex++;
                modMin = modMax;
            }
        }
        int seqIndex = 0;
        int seqMin = 0;
        int seqMax = 0;
        foreach (LevelDesigner.Sequence sE in sequenceParams)
        {
            if (sE.divisionApplication[divisionStep])
            {
                float thisProb = sE.probality;
                seqMax += Mathf.RoundToInt(thisProb);

                for (int i = seqMin; i < seqMax; i++)
                {
                    seqProbabilities[i] = seqIndex;
                }
                seqIndex++;
                seqMin = seqMax;
            }
        }
    }

    private void CheckIfModuleHasReachedThePlayer()
    {
        if (currentSelectables[0] != null)
        {
            closestModulePositionRead = currentSelectables[0].transform.position.z;
            if (currentSelectables[0].transform.position.z > playerPosition)
            {
                //add score. 100 gange game speed for now . magic numbers men det er vel ligegyldigt
                gameManager.addToScore(Mathf.RoundToInt(gameSpeed * 100));
                currentSelectables[0].GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                for (int i = 0; i < numberOfModules - 1; i++)
                {
                    upcomingModules[i] = upcomingModules[i + 1];
                }
                upcomingModules[numberOfModules - 1] = null;
                if (upcomingModuleIndex > -1)
                    upcomingModuleIndex--;

                for (int i = 0; i < numberOfSelectableModules; i++)
                {
                    currentSelectables[i] = upcomingModules[i];
                }
                if (selectedIndex < 0)
                {
                    Debug.Log("hey");
                    selectedIndex = 0;
                }
                if (selectedIndex > 0)
                    selectedIndex--;
                SetSelectedModule(selectedIndex);
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
            selectedIndex++;
            if (selectedIndex > numberOfSelectableModules - 1)
            {
                selectedIndex = numberOfSelectableModules - 1;
            }
            SetSelectedModule(selectedIndex);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = 0;
            SetSelectedModule(selectedIndex);
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
        for (int i = 0; i < numberOfSelectableModules; i++)
        {
            if (currentSelectables[i] != null && currentSelectables[i].transform.position.z < playerPosition )
                currentSelectables[i].GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
    }

    private void PrepareModuleThenSpawn()
    {
        if (moduleQueue[0].x > -2)
//        if (moduleQueue[0].x == -1)
        {
            int clusterRoll = Random.Range(0, 100);
            if (clusterRoll <= chanceForSequence)
            {
                int roll = Random.Range(0, seqProbabilities.Length - 1);
                int seqProb = seqProbabilities[roll];
                LevelDesigner.Sequence thisSequence = sequenceParams[seqProb];
                //thisSequnce.seqTypeRot.Length må ikke være over queueLength
                for (int e = 0; e < thisSequence.seqTypeRot.Length; e++)
                {
                    int thisType = Mathf.RoundToInt(thisSequence.seqTypeRot[e].x);
                    int thisRotation = Mathf.RoundToInt(thisSequence.seqTypeRot[e].y / (360 / div));
                    moduleQueue[e] = new Vector2(thisType, thisRotation);
                    spawnNameModOrSeq = "S" + seqProb + " ";
                }

            }
            else
            {
                int roll = Random.Range(0, modProbabilities.Length - 1);
                int modProb = modProbabilities[roll];
                int thisRotation = Mathf.RoundToInt(moduleParams[modProb].modTypeRotProb.y / (360 / div));
                moduleQueue[0] = new Vector2(modProbabilities[roll], thisRotation);
                spawnNameModOrSeq = "M" + modProb + " ";
            }
        }

        upcomingModuleIndex++;
        upcomingModules[upcomingModuleIndex] = SpawnModule(gameSpeed, rotationSpeed, div, Mathf.RoundToInt(moduleQueue[0].y), Mathf.RoundToInt(moduleQueue[0].x));
        upcomingModules[upcomingModuleIndex].name = spawnNameModOrSeq + upcomingModules[upcomingModuleIndex].name + moduleNumber.ToString();
        
//        moduleQueue[0] = new Vector3(-1, -1);
        for(int i=0; i < moduleQueue.Length-1;i++)
        {
            moduleQueue[i] = moduleQueue[i +1];
        }
        for (int i = 0; i < numberOfSelectableModules; i++)
        {
            currentSelectables[i] = upcomingModules[i];
        }
        moduleNumber++;

        if (selectedModule == null)
        {
            SetSelectedModule(upcomingModuleIndex);
        }
    }

    public GameObject SpawnModule(float speed, float rotationSpeed, int division, int initialRotationSteps, int moduleIndex)
    {
      return  gameObject.Instantiate(modules[moduleIndex], transform.position, modules[moduleIndex].transform.rotation, transform, speed, rotationSpeed, division, initialRotationSteps);
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