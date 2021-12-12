using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    public static AudioManager Instance { get; private set; }

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
        GameSettingsManager.Instance.OnSfxVolumeChanged.AddListener(ChangeSfxVolume);
        GameSettingsManager.Instance.OnMusicVolumeChanged.AddListener(ChangeMusicVolume);
    }

    private void ChangeSfxVolume(float value) => sfxSource.volume = value;

    private void ChangeMusicVolume(float value) => musicSource.volume = value;
}