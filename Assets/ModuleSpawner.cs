using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] modules;
    LevelDesigner.SpawnModule[] moduleParams;
    LevelDesigner.Sequence[] sequenceParams;
    public int[] probalities;
    public int chanceForSequence;
    public int[] sequenceProbabilities;

    public GameObject[] currentSelectables;
    public GameObject[] upcomingModules;
    public Vector2[] moduleQueue;
    public int queueLength;
    public int numberOfSelectableModules;
    private int indexUpcomingModules;
    public int numberOfModules;
    public int selectedIndex;
    public Module selectedModule;
    public int division;
    private int moduleNumber;
    public int playerPosition;

    public float closestModulePositionRead;
    public bool autoSpawn;
    public float autoSpawnSpeed;
    private float autoSpawnTimer;
    public float gameSpeed;
    public float rotationSpeed;
    GameManager gameManager;
    LevelDesigner levelDesigner;

    void Start()
    {
        levelDesigner = GetComponent<LevelDesigner>();
        gameManager = FindObjectOfType<GameManager>();
        currentSelectables = new GameObject[numberOfSelectableModules];
        upcomingModules = new GameObject[numberOfModules];
        indexUpcomingModules = -1;
        selectedIndex = 0;
        moduleQueue = new Vector2[queueLength];
        for (int i = 0; i < moduleQueue.Length; i++)
        {
            moduleQueue[i] = new Vector2(-1, -1);
        }

        SetProbabilities();
    }
    //calculate probabilites 
    public void SetProbabilities()
    {
        int totalProb = 0;
        moduleParams = levelDesigner.spawnModule;
        foreach (LevelDesigner.SpawnModule sM in moduleParams)
        {
            bool include = false;
            for(int i =0; i< sM.divisionApplication.Length; i++)
            {

            } 
            float thisProb = sM.modTypeRotProb.z;
            totalProb += Mathf.RoundToInt(thisProb);
        }
        int totalSequenceProbs = 0;
        sequenceParams = levelDesigner.spawnSequence;
        foreach (LevelDesigner.Sequence sE in sequenceParams)
        {
            float thisProb = sE.probality;
            totalSequenceProbs += Mathf.RoundToInt(thisProb);
        }
        probalities = new int[totalProb];

        int min = 0;
        int index = 0;
        int max = 0;
        foreach (LevelDesigner.SpawnModule sM in moduleParams)
        {
            float thisProb = sM.modTypeRotProb.z;
            max += Mathf.RoundToInt(thisProb);

            for (int i = min; i < max; i++)
            {
                probalities[i] = index;
            }
            index++;
            min = max;
        }
        int clusterIndex = 0;
        int clusterMax = 0;
        foreach (LevelDesigner.Sequence sE in sequenceParams)
        {
            float thisProb = sE.probality;
            clusterMax += Mathf.RoundToInt(thisProb);

            for (int i = min; i < max; i++)
            {
                sequenceProbabilities[i] = clusterIndex;
            }
            clusterIndex++;
            min = clusterMax;
        }
    }

    void setSelectedModule(int index)
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
            if (currentSelectables[i].transform.position.z < playerPosition)
                currentSelectables[i].GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
    }

    void Update()
    {
        if (currentSelectables[0] != null)
        {
            closestModulePositionRead = currentSelectables[0].transform.position.z;
            if (currentSelectables[0].transform.position.z > playerPosition)
            {
                //add score. 100 gange game speed for now . magic numbers men det er vel ligegyldigt
                gameManager.addToScore(Mathf.RoundToInt( gameSpeed * 100));
                currentSelectables[0].GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                for (int i = 0; i < numberOfModules - 1; i++)
                {
                    upcomingModules[i] = upcomingModules[i + 1];
                }
                upcomingModules[numberOfModules - 1] = null;
                if (indexUpcomingModules > -1)
                    indexUpcomingModules--;
                
                for (int i = 0; i < numberOfSelectableModules; i++)
                {
                    currentSelectables[i] = upcomingModules[i];
                }
                if (selectedIndex == 0)
                {
                    
                }
                if (selectedIndex > 0)
                selectedIndex--;
                setSelectedModule(selectedIndex);
            }
          
        }
        if (autoSpawn)
        {
            autoSpawnTimer += Time.deltaTime;
            if (autoSpawnTimer > 1 / autoSpawnSpeed)
            {
                PrepareModuleThenSpawn();
                autoSpawnTimer = 0;
            }
        }
        //test
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PrepareModuleThenSpawn();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex++;
            if(selectedIndex > numberOfSelectableModules - 1)
            {
                selectedIndex = numberOfSelectableModules - 1;
            }
            setSelectedModule(selectedIndex);
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = 0;
            setSelectedModule(selectedIndex);
        }
        //rotér mod uret
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedModule)
            {
//                print("roter mod uret ModuleSpawner");
                selectedModule.Rotate(false);
                checkForLineUp();
            }
        }
        //rotér med uret
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedModule)
            {
                selectedModule.Rotate(true);
                checkForLineUp();
            }
        }
    }

    private void PrepareModuleThenSpawn()
    {
        //if (indexSelectableModules < numberOfSelectableModules - 1 && indexUpcomingModules < 1)
        //{
        //    indexSelectableModules++;
        //    currentModuleSelectables[indexSelectableModules] = SpawnModule(1f, 1f, division);
        //    currentModuleSelectables[indexSelectableModules].name = "Module" + moduleNumber;
        //}
       // opbyg queue
       //if(moduleQueue[0] == -1)
       // {
       //     int clusterRoll = Random.Range(0, 100);
       //     if (clusterRoll <= clusterProbability)
       //     {
       //         int roll = Random.Range(0, probalities.Length - 1);
       //         for(int i=0; i<3;i++)
       //         {
       //             moduleQueue[i] = probalities[roll];
       //         }
       //     }
       //     else
       //     {
       //         int roll = Random.Range(0, probalities.Length - 1);
       //         moduleQueue[0] = probalities[roll];
               
       //     }
       // }
       if(moduleQueue[0].x == -1)
        {
            int clusterRoll = Random.Range(0, 100);
            if (clusterRoll <= chanceForSequence)
            {
                int roll = Random.Range(0, sequenceProbabilities.Length - 1);
                LevelDesigner.Sequence thisSequence = sequenceParams[roll];
               //thisSequnce.seqTypeRot.Length må ikke være over queueLength
                  for (int e = 0; e < thisSequence.seqTypeRot.Length; e++)
                    {
                        int thisType = Mathf.RoundToInt( thisSequence.seqTypeRot[e].x);
                        int thisRotation = Mathf.RoundToInt(thisSequence.seqTypeRot[e].y/ (360/division));
                        moduleQueue[e] = new Vector2(thisType, thisRotation);
                    }
                
            }
            else
            {
                int roll = Random.Range(0, probalities.Length - 1);
                int thisRotation = Mathf.RoundToInt(moduleParams[probalities[roll]].modTypeRotProb.y / (360/division));
                moduleQueue[0] = new Vector2(probalities[roll], thisRotation);
            }
        }
        

        indexUpcomingModules++;
        //initialrotation
        //int rand = Random.Range(0, division);

        upcomingModules[indexUpcomingModules] = SpawnModule(gameSpeed, rotationSpeed, division, Mathf.RoundToInt(moduleQueue[0].y), Mathf.RoundToInt( moduleQueue[0].x));
        upcomingModules[indexUpcomingModules].name +=   moduleNumber;

        moduleQueue[0] = new Vector2(-1, -1);
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
            setSelectedModule(indexUpcomingModules);
        }
    }

    public GameObject SpawnModule(float speed, float rotationSpeed, int division, int initialRotationSteps, int moduleIndex)
    {
      return  gameObject.Instantiate(modules[moduleIndex], transform.position, modules[moduleIndex].transform.rotation, transform, speed, rotationSpeed, division, initialRotationSteps);
    }
    public bool checkForLineUp()
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
        print("lineUp starting at " + startIndex);
    }
}

