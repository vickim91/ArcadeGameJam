using UnityEngine;
using UnityEngine.Audio;

public class MuteTool : MonoBehaviour
{
    public bool activateGlobalMuteSetting;
    public enum Mute
    {
        None,
        All,
        Sounds,
        Music
    }
    public Mute mute;

    public AudioMixerSnapshot noMute;
    public AudioMixerSnapshot muteAll;
    public AudioMixerSnapshot muteSounds;
    public AudioMixerSnapshot muteMusic;

    private Mute snapshotThisFrame;
    private Mute snapshotLastFrame;

    private void Update()
    {
        if (activateGlobalMuteSetting)
        {
            snapshotThisFrame = mute;
            if (snapshotLastFrame != snapshotThisFrame)
            {
                switch (mute)
                {
                    case Mute.None:
                        noMute.TransitionTo(0f);
                        break;
                    case Mute.All:
                        muteAll.TransitionTo(0f);
                        break;
                    case Mute.Sounds:
                        muteSounds.TransitionTo(0f);
                        break;
                    case Mute.Music:
                        muteMusic.TransitionTo(0f);
                        break;
                }
            }
            snapshotLastFrame = snapshotThisFrame;
        }
    }
}
