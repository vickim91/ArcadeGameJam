using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    public bool snapToDiv;
    public int chanceForSequence;
    public int divisionStepMax = 10;

    [System.Serializable]
    public class ModuleSpawn
    {
        public Vector3 modTypeRotProb;
        public int[] theREALDivApplication;
        [HideInInspector]
        public float initialRotation;

    }
    public ModuleSpawn[] moduleSpawn;

    [System.Serializable]
    public class SequenceSpawn
    {
        public int probality;
        public Vector2[] seqTypeRot;
        public int[] theREALDivApplication;
        [HideInInspector]
        public float[] initialRotation;
    }
    public SequenceSpawn[] sequenceSpawn;

    [HideInInspector]
    public int queueLength;

    // Start is called before the first frame update
    void Awake()
    {
        FillOutMissingDivisionApplicationsAndSetQueueLength();
        SetInitialRotations();
        SetSpawnQueueLength();
    }

    void SetSpawnQueueLength()
    {

    }

    //private void FillOutMissingBoolsInDivisionApplicationArrays()
    //{
    //    int lSteps = divisionStepMax + 1;
    //    for (int i = 0; i < moduleSpawn.Length; i++)
    //    {
    //        int lThis = moduleSpawn[i].divApplication.Length;
    //        if (lThis < lSteps)
    //        {
    //            bool[] divAppFiller = new bool[lSteps];
    //            for (int divApps = 0; divApps < lThis; divApps++)
    //            {
    //                divAppFiller[divApps] = moduleSpawn[i].divApplication[divApps];
    //            }
    //            if (lThis == 0)
    //            {
    //                for (int divApps = 0; divApps < lSteps; divApps++)
    //                {
    //                    divAppFiller[divApps] = true;
    //                }
    //            }
    //            moduleSpawn[i].divApplication = divAppFiller;
    //        }
    //    }
    //    for (int i = 0; i < sequenceSpawn.Length; i++)
    //    {
    //        int lThis = sequenceSpawn[i].divApplication.Length;

    //        if (lThis < lSteps)
    //        {
    //            bool[] divAppFiller = new bool[lSteps];
    //            for (int divApps = 0; divApps < lThis; divApps++)
    //            {
    //                divAppFiller[divApps] = sequenceSpawn[i].divApplication[divApps];
    //                divAppFiller[divApps] = true;
    //            }
    //            if (lThis == 0)
    //            {
    //                for (int divApps = 0; divApps < lSteps; divApps++)
    //                {
    //                    divAppFiller[divApps] = true;
    //                }
    //            }
    //            sequenceSpawn[i].divApplication = divAppFiller;
    //        }
    //    }
    //}
    
    private void FillOutMissingDivisionApplicationsAndSetQueueLength()
    {

        for (int i = 0; i < moduleSpawn.Length; i++)
        {
            int lThis = moduleSpawn[i].theREALDivApplication.Length;
            if (lThis == 0) // if no divApplication, then apply to all divisionSteps
            {
                moduleSpawn[i].theREALDivApplication = new int[divisionStepMax + 1];
                for (int divStep = 0; divStep < divisionStepMax + 1; divStep++)
                {
                    moduleSpawn[i].theREALDivApplication[divStep] = divStep;
                }
            }
        }
        for (int i = 0; i < sequenceSpawn.Length; i++)
        {
            if (sequenceSpawn[i].seqTypeRot.Length > queueLength)
                queueLength = sequenceSpawn[i].seqTypeRot.Length;
            int lThis = sequenceSpawn[i].theREALDivApplication.Length;
            if (lThis == 0)
            {
                sequenceSpawn[i].theREALDivApplication = new int[divisionStepMax + 1];
                for (int divStep = 0; divStep < divisionStepMax + 1; divStep++)
                {
                    sequenceSpawn[i].theREALDivApplication[divStep] = divStep;
                }
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
        }
        for (int seqSpawnID = 0; seqSpawnID < sequenceSpawn.Length; seqSpawnID++)
        {
            sequenceSpawn[seqSpawnID].initialRotation = new float[sequenceSpawn[seqSpawnID].seqTypeRot.Length];
            for (int seqSpawnElemID = 0; seqSpawnElemID < sequenceSpawn[seqSpawnID].seqTypeRot.Length; seqSpawnElemID++)
            {
                sequenceSpawn[seqSpawnID].initialRotation[seqSpawnElemID] = sequenceSpawn[seqSpawnID].seqTypeRot[seqSpawnElemID].y;
            }
        }
    }

}
