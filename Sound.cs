using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    // Just to identify what kind of attack sound we are using here
    public string typeOfSound = "";
    public AudioSource audioSource;

    public AudioClip[] audioClips;
    public bool shouldAudioClipLoopFromStart = false;

    // If you only have one audioclip to play on your object, place it here :)
    public AudioClip specificAudioClip;
    void Start()
    {
        if (shouldAudioClipLoopFromStart)
        {
            PlayLoopingAudioClip();
        }
    }
    // Play a random sound from a list of audioclips
    public void PlayRandomSoundEffectFromList()
    {
        int randNumber = Random.Range(0, audioClips.Length);
        audioSource.PlayOneShot(audioClips[randNumber]);

    }
    // Play a pre-chosen audioclip
    public void PlaySpecificSound()
    {
        audioSource.PlayOneShot(specificAudioClip);
    }

    public void PlayLoopingAudioClip()
    {
        audioSource.loop = true;
        audioSource.PlayOneShot(specificAudioClip);
    }
}