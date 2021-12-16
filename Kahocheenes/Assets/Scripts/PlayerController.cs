using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private List<GameObject> controlsHandlerPrefabs;
    private AbstractControlsHandler _currentControlsHandler;
    private Transform _transform;

    public int Id { get; private set; }

    private void Awake()
    {
        _transform = transform;
    }

    public void Initialize(int id)
    {
        Id = id;
        NetworkControllerManager.Instance.ListenClientsControlsEvents(Id, HandleControls);
        SceneManager.Instance.OnSceneChanged.AddListener(UpdateHandlerBasedOnScene);
        UpdateHandlerBasedOnScene(SceneManager.Instance.CurrentScene);
    }

    public void HandleControls(ControlsPair pair) => _currentControlsHandler.HandleControls(pair);

    public void UpdateHandlerBasedOnScene(int scene)
    {
        if (_currentControlsHandler)
            Destroy(_currentControlsHandler.gameObject);
        _currentControlsHandler = Instantiate(controlsHandlerPrefabs[scene], _transform)
            .GetComponent<AbstractControlsHandler>();
        _currentControlsHandler.Initialize(Id);
    }

    private void OnDestroy()
    {
        SceneManager.Instance.OnSceneChanged.RemoveListener(UpdateHandlerBasedOnScene);
    }
}