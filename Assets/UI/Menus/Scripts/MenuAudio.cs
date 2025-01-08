using System;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    // Sounds
    [Header("Sounds")]
    [SerializeField] private AudioSource soundEffects;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip back;

    private void Start()
    {
        // If there's no audio source or there's no audio clip then skip this part
        if (music == null || music.clip == null)
        {
            return;
        }

        // If not already playing, then play the main menu music
        if (!music.isPlaying)
        {
            music.Play();
        }
    }
    
    /// <summary>
    /// Plays the back sound.
    /// </summary>
    public void PlayBackSound()
    {
        soundEffects.clip = back;
        if (!soundEffects.isPlaying)
        {
            soundEffects.Play();
        }
    }
    
    /// <summary>
    /// Plays the click sound.
    /// </summary>
    public void PlayClickSound()
    {
        soundEffects.clip = click;
        if (!soundEffects.isPlaying)
        {
            soundEffects.Play();
        }
    }
}
