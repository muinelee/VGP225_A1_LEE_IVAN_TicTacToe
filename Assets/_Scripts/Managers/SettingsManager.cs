using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SettingsManager : Singleton<SettingsManager>
{
    public float MasterVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public float MusicVolume { get; private set; }

    public event Action OnSettingsChanged;

    public void Start()
    {
        LoadVolumeSettings();        
    }

    public void LoadVolumeSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("Master", 0.5f);
        SFXVolume = PlayerPrefs.GetFloat("SFX", 0.5f);
        MusicVolume = PlayerPrefs.GetFloat("Music", 0.5f);

        OnSettingsChanged?.Invoke();
    }

    public void SaveVolumeSettings(float master, float sfx, float music)
    {
        MasterVolume = master;
        SFXVolume = sfx;
        MusicVolume = music;

        PlayerPrefs.SetFloat("Master", master);
        PlayerPrefs.SetFloat("SFX", sfx);
        PlayerPrefs.SetFloat("Music", music);
        PlayerPrefs.Save();

        OnSettingsChanged?.Invoke();
    }
}

