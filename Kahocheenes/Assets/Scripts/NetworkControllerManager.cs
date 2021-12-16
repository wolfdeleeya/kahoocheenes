using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

public class NetworkControllerManager : MonoBehaviour
{
    public UnityEventInteger OnClientConnected;
    public UnityEvent OnGameDisconnected;

    [HideInInspector] public UnityEventString OnConnectionCodeReceived;

    private List<UnityEventControlsPair> _clientControlsEvents = new List<UnityEventControlsPair>();
    private WebSocket _serverSocket = new WebSocket("ws://rainy-carpal-leptoceratops.glitch.me");
    private string _connectionCode = String.Empty;
    private ConcurrentQueue<byte[]> _messageQueue = new ConcurrentQueue<byte[]>();

    private const int LEN_OF_SERVER_CODE = 4;

    public static NetworkControllerManager Instance { get; private set; }

    public string ConnectionCode
    {
        get { return _connectionCode; }

        private set
        {
            _connectionCode = value;
            Debug.Log("RECEIVED CODE IS: " + value);
            OnConnectionCodeReceived.Invoke(_connectionCode);
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

        _serverSocket.Connect();
        _serverSocket.OnMessage += (sender, e) => _messageQueue.Enqueue(e.RawData);
    }

    private void Update()
    {
        if (_messageQueue.TryDequeue(out var data))
            HandleMessage(data);
    }

    public void ConnectToServer()
    {
        var greetingsPacket = new byte[] {0};
        _serverSocket.Send(greetingsPacket); //request connection code
    }

    public void DisconnectFromServer()
    {
        if (ConnectionCode.Length == 0)
            return;

        var farewellPackage = new byte[LEN_OF_SERVER_CODE + 1];
        farewellPackage[0] = 0;
        for (int i = 0; i < LEN_OF_SERVER_CODE; ++i)
            farewellPackage[i + 1] = (byte) ConnectionCode[i];
        _serverSocket.Send(farewellPackage);
        ConnectionCode = String.Empty;
        _clientControlsEvents.Clear();
        OnGameDisconnected.Invoke();
    }

    private void HandleMessage(byte[] data)
    {
        if (data.Length == LEN_OF_SERVER_CODE + 1 && data[0] == 0) //check if this is a message with game's code
            ConnectionCode = CalculateStringFromBytes(data, 1);
        if (data.Length == 2 && data[0] == 0) //check if this is a client connected message
            ClientConnected(data[1]);
        if (data.Length == 4 && data[0] == 0) //check if this is a client command message
            HandleClientCommand(data);
    }

    private void ClientConnected(int clientID)
    {
        _clientControlsEvents.Add(new UnityEventControlsPair());
        OnClientConnected.Invoke(clientID);
    }

    private void HandleClientCommand(byte[] data)
    {
        int clientId = data[1];
        if (clientId > _clientControlsEvents.Count)
        {
            Debug.LogError("BAD CLIENT ID: CLIENT ID " + clientId + " IS NOT REGISTERED.");
            return;
        }

        ControlsPair controls;
        controls.Type = (ControlsType) data[2];
        controls.State = (ControlsState) data[3];
        _clientControlsEvents[clientId - 1].Invoke(controls);
    }

    public void ListenClientsControlsEvents(int id, UnityAction<ControlsPair> handler) =>
        _clientControlsEvents[id - 1].AddListener(handler);

    private void OnApplicationQuit()
    {
        DisconnectFromServer();
        _serverSocket.Close();
    }

    private static string CalculateStringFromBytes(byte[] bytes, int from = 0)
    {
        string result = "";
        for (int i = from; i < bytes.Length; ++i)
            result += (char) bytes[i];
        return result;
    }
}