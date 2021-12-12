using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    public void ChangeScreen(int targetScreen) => onScreenChanged.Invoke(targetScreen);

    public void SetSfxVolume(float amount) => GameSettingsManager.Instance.SfxVolume = amount;

    public void SetMusicVolume(float amount) => GameSettingsManager.Instance.SfxVolume = amount;

    public void SetGameCode(string code) => gameCodeText.text = code;

    public void Quit() => Application.Quit();
}