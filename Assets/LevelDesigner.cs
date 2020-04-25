using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    public int divisionAmount = 5; 

    [System.Serializable]
    public class SpawnModule
    {
        public Vector3 modTypeRotProb;
        public int[] divisionApplication;
    }
    public SpawnModule[] spawnModule;

    [System.Serializable]
    public class Sequence
    {
        public int probality;
        public Vector2[] seqTypeRot;
        public int[] divisionApplication;
    }
    public Sequence[] spawnSequence;

    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {

    }

    private void SetModuleSequenceDivisionApplicationToMax(SpawnModule mod)
    {
        mod.divisionApplication = new int[divisionAmount];
        int length = mod.divisionApplication.Length;
        mod.divisionApplication[0] = 2;
        for (int i = 1; i < length; i++)
        {
            mod.divisionApplication[i] = mod.divisionApplication[i - 1] + 1; // temporary: replace if division sequence is changed
        }
    }
    private void SetSequenceDivisionApplicationToMax(Sequence seq)
    {
        seq.divisionApplication = new int[divisionAmount];
        int length = seq.divisionApplication.Length;
        seq.divisionApplication[0] = 2;
        for (int i = 1; i < length; i++)
        {
            seq.divisionApplication[i] = seq.divisionApplication[i - 1] + 1; // temporary: replace if division sequence is changed
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
