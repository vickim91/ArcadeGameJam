using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    ModuleSpawner moduleSpawner;
    int numOfSelectables;
    public AudioLoop rotation;
    public AudioEvent rotationStop;
    public AudioEvent rotStopClearable;
    public AudioEvent rotStopTwoAligned;
    public AudioEvent rotStopThreeAligned;
    public AudioEvent selectNextMod;
    public AudioEvent selectPrevMod;
    public AudioEvent rotCueRight;
    public AudioEvent rotCueLeft;
    
    private AudioLoop[] rotationV;
    private AudioEvent[] rotStopV;
    private AudioEvent[] rotStopClearableV;

    public float varRotStopPitching;
    public float varRotPitching;
    public float varVolDuckPercent;
    public float varVolDuckSlope;
    public float selectionPitching;
    public float rotCuePitching;
    public float rotPitchingCounterclockwise;

    int selectedModule;

    // Start is called before the first frame update
    void Start()
    {
        moduleSpawner = FindObjectOfType<ModuleSpawner>();
        numOfSelectables = moduleSpawner.numberOfSelectableMods;
        InstantiateAudioEventVariantsWithPitching(ref rotationStop, ref rotStopV, true);
        InstantiateAudioEventVariantsWithPitching(ref rotStopClearable, ref rotStopClearableV, true);
        InstantiateAudioLoopVariants(ref rotation, ref rotationV);

        InstantiateAudioEvent(ref rotStopTwoAligned);
        InstantiateAudioEvent(ref rotStopThreeAligned);
        InstantiateAudioEvent(ref selectNextMod);
        InstantiateAudioEvent(ref selectPrevMod);
        InstantiateAudioEvent(ref rotCueRight);
        InstantiateAudioEvent(ref rotCueLeft);

        ShiftingStartValues();
    }

    void Update()
    {
//        InputMethodsForTesting();
    }

    private void InstantiateAudioEvent(ref AudioEvent audioEvent)
    {
        audioEvent = Instantiate(audioEvent, transform);
        audioEvent.transform.parent = transform;
        audioEvent.name = audioEvent.name + "_single";
    }
    private void InstantiateAudioLoop(ref AudioLoop audioLoop)
    {
        audioLoop = Instantiate(audioLoop, transform);
        audioLoop.transform.parent = transform;
        audioLoop.name = audioLoop.name + "_single";
    }

    private void InstantiateAudioEventVariantsWithPitching(ref AudioEvent audioEvent, ref AudioEvent[] variants, bool pitching)
    {
        GameObject variantParent = new GameObject();
        variantParent.transform.parent = transform;
        variantParent.name = audioEvent.name + "_parent";
        variants = new AudioEvent[numOfSelectables];
        for (int i = 0; i < numOfSelectables; i++)
        {
            variants[i] = Instantiate(audioEvent, variantParent.transform);
            variants[i].transform.parent = variantParent.transform;
            variants[i].name = audioEvent.name + i;

            if (pitching)
                for (int e = 0; e < variants[i].sound.Length; e++)
                {
                    variants[i].sound[e].pitch.minValue += varRotStopPitching * i;
                    variants[i].sound[e].pitch.maxValue += varRotStopPitching * i;
                }
        }
    }

    private void InstantiateAudioLoopVariants(ref AudioLoop audioLoop, ref AudioLoop[] variants)
    {
        GameObject variantParent = new GameObject();
        variantParent.transform.parent = transform;
        variantParent.name = audioLoop.name + "_parent";
        variants = new AudioLoop[numOfSelectables];
        for (int i = 0; i < numOfSelectables; i++)
        {
            variants[i] = Instantiate(audioLoop, variantParent.transform);
            variants[i].transform.parent = variantParent.transform;
            variants[i].name = audioLoop.name + i;
        }
    }

    int rotCueLengthClockwise;
    int rotCueLengthCounterclockwise;

    private void InputMethodsForTesting()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedModule++;
            SelectNextModule(selectedModule);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedModule--;
            SelectPrevMod(selectedModule);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotationCue(false, true);
                        Rotation(false, selectedModule, true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotationCue(false, false);
                        Rotation(false, selectedModule, false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            RotationStop(selectedModule);
            Rotation(true, selectedModule, false);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            RotationStop(selectedModule);
            RotationStopClearable(selectedModule);
            Rotation(true, selectedModule, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TwoAligned();
            RotationStop(selectedModule);
            Rotation(true, selectedModule, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ThreeAligned();
            RotationStop(selectedModule);
            Rotation(true, selectedModule, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ShiftRotationLoopVoices();
        }
    }

    public void RotationStop(int selMod)
    {
        rotStopV[selMod].TriggerAudioEvent();
    }

    public void RotationStopClearable(int selMod)
    {
        rotStopClearableV[selMod].TriggerAudioEvent();
    }

    public void Rotation(bool stop, int selectedModule, bool clockwise)
    {
        if (stop)
        {
            rotationV[selectedModule].StopAudioLoop();
        }
        else
        {
            print("selModRotationStart:" + selectedModule);
            float pitchClockwise = rotationV[selectedModule].initialPitch + varRotPitching * selectedModule;
            if (clockwise)
                rotationV[selectedModule].pitch = pitchClockwise;
            else
                rotationV[selectedModule].pitch = pitchClockwise - rotPitchingCounterclockwise;
            for (int i = 0; i < numOfSelectables; i++)
            {
                if (rotationV[i].IsPlaying())
                {
                    float initVol = rotationV[i].initialVolume;
                    float duckVol = initVol * varVolDuckPercent * 0.01f;
                    float slope = varVolDuckSlope;
                    if (i != selectedModule)
                        rotationV[i].FadeAudioLoop(duckVol, -slope);
                    else
                        rotationV[i].FadeAudioLoop(initVol, slope);
                }
            }
            if (rotationV[selectedModule].IsPlaying() == false)
                rotationV[selectedModule].StartAudioLoop();
        }
    }

    int counter;
    bool isPlaying;

    public void ShiftingStartValues()
    {
        counter = 4;
        rotationV[4].name = rotationV[4].name + "playing";
//        Rotation(false, 4, true);
    }
    public void ShiftRotationLoopVoices()
    {
        if (rotationV[0].IsPlaying())
        {
            Rotation(true, 0, false);
        }
        isPlaying = false;
        AudioLoop rotVZero = rotationV[0];
        for (int i = 0; i < 4; i++)
        {
            rotationV[i] = rotationV[i + 1];
//            Debug.Log(i + " " + rotationV[i].IsPlaying());
        }
        rotationV[4] = rotVZero;
    }

    public void ModulePassesPlayer()
    {
        Rotation(true, 0, false);
        // should anything extra happen here? a shift in the array? is that even neccessary?
    }

    public void TwoAligned()
    {
        rotStopTwoAligned.TriggerAudioEvent();
    }
    public void ThreeAligned()
    {
        rotStopThreeAligned.TriggerAudioEvent();
    }
    public void SelectNextModule(int resultingModuleSelection)
    {
        RangedFloat initPitch = selectNextMod.sound[0].initialPitch;
        float pitching = selectionPitching * resultingModuleSelection;
        selectNextMod.sound[0].pitch.minValue = initPitch.minValue + pitching;
        selectNextMod.sound[0].pitch.maxValue = initPitch.maxValue + pitching;
        selectNextMod.TriggerAudioEvent();
        RotationCue(true, false);
    }
    public void SelectPrevMod(int resultingModuleSelection)
    {
        RangedFloat initPitch = selectPrevMod.sound[0].initialPitch;
        float pitching = selectionPitching * resultingModuleSelection;
        selectPrevMod.sound[0].pitch.minValue = initPitch.minValue + pitching;
        selectPrevMod.sound[0].pitch.maxValue = initPitch.maxValue + pitching;
        selectPrevMod.TriggerAudioEvent();
        RotationCue(true, false);
    }

    public void RotationCue(bool resetCueLength, bool clockwise) // cueLength is not the sum of clockwise and counterclockwise. It is the sum of cues made since the last switch in cue-direction or the last rotationStop.
    {
        if (resetCueLength)
        {
            rotCueLengthClockwise = 0;
            rotCueLengthCounterclockwise = 0;
        }
        else
        {
            if (clockwise)
            {
                RangedFloat initPitch = rotCueLeft.sound[0].initialPitch;
                float pitching = rotCuePitching * rotCueLengthClockwise;
                rotCueLeft.sound[0].pitch.minValue = initPitch.minValue + pitching;
                rotCueLeft.sound[0].pitch.maxValue = initPitch.maxValue + pitching;
                rotCueLeft.TriggerAudioEvent();
                rotCueLengthClockwise++;
                rotCueLengthCounterclockwise = 0;
            }
            else
            {
                RangedFloat initPitch = rotCueRight.sound[0].initialPitch;
                float pitching = rotCuePitching * rotCueLengthCounterclockwise;
                rotCueRight.sound[0].pitch.minValue = initPitch.minValue + pitching;
                rotCueRight.sound[0].pitch.maxValue = initPitch.maxValue + pitching;
                rotCueRight.TriggerAudioEvent();
                rotCueLengthClockwise = 0;
                rotCueLengthCounterclockwise++;
            }
        }
    }
}
