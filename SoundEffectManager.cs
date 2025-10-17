using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SoundEffectManager : MonoBehaviour
{
    [Header("Master Volume")]
    [SerializeField] private Slider masterVolumeSlider;

    [Header("Audio Sources Parent")]
    [SerializeField] private List<Transform> audioSourceParents;

    private List<AudioSource> allAudioSources;
    private const string MASTER_VOLUME_PREFS_KEY = "MasterVolume";

    void Awake()
    {
        allAudioSources = new List<AudioSource>();

        if (audioSourceParents == null || audioSourceParents.Count == 0)
        {
            Debug.LogError("No Audio Source Parents have been assigned in the SoundEffectManager inspector!");
            return;
        }

        foreach (Transform parent in audioSourceParents)
        {
            if (parent != null)
            {
                allAudioSources.AddRange(parent.GetComponentsInChildren<AudioSource>(true));
            }
        }
        
        if (masterVolumeSlider == null)
        {
            Debug.LogError("Master Volume Slider is not assigned in the SoundEffectManager inspector!");
            return;
        }

        float savedVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_PREFS_KEY, 1.0f);
        
        masterVolumeSlider.SetValueWithoutNotify(savedVolume);
        SetMasterVolume(savedVolume);

        masterVolumeSlider.onValueChanged.AddListener(OnMasterSliderValueChanged);
    }
    
    private void OnMasterSliderValueChanged(float value)
    {
        SetMasterVolume(value);
        PlayerPrefs.SetFloat(MASTER_VOLUME_PREFS_KEY, value);
    }

    public void SetMasterVolume(float volume)
    {
        foreach (AudioSource source in allAudioSources)
        {
            source.volume = volume;
        }
    }
}

