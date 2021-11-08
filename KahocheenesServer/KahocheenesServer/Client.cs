using System;
using System.Net.Sockets;

namespace KahocheenesServer
{
    public class Client
    {
        private static readonly int _dataBufferSize = 4096;

        public TCP Tcp
        {
            get => _tcp;
            private set => _tcp = value;
        }

        public int Id
        {
            get => _id;
            private set => _id = value;
        }

        private TCP _tcp;
        private int _id;

        public Client(int id)
        {
            Id = id;
            Tcp = new TCP(id);
        }


        public class TCP
        {
            public TcpClient Socket;

            private readonly int _id;
            private NetworkStream _stream;
            private Byte[] _receiveBuffer;

            public TCP(int id)
            {
                _id = id;
            }

            public void Connect(TcpClient socket)
            {
                Socket = socket;
                Socket.ReceiveBufferSize = _dataBufferSize;
                Socket.SendBufferSize = _dataBufferSize;

                _stream = Socket
                    .GetStream(); //Gets network stream that is used to send and receive data from client's socket.
                _receiveBuffer = new byte[_dataBufferSize];

                _stream.BeginRead(_receiveBuffer, 0, _dataBufferSize, ReceiveCallback,
                    null); //Sets callback that will be executed once that receive buffer is filled
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int numOfBytes = _stream.EndRead(result);
                    if (numOfBytes == 0) //All data is received and the client's socket connection has shut down.
                    {
                        //TODO: DISCONNECT
                        return;
                    }

                    byte[] data = new byte[numOfBytes];
                    Array.Copy(_receiveBuffer, data, numOfBytes);

                    //TODO: HANDLE DATA
                    _stream.BeginRead(_receiveBuffer, 0, _dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error receiving client {0}'s data packet!", _id);
                }
            }
        }
    }
}