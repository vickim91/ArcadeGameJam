using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] modules;
    public Vector3[] moduleParams;
    public int[] probalities;
    public int clusterProbability;
    public GameObject[] currentSelectables;
    public GameObject[] upcomingModules;
    public int[] moduleQueue;
    public int queueLength;
    public int numberOfSelectableModules;
    private int indexUpcomingModules;
    public int numberOfModules;
    public int selectedIndex;
    public Module selectedModule;
    public int division;
    private int moduleNumber;
    public int playerPosition;
    public int destroyPosition;
    public float closestModulePositionRead;
    public bool autoSpawn;
    public float autoSpawnSpeed;
    private float autoSpawnTimer;
    public float gameSpeed;
    public float rotationSpeed;
 

    void Start()
    {
        currentSelectables = new GameObject[numberOfSelectableModules];
        upcomingModules = new GameObject[numberOfModules];
        indexUpcomingModules = -1;
        selectedIndex = 0;
        moduleQueue = new int[queueLength];
        for (int i =0; i< moduleQueue.Length; i++)
        {
            moduleQueue[i] = -1;
        }

        int totalProb = 0;
        foreach (Vector3 v in moduleParams)
        {
            totalProb += Mathf.RoundToInt(v.y);
        }

        probalities = new int[totalProb];

        int min = 0;
        int index = 0;
        int max = 0;
        foreach(Vector3 v in moduleParams)
        {
            max += Mathf.RoundToInt(v.y);
          
            for (int i=min; i< max; i++ )
            {
                probalities[i] = index;
            }
            index++;
            min = max;
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
            }
        }
        //rotér med uret
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedModule)
            {
                selectedModule.Rotate(true);
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
       if(moduleQueue[0] == -1)
        {
            int clusterRoll = Random.Range(0, 100);
            if (clusterRoll <= clusterProbability)
            {
                int roll = Random.Range(0, probalities.Length - 1);
                for(int i=0; i<3;i++)
                {
                    moduleQueue[i] = probalities[roll];
                }
            }
            else
            {
                int roll = Random.Range(0, probalities.Length - 1);
                moduleQueue[0] = probalities[roll];
               
            }
        }
        

        indexUpcomingModules++;
        //initialrotation
        int rand = Random.Range(0, division);

        upcomingModules[indexUpcomingModules] = SpawnModule(gameSpeed, rotationSpeed, division, rand, moduleQueue[0]);
        upcomingModules[indexUpcomingModules].name +=   moduleNumber;

        moduleQueue[0] = -1;
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
}
