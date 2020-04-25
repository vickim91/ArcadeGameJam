using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject module;
    public GameObject[] currentSelectables;
    public GameObject[] upcomingModules;
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

    void Start()
    {
        currentSelectables = new GameObject[numberOfSelectableModules];
        upcomingModules = new GameObject[numberOfModules];
        indexUpcomingModules = -1;
        selectedIndex = 0;
    }

    void setSelectedModule(int index)
    {
        if(currentSelectables[index] != null)
        {
            if (selectedModule != null)
            {
                selectedModule.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            }
            selectedModule = currentSelectables[index].GetComponent<Module>();
            selectedModule.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
    }
    void Update()
    {
        if (currentSelectables[0] != null)
        {
            closestModulePositionRead = currentSelectables[0].transform.position.z;
            if (currentSelectables[0].transform.position.z > playerPosition)
            {
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
                setSelectedModule(0);
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
            if(selectedIndex > 2)
            {
                selectedIndex = 2;
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
                print("roter mod uret ModuleSpawner");
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

        indexUpcomingModules++;
        upcomingModules[indexUpcomingModules] = SpawnModule(gameSpeed, rotationSpeed, division);
        upcomingModules[indexUpcomingModules].name = "Module" + moduleNumber;

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

    public GameObject SpawnModule(float speed, float rotationSpeed, int division)
    {
      return  gameObject.Instantiate(module, transform.position, module.transform.rotation, transform, speed, rotationSpeed, division);
    }
}
