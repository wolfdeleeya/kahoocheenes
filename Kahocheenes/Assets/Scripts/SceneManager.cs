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
            OnSceneChanged.Invoke((int) _currentScene);
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

    public void ChangeScene(int sceneIndex)
    {
        CurrentScene = sceneIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}