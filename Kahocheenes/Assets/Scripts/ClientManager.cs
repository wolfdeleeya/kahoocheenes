using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public UnityEventGameObject OnPlayerCreated = new UnityEventGameObject();
    [SerializeField] private GameObject playerControllerPrefab;

    private List<PlayerController> _spawnedPlayerControllers;

    public static ClientManager Instance { get; private set; }

    public int NumOfPlayers
    {
        get => _spawnedPlayerControllers.Count;
    }

    public PlayerController GetPlayerController(int index) => _spawnedPlayerControllers[index];

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
        _spawnedPlayerControllers = new List<PlayerController>();
    }

    public void OnClientConnectedHandler(int clientId)
    {
        var player = Instantiate(playerControllerPrefab).GetComponent<PlayerController>();
        player.Initialize(clientId);
        _spawnedPlayerControllers.Add(player);
        OnPlayerCreated.Invoke(player.gameObject);
    }

    public void OnGameDisconnected()
    {
        foreach (var player in _spawnedPlayerControllers)
            Destroy(player.gameObject);
        _spawnedPlayerControllers.Clear();
    }
}