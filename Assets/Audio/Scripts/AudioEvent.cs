using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioEvent : MonoBehaviour
{
    private int selectedFile = -1;
    private int prevFileSelection = -1;

    [System.Serializable]
    public class Sound
    {
        public AudioClip soundFile;
        [MinMaxRange(0.5f, 2)]
        public RangedFloat pitch;
        [HideInInspector]
        public RangedFloat initialPitch;
        [Range(0, 1)]
        public float volume;
        [HideInInspector]
        public float initialVolume;
    }
    public Sound[] sound;
    public enum Parameters
    {
        Individual,
        InheritFromFirstElement,
        Default
    }
    public Parameters pitchAndVolLogic;

    public AudioMixerGroup output;
    public GameObject voicePrefab;
    public int voiceMax = 1;
    private int currentVoice = -1;
    private GameObject[] voice;
    private VoicePlayer[] voicePlayer;

    public enum Kill 
    {
        First,
        Last
    }
    public Kill kill;

    public enum Seq
    {
        Random,
        Sequence,
        RandomNoRepeat
    }
    public Seq seq;

    public bool fadeOutWhenStopped;
    [Range(0.0001f, 0.1f)]
    public float fadeOutSlopeWhenStopped;
    private bool stopping;



    public void Start()
    {
        for (int i = 0; i < sound.Length; i++)
        {
            sound[i].initialPitch = sound[i].pitch;
            sound[i].initialVolume = sound[i].volume;
        }
        BuildVoices();
    }

    public void BuildVoices()
    {
        voice = new GameObject[voiceMax];
        for (int i = 0; i < voiceMax; i++)
        {
            voice[i] = Instantiate(voicePrefab, transform);
            voice[i].transform.parent = transform;
            voice[i].name = "voice" + i;
        }
        voicePlayer = new VoicePlayer[voiceMax];
    }

    private void Update()
    {
        if (stopping)
        {
            float fadeOutSlope;
            if (fadeOutWhenStopped)
                fadeOutSlope = fadeOutSlopeWhenStopped;
            else
                fadeOutSlope = 1;
            for (int i = 0; i < voiceMax; i++)
            {
                if (voicePlayer[i] != null)
                    voicePlayer[i].FadeOutThenStop(fadeOutSlope);
            }
        }
    }

    public void StopSoundAllVoices()
    {
        stopping = true;
    }

    public void TriggerAudioEvent()
    {
        stopping = false;
        PrepareVoicing();
        if (kill == Kill.Last && voicePlayer[currentVoice].isPlaying())
        {
            return;
        }
        ChooseSequence();
        voicePlayer[currentVoice].PlaySound(selectedFile, sound, output, pitchAndVolLogic);
    }

    private void PrepareVoicing()
    {
        currentVoice += 1;
        if (currentVoice > voiceMax - 1)
        {
            currentVoice = 0;
        }
        voicePlayer[currentVoice] = voice[currentVoice].GetComponent<VoicePlayer>();
    }

    private void ChooseSequence()
    {
        switch(seq)
        {
            case Seq.Random:
                selectedFile = Random.Range(0, sound.Length);
                break;
            case Seq.Sequence:
                selectedFile += 1;
                if (selectedFile > sound.Length - 1)
                {
                    selectedFile = 0;
                }
                break;
            case Seq.RandomNoRepeat:
                while (selectedFile == prevFileSelection)
                {
                    selectedFile = Random.Range(0, sound.Length);
                }
                prevFileSelection = selectedFile;
                break;
        }
    }

    // previewing audio events: locked to random-sequence & 1 voice 
    public void PreviewAudioEvent(AudioSource audioSource)
    {
        selectedFile = Random.Range(0, sound.Length);
        audioSource.clip = sound[selectedFile].soundFile;
        audioSource.pitch = Random.Range(sound[selectedFile].pitch.minValue, sound[selectedFile].pitch.maxValue);
        audioSource.volume = sound[selectedFile].volume;
        audioSource.Play();
    }
}