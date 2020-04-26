using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    public bool snapToDiv;
    public int chanceForSequence;
    public int[] divisionStepSequence;

    [System.Serializable]
    public class SpawnModule
    {
        public Vector3 modTypeRotProb;
        public bool[] divApplication;
    }
    public SpawnModule[] spawnModule;

    [System.Serializable]
    public class Sequence
    {
        public int probality;
        public Vector2[] seqTypeRot;
        public bool[] divApplication;
    }
    public Sequence[] spawnSequence;

    // Start is called before the first frame update
    void Awake()
    {
        SetModuleRandomSpawnRotation(spawnModule.Length);
        SetSequenceRandomSpawnRotation(spawnSequence.Length);
        FillOutMissingBoolsInDivisionApplicationArrays();
    }

    private void FillOutMissingBoolsInDivisionApplicationArrays()
    {
        int lSteps = divisionStepSequence.Length;
        for (int i = 0; i < spawnModule.Length; i++)
        {
            int lThis = spawnModule[i].divApplication.Length;
            if (lThis < lSteps)
            {
                bool[] divAppFiller = new bool[lSteps];
                for (int divApps = 0; divApps < lThis; divApps++)
                {
                    divAppFiller[divApps] = spawnModule[i].divApplication[divApps];
                }
                if (lThis == 0)
                {
                    for (int divApps = 0; divApps < lSteps; divApps++)
                    {
                        divAppFiller[divApps] = true;
                    }
                }
                spawnModule[i].divApplication = divAppFiller;
            }
        }
        for (int i = 0; i < spawnSequence.Length; i++)
        {
            int lThis = spawnSequence[i].divApplication.Length;
            
            if (lThis < lSteps)
            {
                bool[] divAppFiller = new bool[lSteps];
                for (int divApps = 0; divApps < lThis; divApps++)
                {
                    divAppFiller[divApps] = spawnSequence[i].divApplication[divApps];
                    divAppFiller[divApps] = true;
                }
                if (lThis == 0)
                {
                    for (int divApps = 0; divApps < lSteps; divApps++)
                    {
                        divAppFiller[divApps] = true;
                    }
                }
                spawnSequence[i].divApplication = divAppFiller;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetModuleRandomSpawnRotation(int length)
    {
        for (int i = 0; i < length; i++)
        {
            float rotation = spawnModule[i].modTypeRotProb.y;
            if (rotation == 0)
            {
                Debug.Log("time for some random rotation, oke?");
                spawnModule[i].modTypeRotProb.y = Random.Range(0, 360);
            }
        }
    }

    private void SetSequenceRandomSpawnRotation(int length)
    {
        for (int i = 0; i < length; i++)
        {
            for (int localI = 0; localI < spawnSequence[i].seqTypeRot.Length; localI++)
            {
                float rotation = spawnSequence[i].seqTypeRot[localI].y;
                if (rotation == 0)
                {
                    spawnSequence[i].seqTypeRot[localI].y = Random.Range(0, 360);
                }
            }
        }
    }

}
