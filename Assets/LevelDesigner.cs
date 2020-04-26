using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    public bool snapToDiv;
    public int chanceForSequence;
    public int[] divisionStepSequence;

    [System.Serializable]
    public class ModuleSpawn
    {
        public Vector3 modTypeRotProb;
        public bool[] divApplication;
        [HideInInspector]
        public float initialRotation;

    }
    public ModuleSpawn[] moduleSpawn;

    [System.Serializable]
    public class SequenceSpawn
    {
        public int probality;
        public Vector2[] seqTypeRot;
        public bool[] divApplication;
        [HideInInspector]
        public float[] initialRotation;
    }
    public SequenceSpawn[] sequenceSpawn;

    // Start is called before the first frame update
    void Awake()
    {
        SetInitialRotations();
        SetModuleRandomSpawnRotation();
        SetSequenceRandomSpawnRotation();
        FillOutMissingBoolsInDivisionApplicationArrays();
    }

    private void FillOutMissingBoolsInDivisionApplicationArrays()
    {
        int lSteps = divisionStepSequence.Length;
        for (int i = 0; i < moduleSpawn.Length; i++)
        {
            int lThis = moduleSpawn[i].divApplication.Length;
            if (lThis < lSteps)
            {
                bool[] divAppFiller = new bool[lSteps];
                for (int divApps = 0; divApps < lThis; divApps++)
                {
                    divAppFiller[divApps] = moduleSpawn[i].divApplication[divApps];
                }
                if (lThis == 0)
                {
                    for (int divApps = 0; divApps < lSteps; divApps++)
                    {
                        divAppFiller[divApps] = true;
                    }
                }
                moduleSpawn[i].divApplication = divAppFiller;
            }
        }
        for (int i = 0; i < sequenceSpawn.Length; i++)
        {
            int lThis = sequenceSpawn[i].divApplication.Length;
            
            if (lThis < lSteps)
            {
                bool[] divAppFiller = new bool[lSteps];
                for (int divApps = 0; divApps < lThis; divApps++)
                {
                    divAppFiller[divApps] = sequenceSpawn[i].divApplication[divApps];
                    divAppFiller[divApps] = true;
                }
                if (lThis == 0)
                {
                    for (int divApps = 0; divApps < lSteps; divApps++)
                    {
                        divAppFiller[divApps] = true;
                    }
                }
                sequenceSpawn[i].divApplication = divAppFiller;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetModuleRandomSpawnRotation()
    {
        for (int i = 0; i < moduleSpawn.Length; i++)
        {
            float rotation = moduleSpawn[i].modTypeRotProb.y;
            if (rotation == 0)
            {
//                Debug.Log("time for some random rotation, oke?");
                moduleSpawn[i].modTypeRotProb.y = Random.Range(0, 360);
            }
        }
    }

    private void SetSequenceRandomSpawnRotation()
    {
        for (int i = 0; i < sequenceSpawn.Length; i++)
        {
            for (int localI = 0; localI < sequenceSpawn[i].seqTypeRot.Length; localI++)
            {
                float rotation = sequenceSpawn[i].seqTypeRot[localI].y;
                if (rotation == 0)
                {
                    sequenceSpawn[i].seqTypeRot[localI].y = Random.Range(0, 360);
                }
            }
        }
    }

    public bool CheckIfSpawnRotationIsZero(bool sequence, int seqID, int modID)
    {
        if (sequence)
        {
            if (sequenceSpawn[seqID].initialRotation[modID] == 0)
                return true;
        }
        else
        {
            if (moduleSpawn[modID].initialRotation == 0)
                return true;
        }
        return false;
    }

    private void SetInitialRotations()
    {
        for (int modSpawnID = 0; modSpawnID < moduleSpawn.Length; modSpawnID++)
        {
            moduleSpawn[modSpawnID].initialRotation = moduleSpawn[modSpawnID].modTypeRotProb.y;
//            Debug.Log("initMod" + modSpawnID + " " + moduleSpawn[modSpawnID].initialRotation);
        }
        for (int seqSpawnID = 0; seqSpawnID < sequenceSpawn.Length; seqSpawnID++)
        {
            sequenceSpawn[seqSpawnID].initialRotation = new float[sequenceSpawn.Length];
            for (int seqSpawnElemID = 0; seqSpawnElemID < sequenceSpawn[seqSpawnID].seqTypeRot.Length; seqSpawnElemID++)
            {
                sequenceSpawn[seqSpawnID].initialRotation[seqSpawnElemID] = sequenceSpawn[seqSpawnID].seqTypeRot[seqSpawnElemID].y;
//                Debug.Log("initSeq" + seqSpawnID + " " + sequenceSpawn[seqSpawnID].initialRotation[seqSpawnElemID]);
            }
        }
    }

}
