using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class OptionsMenu : MonoBehaviour
{

    // Audio Mixers
    [Header("Volume Sliders")] 
    [SerializeField] private VolumeSlider master;
    [SerializeField] private VolumeSlider soundEffects;
    [SerializeField] private VolumeSlider music;
    [SerializeField] private VolumeSlider mainMenuMusic;
    
    /// <summary>
    /// Called by a volume slider in the main menu UI.
    /// </summary>
    public void OnVolumeChange(float value)
    {
        
    }
}
