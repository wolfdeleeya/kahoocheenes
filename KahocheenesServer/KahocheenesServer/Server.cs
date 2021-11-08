using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace KahocheenesServer
{
    public class Server
    {
        public static int ServersPort { get; private set; }
        public static int ControllersPort { get; private set; }

        private static Dictionary<int, Client> _availableGamesDictionary;
        private static TcpListener _serversListener;
        private static TcpListener _controllersListener;

        public static void Start(int serversPortNumber, int controllersPortNumber)
        {
            ServersPort = serversPortNumber;
            ControllersPort = controllersPortNumber;
            
            _serversListener = new TcpListener(IPAddress.Any, ServersPort);   //Listen for client activity on all network interfaces and on set port
            _controllersListener = new TcpListener(IPAddress.Any, ControllersPort);
            
            _serversListener.Start();      //Starts listening for incoming connection requests.
            _controllersListener.Start();
            
            _serversListener.BeginAcceptTcpClient(new AsyncCallback(TCPServerConnectCallback), null);        //Which callback should server call after the client has been accepted
            _controllersListener.BeginAcceptTcpClient(new AsyncCallback(TCPControllerConnectCallback), null);  
            
            Console.WriteLine("Server listener started on port {0} and controller listener started on port {1}...", ServersPort, ControllersPort);
        }

        private static void TCPServerConnectCallback(IAsyncResult result)
        {
            TcpClient client = _serversListener.EndAcceptTcpClient(result);
            _serversListener.BeginAcceptTcpClient(new AsyncCallback(TCPServerConnectCallback), null);  
            //TODO: Send welcome server packet with correct room code
        }
        
        private static void TCPControllerConnectCallback(IAsyncResult result)
        {
            TcpClient client = _controllersListener.EndAcceptTcpClient(result);
            _controllersListener.BeginAcceptTcpClient(new AsyncCallback(TCPControllerConnectCallback), null);  
            //TODO: Send welcome controller packet and pass him ip address and port of required server once the controller sends correct code
        }
    }
}