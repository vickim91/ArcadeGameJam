using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioLoop music1;
    public AudioLoop music2;
    public AudioEvent musicStinger;
    public float musicFadeSlope;
    public float musicFadeDelay;
    public float beatDelay;
    public float beatSnapTolerance;
    public static bool firstLoad = true;

    public AudioLoop rotation;
    public AudioEvent rotStopPre;
    public AudioEvent rotationStop;
    public AudioEvent rotStopClearable;
    public AudioEvent rotStopTwoAligned;
    public AudioEvent rotStopThreeAligned;
    public AudioEvent selectNextMod;
    public AudioEvent selectPrevMod;
    public AudioEvent rotCueRight;
    public AudioEvent rotCueLeft;
    public AudioEvent death;
    public AudioEvent clickSound;
    
    private AudioLoop[] rotationV;
    private AudioEvent[] rotStopV;
    private AudioEvent[] rotStopClearableV;

    public float varRotStopPitching;
    public float varRotPitching;
    public float varDistVolSlopeRot;
    public float varDistVolSlopeRotStop;
    public float varVolDuckPercent;
    public float varVolDuckSlope;
    public float selectionPitching;
    public float rotCuePitching;
    public float rotPitchingCounterclockwise;

    public AudioMixerSnapshot gameMix;
    public AudioMixerSnapshot menuMix;
    public float menuFadeSlope;

    public int rotStopCueLenMax;
    public float rotStopCueLenPitching;
    public float rotStopCueLenVol;

    private ModuleSpawner moduleSpawner;
    private int numOfSelectables;
    private int beatCounter;
    private float secondsPerBeat = 60f / 100f / 2f;
    [HideInInspector]
    public float timeUntilNextBeat; // use this to snap rotation-animations to the beat
    private int[] totalCueLengths;
    private IEnumerator snapToBeat;
    private IEnumerator fadeMusic;
    private int rotCueLengthClockwise;
    private int rotCueLengthCounterclockwise;
    private int previousSelection;

    public bool Beat() // use this to trigger rhythmic visuals
    {
        if (timeUntilNextBeat > secondsPerBeat - 0.001f)
            return true;
        else if (timeUntilNextBeat < 0.001f)
            return true;
        else
            return false;
    }

    void Start()
    {
        moduleSpawner = FindObjectOfType<ModuleSpawner>();
        numOfSelectables = moduleSpawner.numberOfSelectableMods;
        totalCueLengths = new int[numOfSelectables];
        InstantiateAudioEventVariantsWithPitching(ref rotationStop, ref rotStopV, true);
        InstantiateAudioEventVariantsWithPitching(ref rotStopClearable, ref rotStopClearableV, false);
        InstantiateAudioLoopVariants(ref rotation, ref rotationV);
        InstantiateAudioEvent(ref rotStopPre);
        InstantiateAudioEvent(ref musicStinger);
        InstantiateAudioEvent(ref rotStopTwoAligned);
        InstantiateAudioEvent(ref rotStopThreeAligned);
        InstantiateAudioEvent(ref selectNextMod);
        InstantiateAudioEvent(ref selectPrevMod);
        InstantiateAudioEvent(ref rotCueRight);
        InstantiateAudioEvent(ref rotCueLeft);
        InstantiateAudioEvent(ref death);
        InstantiateAudioEvent(ref clickSound);

        if (firstLoad)
        {
            InstantiateMusicLoop(ref music2);
            InstantiateMusicLoop(ref music1);
            DontDestroyOnLoad(this);
            firstLoad = false;
        }
    }

    void FixedUpdate()
    {
        timeUntilNextBeat = secondsPerBeat - Time.time % secondsPerBeat;
        if (Beat())
        {
            beatCounter++;
            if (beatCounter == 2)
            {
                music1.StartAudioLoop();
                music2.StartAudioLoop();
                music2.FadeAudioLoop(0, 1);
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameRestart();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MusicSecondPhase();
        }
    }

    private void InstantiateAudioEvent(ref AudioEvent audioEvent)
    {
        audioEvent = Instantiate(audioEvent, transform);
        audioEvent.transform.parent = transform;
        audioEvent.name = audioEvent.name + "_single";
    }
    private void InstantiateMusicLoop(ref AudioLoop audioLoop)
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

    private void SetRotStopCueLengthPitchingAndVolNudge(int moduleIndex)
    {
        if (totalCueLengths[moduleIndex] > rotStopCueLenMax)
            totalCueLengths[moduleIndex] = rotStopCueLenMax;
        float pitchingMax = rotStopCueLenMax * rotStopCueLenPitching;
        float pitching = totalCueLengths[moduleIndex] * rotStopCueLenPitching;
        RangedFloat initPitch = rotStopV[moduleIndex].sound[0].initialPitch;
        rotStopV[moduleIndex].sound[0].pitch.minValue = initPitch.minValue + pitchingMax - pitching;
        rotStopV[moduleIndex].sound[0].pitch.maxValue = initPitch.maxValue + pitchingMax - pitching;
        float volNudgeMax = rotStopCueLenMax * rotStopCueLenVol;
        float volNudge = totalCueLengths[moduleIndex] * rotStopCueLenVol;
        rotStopV[moduleIndex].sound[0].volume += (volNudge - volNudgeMax);
    }

    IEnumerator SnapToBeat(AudioEvent audioEvent)
    {
        if (timeUntilNextBeat > secondsPerBeat - beatSnapTolerance)
        {
            audioEvent.TriggerAudioEvent();
            yield break;
        }
        else
            rotStopPre.TriggerAudioEvent();
        while (Beat() == false)
        {
            yield return null;
        }
        yield return new WaitForSecondsRealtime(0);
        audioEvent.TriggerAudioEvent();
        rotStopPre.StopSoundAllVoices();
    }

    IEnumerator CrossfadeMusic(AudioLoop loop1, float fadeOutSlope, float fadeOutDelay, AudioLoop loop2, float fadeInSlope)
    {
        loop2.FadeAudioLoop(loop1.initialVolume, fadeInSlope);
        yield return new WaitForSeconds(fadeOutDelay);
        loop1.FadeAudioLoop(0, fadeOutSlope);
    }

    public void UpdateLoopVolumeDuckingAppliance(int selectedModule)
    {
        if (previousSelection != selectedModule)
        {
            for (int i = 0; i < numOfSelectables; i++)
            {
                float initVol = rotationV[i].initialVolume;
                float duckVol = initVol * varVolDuckPercent * 0.01f;
                float distSlope = varDistVolSlopeRot * i;
                float fadeSlope = varVolDuckSlope;
                if (i != selectedModule)
                    rotationV[i].FadeAudioLoop(duckVol - distSlope, fadeSlope);
                else
                {
                    rotationV[i].FadeAudioLoop(initVol - distSlope, fadeSlope * 2);
                    rotationV[i].StartAudioLoop();
                }
            }
            previousSelection = selectedModule;
        }
    }

    public void UpdateEventVolumeDuckingAppliance(int moduleIndex, bool isSelected)
    {
        float initVol = rotStopV[moduleIndex].sound[0].initialVolume;
        float distSlope = varDistVolSlopeRotStop * moduleIndex;
        if (isSelected)
            rotStopV[moduleIndex].sound[0].volume = initVol - distSlope * 0.5f;
        else
            rotStopV[moduleIndex].sound[0].volume = initVol - distSlope;
    }

    public void ShiftRotationVoices()
    {
        if (rotationV[0].IsPlaying())
        {
            rotationV[0].StopAudioLoop();
        }
        AudioLoop rotVZero = rotationV[0];
        for (int i = 0; i < 4; i++)
        {
            rotationV[i] = rotationV[i + 1];
        }
        rotationV[4] = rotVZero;


        for (int i = 0; i < 4; i++)
        {
            totalCueLengths[i] = totalCueLengths[i + 1];
        }
        totalCueLengths[4] = 0;
    }

    public void SelectNextModule(int resultingModuleSelection)
    {
        RangedFloat initPitch = selectNextMod.sound[0].initialPitch;
        float pitching = selectionPitching * resultingModuleSelection;
        selectNextMod.sound[0].pitch.minValue = initPitch.minValue + pitching;
        selectNextMod.sound[0].pitch.maxValue = initPitch.maxValue + pitching;
        selectNextMod.TriggerAudioEvent();
        RotationCue(true, false, resultingModuleSelection - 1);
    }
    public void SelectPrevMod(int resultingModuleSelection)
    {
        RangedFloat initPitch = selectPrevMod.sound[0].initialPitch;
        float pitching = selectionPitching * resultingModuleSelection;
        selectPrevMod.sound[0].pitch.minValue = initPitch.minValue + pitching;
        selectPrevMod.sound[0].pitch.maxValue = initPitch.maxValue + pitching;
        selectPrevMod.TriggerAudioEvent();
        RotationCue(true, false, resultingModuleSelection + 1);
    }
    public void Rotation(int selectedModule, bool clockwise)
    {
        float pitchClockwise = rotationV[selectedModule].initialPitch + varRotPitching * selectedModule;
        if (clockwise)
            rotationV[selectedModule].pitch = pitchClockwise;
        else
            rotationV[selectedModule].pitch = pitchClockwise - rotPitchingCounterclockwise;
        if (rotationV[selectedModule].IsPlaying() == false)
            rotationV[selectedModule].StartAudioLoop();
        if (rotationV[selectedModule].isStopping)
            rotationV[selectedModule].StartAudioLoop();
    }
    public void RotationCue(bool resetCueLength, bool clockwise, int selMod) // cueLength is not the sum of clockwise and counterclockwise. It is the sum of cues made since the last switch in cue-direction or the last rotationStop.
    {
        if (resetCueLength)
        {
            rotCueLengthClockwise = 0;
            rotCueLengthCounterclockwise = 0;
        }
        else
        {
            totalCueLengths[selMod]++;
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

    public void RotationStop(int selMod)
    {
        SetRotStopCueLengthPitchingAndVolNudge(selMod);
        RotationCue(true, false, selMod);
        totalCueLengths[selMod] = 0;
        rotationV[selMod].StopAudioLoop();
        snapToBeat = SnapToBeat(rotStopV[selMod]);
        StartCoroutine(snapToBeat);
    }

    public void RotationStopClearable(int selMod)
    {
        snapToBeat = SnapToBeat(rotStopClearableV[selMod]);
        StartCoroutine(snapToBeat);
    }
    public void RotationStopTwoAligned()
    {
        snapToBeat = SnapToBeat(rotStopTwoAligned);
        StartCoroutine(snapToBeat);
    }
    public void RotationStopThreeAligned()
    {
        snapToBeat = SnapToBeat(rotStopThreeAligned);
        StartCoroutine(snapToBeat);
    }


    public void MenuToggle(bool enterOrExitMenu)
    {
        clickSound.TriggerAudioEvent();
        if (enterOrExitMenu)
            menuMix.TransitionTo(menuFadeSlope);
        else
            gameMix.TransitionTo(menuFadeSlope);
    }
    public void PressMenuButton()
    {
        clickSound.TriggerAudioEvent();
    }

    public void GameRestart()
    {
        clickSound.TriggerAudioEvent();
        MusicFirstPhase();
    }

    public void Death()
    {
        death.TriggerAudioEvent();
        MusicFirstPhase();
    }

    private void MusicFirstPhase()
    {
        musicStinger.TriggerAudioEvent();
        fadeMusic = CrossfadeMusic(music2, musicFadeSlope, musicFadeDelay, music1, musicFadeSlope);
        StartCoroutine(fadeMusic);
    }

    public void MusicSecondPhase()
    {
        musicStinger.TriggerAudioEvent();
        fadeMusic = CrossfadeMusic(music1, musicFadeSlope, musicFadeDelay, music2, musicFadeSlope);
        StartCoroutine(fadeMusic);
    }

    public void MusicThirdPhase()
    {

    }

    public void StarPower()
    {
        MusicSecondPhase();
    }

    public void ObliteratePunyModule()
    {
        death.TriggerAudioEvent();
    }

    public void DeactivateStarPower()
    {
        MusicFirstPhase();
    }
}
