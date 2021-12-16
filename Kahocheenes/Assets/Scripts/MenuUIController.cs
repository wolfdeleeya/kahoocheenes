using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [Serializable]
    public enum UIComponentScreen
    {
        MainMenu,
        Settings,
        GameHub
    }

    [SerializeField] private UnityEventInteger onScreenChanged;

    [SerializeField] private MenuUIAnimator animator;

    [SerializeField] private TextMeshProUGUI gameCodeText;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        musicSlider.value = GameSettingsManager.Instance.MusicVolume;
        sfxSlider.value = GameSettingsManager.Instance.SfxVolume;
        NetworkControllerManager.Instance.OnConnectionCodeReceived.AddListener(SetGameCode);
    }

    private void OnDestroy()
    {
        NetworkControllerManager.Instance.OnConnectionCodeReceived.RemoveListener(SetGameCode);
    }

    public void ChangeScreen(int targetScreen) => onScreenChanged.Invoke(targetScreen);

    public void SetSfxVolume(float amount) => GameSettingsManager.Instance.SfxVolume = amount;

    public void SetMusicVolume(float amount) => GameSettingsManager.Instance.SfxVolume = amount;

    public void SetGameCode(string code) => gameCodeText.text = code;

    public void Quit() => Application.Quit();
}