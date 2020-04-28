using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioLoop : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip soundFile;
    public float pitch;
    [HideInInspector]
    public float initialPitch;
    public float volume;
    [HideInInspector]
    public float initialVolume;
    public AudioMixerGroup output;
    [Range(0,99.9f)]
    public float seekPercent;
    public bool playOnStart;
    public bool dontDestroyOnLoad;
    public static bool firstLoad = true;

    public bool fadeOutWhenStopped;
    [Range(0.0001f, 0.1f)]
    public float fadeOutSlopeWhenStopped;
    [MinMaxRange(0, 0.1f)]
    public RangedFloat pitchFadeOutSlope;
    private float pitchFadeOutSlopeCalc;
    [HideInInspector]
    public bool isStopping;
    private bool fading;
    private float fadeVolDestination;
    private float fadeVolSlope;

    private void Start()
    {
        initialPitch = pitch;
        initialVolume = volume;
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        if (playOnStart && firstLoad)
        {
            StartAudioLoop();
        }
        if (dontDestroyOnLoad)
        {
            if (firstLoad == true)
            {
                firstLoad = false;
                DontDestroyOnLoad(this);
            }
        }
    }



    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    SceneManager.LoadScene("SampleScene");
        //}

        if (isStopping)
            FadeOutThenStop();
        else if (fading)
        {
            FadeVolume();
        }
        else
        {
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            SnapToMinOrMaxVolume();
        }
    }

    private void FadeOutThenStop()
    {
        fading = false;
        float fadeOutSlope;
        if (fadeOutWhenStopped)
            fadeOutSlope = fadeOutSlopeWhenStopped;
        else
            fadeOutSlope = 1;
        if (audioSource.pitch > 0)
        {
            audioSource.pitch -= pitchFadeOutSlopeCalc;
        }
        audioSource.volume -= fadeOutSlope;
        if (audioSource.volume <= 0)
        {
            audioSource.Stop();
            audioSource.pitch = pitch;
            audioSource.volume = volume;
        }
    }

    private void FadeVolume()
    {
        if (isStopping == false)
        {
            if (fadeVolSlope < 0)
            {
                if (volume > fadeVolDestination)
                    volume += fadeVolSlope;
                else
                    fading = false;
            }
            else if (fadeVolSlope > 0)
            {
                if (volume < fadeVolDestination)
                    volume += fadeVolSlope;
                else
                    fading = false;
            }
        }
    }

    public void FadeAudioLoop(float volDestination, float volSlope)
    {
        fading = true;
        fadeVolDestination = volDestination;
        fadeVolSlope = volSlope;
    }

    private void SnapToMinOrMaxVolume()
    {
        if (volume > 1)
        {
            volume = 1;
        }
        else if (volume < 0)
        {
            volume = 0;
        }
    }

    public void StartAudioLoop()
    {
        isStopping = false;
        audioSource.outputAudioMixerGroup = output;
        audioSource.clip = soundFile;
        audioSource.Play();
    }



    public void StopAudioLoop()
    {
        pitchFadeOutSlopeCalc = UnityEngine.Random.Range(pitchFadeOutSlope.minValue, pitchFadeOutSlope.maxValue);
        isStopping = true;
    }

    public bool IsPlaying()
    {
        if (audioSource.isPlaying)
            return true;
        else
            return false;
    }

    public void TriggerSeekAndPlay()
    {
        isStopping = false;
        StartAudioLoop();
        audioSource.time = soundFile.length * seekPercent * 0.01f;
    }
}