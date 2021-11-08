using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    public string ServerIPAddress = "127.0.0.1";
    public int ServerPort = 5046;
    
    public int MyPort = 5045;
    private static readonly int _dataBufferSize = 4096;

    private TcpListener _controllerListener;
    private TcpClient _gameSocket;
    private List<Client> _connectedClients;
    
    public static NetworkGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;
    }

    public void Connect()
    {
        
    }
}
