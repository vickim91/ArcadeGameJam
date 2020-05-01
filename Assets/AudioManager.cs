using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioLoop music1;
    public AudioLoop music2;
    public AudioLoop music3;
    public AudioEvent musicStinger;
    public AudioLoop starPower;
    public float musicFadeSlope;
    public float musicFadeDelay;
    public float starPowerFadeSlope;
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
    public AudioEvent moduleCleared;
    
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
    private GameManager gameManager;
    private int numOfSelectables;
    private int[] totalCueLengths;
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

    //public bool RightBeforeBeat()
    //{
    //    return false;
    //}



    private void Awake()
    {
        if (!firstLoad)
        {
            Destroy(this.gameObject);
        }
        beatCounter = -2;
        moduleSpawner = FindObjectOfType<ModuleSpawner>();
        numOfSelectables = moduleSpawner.numberOfSelectableMods;
        totalCueLengths = new int[numOfSelectables];

        if (firstLoad)
        {
            InstantiateAudioEventVariantsWithPitching(ref rotationStop, ref rotStopV, true);
            InstantiateAudioEventVariantsWithPitching(ref rotStopClearable, ref rotStopClearableV, false);
            InstantiateAudioLoopVariants(ref rotation, ref rotationV);
            InstantiateAudioEvent(ref rotStopPre);
            InstantiateAudioEvent(ref rotStopTwoAligned);
            InstantiateAudioEvent(ref rotStopThreeAligned);
            InstantiateAudioEvent(ref selectNextMod);
            InstantiateAudioEvent(ref selectPrevMod);
            InstantiateAudioEvent(ref rotCueRight);
            InstantiateAudioEvent(ref rotCueLeft);
            InstantiateAudioEvent(ref clickSound);
            InstantiateAudioEvent(ref moduleCleared);
            InstantiateMusicLoop(ref music1);
            InstantiateMusicLoop(ref music2);
            InstantiateMusicLoop(ref music3);
            InstantiateMusicLoop(ref starPower);
            InstantiateAudioEvent(ref musicStinger);
            InstantiateAudioEvent(ref death);
            DontDestroyOnLoad(this);
            firstLoad = false;
        }
    }
    void Start()
    {
        GameStart();
        gameManager = FindObjectOfType<GameManager>();
        gameMix.TransitionTo(0f);
    }

    private IEnumerator snapToBeat;
    private IEnumerator fadeMusic;
    private IEnumerator fadeOutMusic;
    private IEnumerator fadeInMusic;
    private float secondsPerBeat = 60f / 100f / 2f;
    [HideInInspector]
    public float timeUntilNextBeat; 
    public int beatCounter;
    public int barCounter = 8; // because loop starts at bar9
    public int loopCounterLvl2;
    public enum MusicStates
    {
        level1,
        level2,
        level3
    }
    public MusicStates musicStates;

    public bool musicStartIsLooping;
    public bool musicStartIsPlaying;
    public bool musicTrackIsPlaying;
    public bool musicEndIsPlaying;
    public bool gameIsPlaying;
    public float musicStartFadeSlope;
    public float musicStartFadeDelay;
    public float musicEndFadeSlope;
    public float musicEndFadeDelay;
    public float musicCutFadeSlope;
    public float musicCutStopDelay;

    void FixedUpdate()
    {
        timeUntilNextBeat = secondsPerBeat - Time.time % secondsPerBeat;
        if (Beat())
        {
            beatCounter++;
            if (beatCounter % 4 == 1)
            {
                beatCounter = 1;
                barCounter++;
            }
            if (barCounter > 16)
                barCounter = 1;
            MusicStateChanges();
        }
    }

    private void MusicStateChanges()
    {
        if (musicStates == MusicStates.level1)
        {
            if (musicTrackIsPlaying)
            {
                musicStinger.sound[0].volume = musicStinger.sound[0].initialVolume;
                musicStinger.TriggerAudioEvent();
                fadeOutMusic = FadeOutAndStopMusic(music2, musicCutFadeSlope, musicCutStopDelay);
                StartCoroutine(fadeOutMusic);
                musicTrackIsPlaying = false;
                if (!musicStartIsPlaying)
                {
                    fadeInMusic = FadeMusic(music1, music1.initialVolume, musicCutFadeSlope, 0);
                    StartCoroutine(fadeInMusic);
                    musicStartIsPlaying = true;
                }
            }
            if (musicEndIsPlaying)
            {
                musicStinger.sound[0].volume = musicStinger.sound[0].initialVolume;
                musicStinger.TriggerAudioEvent();
                fadeOutMusic = FadeOutAndStopMusic(music3, musicCutFadeSlope, musicCutStopDelay);
                StartCoroutine(fadeOutMusic);
                musicEndIsPlaying = false;
                if (!musicStartIsPlaying)
                {
                    fadeInMusic = FadeMusic(music1, music1.initialVolume, musicCutFadeSlope, 0);
                    StartCoroutine(fadeInMusic);
                    musicStartIsPlaying = true;
                }
            }
            if (!musicStartIsLooping && beatCounter == 1)
            {
                starPower.StartAudioLoop();
                starPower.volume = 0;
                music1.StartAudioLoop();
                music1.volume = 0;
                music1.FadeAudioLoop(music1.initialVolume, 0.005f);
                musicStartIsLooping = true;
            }
        }

        if (beatCounter == 1)
        {
            if (musicStates == MusicStates.level1)
            {
                if (gameIsPlaying)
                {
                    if (barCounter == 16)
                    {
                        fadeOutMusic = FadeMusic(music1, 0, musicStartFadeSlope, musicStartFadeDelay);
                        StartCoroutine(fadeOutMusic);
                        musicStates = MusicStates.level2;
                        musicStartIsPlaying = false;
                        loopCounterLvl2 = 0;
                    }
                }
            }
            if (musicStates == MusicStates.level2)
            {
                if (barCounter == 1)
                {
                    if (loopCounterLvl2 == 0)
                    {
                        music2.FadeAudioLoop(music2.initialVolume, 1f);
                        music2.StartAudioLoop();
                        musicTrackIsPlaying = true;
                    }
                    loopCounterLvl2++;
                }
                if (loopCounterLvl2 == 6)
                {
                    music2.StopAudioLoop();
                    musicTrackIsPlaying = false;
                    musicStates = MusicStates.level3;
                }
                if (loopCounterLvl2 == 5)
                {
                    if (!musicEndIsPlaying)
                    {
                        music3.StartAudioLoop();
                        music3.FadeAudioLoop(0, 1);
                        musicEndIsPlaying = true;
                    }
                    if (barCounter == 8)
                    {
                        loopCounterLvl2 = 6;
                        fadeOutMusic = FadeMusic(music2, 0, musicEndFadeSlope, musicEndFadeDelay);
                        StartCoroutine(fadeOutMusic);
                        fadeInMusic = FadeMusic(music3, music3.initialVolume, musicEndFadeSlope, musicEndFadeDelay);
                        StartCoroutine(fadeInMusic);
                    }
                }
            }
        }
    }

    IEnumerator FadeOutAndStopMusic(AudioLoop music, float fadeSlope, float stopDelay)
    {
        music.FadeAudioLoop(0, fadeSlope);
        yield return new WaitForSeconds(stopDelay);
        music.StopAudioLoop();
    }
    IEnumerator FadeMusic(AudioLoop music, float volDestination, float fadeSlope, float fadeDelay)
    {
        yield return new WaitForSeconds(fadeDelay);
        music.FadeAudioLoop(volDestination, fadeSlope);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    GameRestart();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    MusicSecondPhase();
        //}
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
        //yield return new WaitForSecondsRealtime(0);
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
        for (int i = 0; i < numOfSelectables - 1; i++)
        {
            rotationV[i] = rotationV[i + 1];
        }
        rotationV[numOfSelectables - 1] = rotVZero;


        for (int i = 0; i < numOfSelectables - 1; i++)
        {
            totalCueLengths[i] = totalCueLengths[i + 1];
        }
        totalCueLengths[numOfSelectables - 1] = 0;
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
    public void PressMenuButton() // this includes: start game, enter "how to play", exit "how to play"
    {
        clickSound.TriggerAudioEvent();
    }

    public void GameStart() // trigger this when starting/restarting from menu, and when restarting after death
    {
        gameIsPlaying = true;
        musicStates = MusicStates.level1;
    }

    public void Death()
    {
        if (!gameManager.godMode)
        {
            gameIsPlaying = false;
            musicStates = MusicStates.level1;
            death.TriggerAudioEvent();
            for (int i = 0; i < numOfSelectables; i++)
            {
                rotationV[i].StopAudioLoop();
            }
        }
    }

    public void StarPower()
    {
        musicStinger.sound[0].volume = musicStinger.sound[0].initialVolume * 0.6f;
        musicStinger.TriggerAudioEvent();
        audioEventDelayedStop = AudioEventDelayedStop(musicStinger, 0.01f * Random.Range(1, 3));
        StartCoroutine(audioEventDelayedStop);
        starPower.FadeAudioLoop(starPower.initialVolume, starPowerFadeSlope * 10);
    }
    IEnumerator AudioEventDelayedStop(AudioEvent audioEvent, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioEvent.StopSoundAllVoices();
    }
    IEnumerator audioEventDelayedStop;

    public void ObliteratePunyModule()
    {
        //moduleCleared.TriggerAudioEvent();
    }

    public void DeactivateStarPower()
    {
        starPower.FadeAudioLoop(0, starPowerFadeSlope);
    }

    public void ModuleCleared()
    {
        //moduleCleared.TriggerAudioEvent();
    }

    private void MusicFirstPhase()
    {
        fadeMusic = CrossfadeMusic(music2, musicFadeSlope, musicFadeDelay, music1, musicFadeSlope);
        StartCoroutine(fadeMusic);
    }

    public void MusicSecondPhase()
    {
        fadeMusic = CrossfadeMusic(music1, musicFadeSlope, musicFadeDelay, music2, musicFadeSlope);
        StartCoroutine(fadeMusic);
    }

    public void MusicThirdPhase()
    {

    }
}
