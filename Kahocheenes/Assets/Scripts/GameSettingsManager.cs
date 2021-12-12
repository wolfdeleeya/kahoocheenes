using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    [HideInInspector] public UnityEventFloat OnMusicVolumeChanged;
    [HideInInspector] public UnityEventFloat OnSfxVolumeChanged;

    private float _musicVolume;
    private float _sfxVolume;

    private const string MusicVolumePrefsKey = "MusicVolume";
    private const string SfxVolumePrefsKey = "SfxVolume";

    public static GameSettingsManager Instance { get; private set; }

    public float MusicVolume
    {
        get { return _musicVolume; }

        set
        {
            _musicVolume = Mathf.Clamp01(value);
            OnMusicVolumeChanged.Invoke(_musicVolume);
        }
    }

    public float SfxVolume
    {
        get { return _sfxVolume; }

        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            OnSfxVolumeChanged.Invoke(_sfxVolume);
        }
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        MusicVolume = PlayerPrefs.GetFloat(MusicVolumePrefsKey, 0.5f);
        SfxVolume = PlayerPrefs.GetFloat(SfxVolumePrefsKey, 0.5f);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat(MusicVolumePrefsKey, MusicVolume);
        PlayerPrefs.SetFloat(SfxVolumePrefsKey, SfxVolume);
    }
}