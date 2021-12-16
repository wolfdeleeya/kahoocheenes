using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private GameObject playerControllerPrefab;

    private List<GameObject> _spawnedPlayerControllers = new List<GameObject>();

    private Transform _transform;
    public static ClientManager Instance { get; private set; }

    public int NumOfPlayers
    {
        get => _spawnedPlayerControllers.Count;
    }

    public GameObject GetPlayerController(int index) => _spawnedPlayerControllers[index];

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
        _transform = transform;
    }

    public void OnClientConnected(int clientId)
    {
        var player = Instantiate(playerControllerPrefab, _transform).GetComponent<PlayerController>();
        player.Initialize(clientId);
        _spawnedPlayerControllers.Add(player.gameObject);
    }

    public void OnGameDisconnected()
    {
        foreach (var player in _spawnedPlayerControllers)
            Destroy(player);
        _spawnedPlayerControllers.Clear();
    }
}