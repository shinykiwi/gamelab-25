using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [SerializeField] private string parameterName;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        audioMixerGroup.audioMixer.GetFloat(parameterName, out var initialVolume);
        //Debug.Log(parameterName + initialVolume);
        //Debug.Log(slider.value);
        slider.value = initialVolume;
    }

    public void OnValueChange(float value)
    {
        audioMixerGroup.audioMixer.SetFloat(parameterName, value);
    }
}
