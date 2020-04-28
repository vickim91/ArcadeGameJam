using UnityEngine;
using UnityEngine.Audio;

public class VoicePlayer: MonoBehaviour
{
    AudioSource audioSource;
    private float pitchMin;
    private float pitchMax;
    private float volume;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (KillAllVoices.killAllVoicesStatic)
        {
            audioSource.Stop();
        }
    }

    public void PlaySound(int selectedFile, AudioEvent.Sound[] sound, AudioMixerGroup output, AudioEvent.Parameters pitchAndVolLogic)
    {
        audioSource.outputAudioMixerGroup = output;
        audioSource.clip = sound[selectedFile].soundFile;
        SetPitchAndVolume(selectedFile, sound, pitchAndVolLogic);
        audioSource.Play();
    }

    private void SetPitchAndVolume(int selectedFile, AudioEvent.Sound[] sound, AudioEvent.Parameters parameterOverwrite)
    {
        switch (parameterOverwrite)
        {
            case AudioEvent.Parameters.Default:
                pitchMin = 1;
                pitchMax = 1;
                volume = 0.5f;
                break;
            case AudioEvent.Parameters.Individual:
                pitchMin = sound[selectedFile].pitch.minValue;
                pitchMax = sound[selectedFile].pitch.maxValue;
                volume = sound[selectedFile].volume;
                break;
            case AudioEvent.Parameters.InheritFromFirstElement:
                pitchMin = sound[0].pitch.minValue;
                pitchMax = sound[0].pitch.maxValue;
                volume = sound[0].volume;
                break;
        }
        InspectorFeedbackForParameterOverwrite(selectedFile, sound);
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.volume = volume;
    }

    private void InspectorFeedbackForParameterOverwrite(int selectedFile, AudioEvent.Sound[] sound)
    {
        sound[selectedFile].pitch.minValue = pitchMin;
        sound[selectedFile].pitch.maxValue = pitchMax;
        sound[selectedFile].volume = volume;
    }

    public void FadeOutThenStop(float fadeSlope)
    {
        audioSource.volume -= fadeSlope;
        if (audioSource.volume <= 0)
        {
            audioSource.Stop();
        }
    }

    public bool isPlaying()
    {
        return audioSource.isPlaying;
    }
}