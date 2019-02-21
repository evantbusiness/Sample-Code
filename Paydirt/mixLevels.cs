using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class mixLevels : MonoBehaviour {

    public AudioMixer masterMixer;
    public AudioMixerSnapshot firstLayer;
    public AudioMixerSnapshot muteAll;

    public void layerLevels(AudioMixerSnapshot snapshot)
    {
        snapshot.TransitionTo(1);
    }

    public void startMusic()
    {
        firstLayer.TransitionTo(0);
    }

    public void mute()
    {
        muteAll.TransitionTo(0);
    }
}
