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
    [MinMaxRange(0, 10)]
    public RangedFloat pitchFadeOutSlope;
    private float pitchFadeOutSlopeCalc;
    [HideInInspector]
    public bool isStopping;
    private bool fading;
    private float fadeVolDestination;
    private float fadeVolSlope;
    public float fadeInTime;
    public bool dontLoop;

    private void Start()
    {
        initialPitch = pitch;
        initialVolume = volume;
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        if (dontLoop)
            audioSource.loop = false;

        if (playOnStart && firstLoad)
        {
            StartAudioLoop();
            if (fadeInTime > 0)
            {
                volume = 0;
                fadeVolDestination = initialVolume;
                fadeVolSlope = 1 / (160 * fadeInTime);
                fading = true;
            }
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



    private void FixedUpdate()
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
        else if (timeFading)
        {
            TimeFading();
        }
        else
        {
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            SnapToMinOrMaxVolume();
        }
    }

    bool timeFading;
    float fadeVolDestination2;
    float fadeTimeDestination;
    private void TimeFading()
    {
        float origin = audioSource.volume;
        float path = fadeVolDestination2 - origin;
        float fadeTimeRemaining = fadeTimeDestination - Time.time;
        float fadeSlope = path * (Time.deltaTime / fadeTimeRemaining);
        if (!isStopping && !fading)
        {
            if (Mathf.Abs(fadeSlope) > 0)
            {
                volume += fadeSlope;
            }
            else
            {
                timeFading = false;
            }
            audioSource.volume = volume;
        }
    }
    public void TimeFadeAudioLoop(float volDestination, float fadingTime)
    {
        timeFading = true;
        fadeTimeDestination = Time.time + fadingTime;
        fadeVolDestination2 = volDestination;
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
                if (volume < fadeVolDestination)
                    volume = fadeVolDestination;
            }
            else if (fadeVolSlope > 0)
            {
                if (volume < fadeVolDestination)
                    volume += fadeVolSlope;
                else
                    fading = false;
                if (volume > fadeVolDestination)
                    volume = fadeVolDestination;
            }
            audioSource.volume = volume;
        }
    }

    public void FadeAudioLoop(float volDestination, float volSlope)
    {
        fading = true;
        fadeVolDestination = volDestination;
        if (fadeVolDestination > volume)
            fadeVolSlope = volSlope;
        else if (fadeVolDestination < volume)
            fadeVolSlope = -volSlope;
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

    public void PauseLoop()
    {
        audioSource.Pause();
    }
    public void UnpauseLoop()
    {
        audioSource.UnPause();
    }

    public void StopAudioLoop()
    {
        pitchFadeOutSlopeCalc = UnityEngine.Random.Range(pitchFadeOutSlope.minValue * 0.001f, pitchFadeOutSlope.maxValue * 0.001f);
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