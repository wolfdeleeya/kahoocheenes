using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Serializable]
    public enum UIComponentScreen
    {
        MainMenu,
        Settings,
        GameHub
    }

    [SerializeField] private UnityEventInteger onScreenChanged;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        musicSlider.value = GameSettingsManager.Instance.MusicVolume;
        sfxSlider.value = GameSettingsManager.Instance.SfxVolume;
    }


    public void ChangeScreen(int targetScreen) => onScreenChanged.Invoke(targetScreen);

    public void SetSfxVolume(float amount) => GameSettingsManager.Instance.SfxVolume = amount;

    public void SetMusicVolume(float amount) => GameSettingsManager.Instance.SfxVolume = amount;

    public void DisconnectFromGame() => NetworkControllerManager.Instance.DisconnectFromServer();

    public void ConnectToGame() => NetworkControllerManager.Instance.ConnectToServer();
    
    public void Quit() => Application.Quit();
}