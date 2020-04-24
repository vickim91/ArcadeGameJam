using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject module;
    public GameObject[] currentModules;
    public int index;
    public int selectedIndex;
    public Module selectedModule;
    void Start()
    {
        currentModules = new GameObject[3];
        index = 0;
        selectedIndex = 0;
    }

    // Update is called once per frame
    void setSelectedModule(int index)
    {
        if(currentModules[index] != null)
        selectedModule = currentModules[index].GetComponent<Module>();
    }
    void Update()
    {
        //test
        if(Input.GetKeyDown(KeyCode.Space))
        {
            
            currentModules[index] = SpawnModule(1f, 1f, 5);
            if (selectedModule == null)
            {
                setSelectedModule(index);
            }
              
            index++;
            if (index == 3)
            {
                index = 0;
            }
           

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
    public GameObject SpawnModule(float speed, float rotationSpeed, int division)
    {
      return  gameObject.Instantiate(module, transform.position, module.transform.rotation, transform, speed, rotationSpeed, division);
    }
}
