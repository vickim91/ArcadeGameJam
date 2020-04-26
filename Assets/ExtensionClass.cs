using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionClass { 
    public static GameObject Instantiate(this Object thisObj, Object original, Vector3 position, Quaternion rotatíon, Transform parent, float speed, float rotationSpeed, int division, int initialRotationSteps, bool spawnAsPuny)
    {
        GameObject obj = Object.Instantiate(original, position, rotatíon, parent) as GameObject;
        Module m = obj.GetComponent<Module>();
        m.Init(speed, rotationSpeed, division, initialRotationSteps, spawnAsPuny);
        return obj ;
        
    }


}
