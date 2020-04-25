using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    public int[] divisionStepSequence;
    public int[] divisionApplication;
    public bool[] divApplTest;

    [System.Serializable]
    public class SpawnModule
    {
        public Vector3 modTypeRotProb;
        public bool[] divisionApplication;
    }
    public SpawnModule[] spawnModule;

    [System.Serializable]
    public class Sequence
    {
        public int probality;
        public Vector2[] seqTypeRot;
        public bool[] divisionApplication;
    }
    public Sequence[] spawnSequence;

    // Start is called before the first frame update
    void Awake()
    {
        DemonstrationAfDivision();

        SetModuleRandomSpawnRotation(spawnModule.Length);
        SetSequenceRandomSpawnRotation(spawnSequence.Length);
        for (int i = 0; i < spawnModule.Length; i++)
        {
            if (spawnModule[i].divisionApplication.Length == 0)
            {
                SetModuleSequenceDivisionApplicationToMax(spawnModule[i]);
            }
        }
        for (int i = 0; i < spawnSequence.Length; i++)
        {
            if (spawnSequence[i].divisionApplication.Length == 0)
            {
                SetSequenceDivisionApplicationToMax(spawnSequence[i]);
            }
        }
    }

    private void DemonstrationAfDivision()
    {
        divisionApplication = new int[divisionStepSequence.Length];
        for (int i = 0; i < divApplTest.Length; i++)
        {
            if (divApplTest[i])
            {
                divisionApplication[i] = divisionStepSequence[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetModuleSequenceDivisionApplicationToMax(SpawnModule mod)
    {
        mod.divisionApplication = new bool[divisionStepSequence.Length];
        int length = mod.divisionApplication.Length;
        for (int i = 0; i < length; i++)
        {
            mod.divisionApplication[i] = true;
        }
    }
    private void SetSequenceDivisionApplicationToMax(Sequence seq)
    {
        seq.divisionApplication = new bool[divisionStepSequence.Length];
        int length = seq.divisionApplication.Length;
        for (int i = 0; i < length; i++)
        {
            seq.divisionApplication[i] = true;
        }
    }

    private void SetModuleRandomSpawnRotation(int length)
    {
        for (int i = 0; i < length; i++)
        {
            float rotation = spawnModule[i].modTypeRotProb.y;
            if (rotation == 0)
            {
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
