using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public UnityEventInteger OnSceneChanged = new UnityEventInteger();

    private int _currentScene;

    public static SceneManager Instance { get; private set; }

    public int CurrentScene
    {
        get { return _currentScene; }

        private set
        {
            _currentScene = value;
            UnityEngine.SceneManagement.SceneManager.LoadScene(_currentScene);
        }
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded +=
            (scene, mode) => OnSceneChanged.Invoke(scene.buildIndex);
    }

    public void ChangeScene(Scene scene) => CurrentScene = (int) scene;

    public enum Scene
    {
        MainMenu,
        Gameplay
    }
}