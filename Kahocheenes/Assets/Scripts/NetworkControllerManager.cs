using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class NetworkControllerManager : MonoBehaviour
{
    [SerializeField] private UnityEventString onConnectionCodeReceived;
    public static NetworkControllerManager Instance { get; private set; }

    private WebSocket _serverSocket;

    private const int LEN_OF_SERVER_CODE = 4;

    private string _connectionCode = String.Empty;

    private ConcurrentQueue<byte[]> _messageQueue;

    public string ConnectionCode
    {
        get { return _connectionCode; }

        private set
        {
            _connectionCode = value;
            Debug.Log("RECEIVED CODE IS: " + value);
            onConnectionCodeReceived.Invoke(_connectionCode);
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
        _messageQueue = new ConcurrentQueue<byte[]>();
        
        _serverSocket = new WebSocket("ws://rainy-carpal-leptoceratops.glitch.me");
        _serverSocket.Connect();
        _serverSocket.OnMessage += (sender, e) => _messageQueue.Enqueue(e.RawData);
    }

    private void Update()
    {
        if(_messageQueue.TryDequeue(out var data))
            HandleMessage(data);
    }

    public void ConnectToServer()
    {
        var greetingsPacket = new byte[] {0};
        _serverSocket.Send(greetingsPacket); //request connection code
    }

    public void DisconnectFromServer()
    {
        if(ConnectionCode.Length == 0)
            return;
        
        var farewellPackage = new byte[LEN_OF_SERVER_CODE + 1];
        farewellPackage[0] = 0;
        for (int i = 0; i < LEN_OF_SERVER_CODE; ++i)
            farewellPackage[i + 1] = (byte) ConnectionCode[i];
        _serverSocket.Send(farewellPackage);
        ConnectionCode = String.Empty;
    }

    private void HandleMessage(byte[] data)
    {
        if (data.Length == LEN_OF_SERVER_CODE + 1 && data[0] == 0) //check if this is a message with game's code
            ConnectionCode = CalculateStringFromBytes(data, 1);
        if (data.Length == 2 && data[0] == 0) //check if this is a client connected message
            ClientConnected(data[1]);
        if (data.Length == 3 && data[0] == 0) //check if this is a client command message
            ClientCommandSent(data[1], data[2]);
    }

    private void ClientConnected(int clientID)
    {
    }

    private void ClientCommandSent(int clientID, int command)
    {
    }

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