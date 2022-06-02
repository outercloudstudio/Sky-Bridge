using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace SkyBridge
{
    [System.Serializable]
    public class Connection
    {
        public enum ConnectionMode
        {
            OFFLINE,
            CONNECTING,
            CONNECTED,
            DISCONNECTED
        }

        public ConnectionMode connectionMode = ConnectionMode.OFFLINE;

        private Thread dataListenerThread;
        private Thread dataSenderThread;

        private TcpClient client;
        private NetworkStream networkStream;

        private byte[] sendBuffer = new byte[0];

        public delegate void PacketRecieved(Packet packet);
        public PacketRecieved onPacketRecieved;

        public Connection(TcpClient _client, NetworkStream _networkStream)
        {
            client = _client;
            networkStream = _networkStream;

            connectionMode = ConnectionMode.CONNECTED;

            StartThreads();
        }

        public Connection()
        {
            
        }

        public void Connect(string IP, int port)
        {
            connectionMode = ConnectionMode.CONNECTING;

            client = new TcpClient(IP, port);

            networkStream = client.GetStream();

            connectionMode = ConnectionMode.CONNECTED;

            StartThreads();
        }

        public void StartThreads()
        {
            dataSenderThread = new Thread(SendLoop);
            dataListenerThread = new Thread(ListenLoop);

            dataSenderThread.Start();
            dataListenerThread.Start();
        }

        public void SendLoop()
        {
            while (true)
            {
                if (sendBuffer.Length > 0)
                {
                    networkStream.Write(sendBuffer, 0, sendBuffer.Length);
                }
            }
        }

        public void ListenLoop()
        {
            while (true)
            {
                byte[] bytes = new byte[4096];

                int bytesRead = networkStream.Read(bytes, 0, bytes.Length);

                for (int readPos = 0; readPos < bytesRead;)
                {
                    byte[] packetLengthBytes = bytes[readPos..(readPos + 4)];
                    int packetLength = BitConverter.ToInt32(packetLengthBytes);

                    byte[] packetBytes = bytes[readPos..(readPos + packetLength)];

                    Packet packet = new Packet(packetBytes);

                    onPacketRecieved(packet);

                    readPos += packetLength;
                }
            }
        }

        public void Abort()
        {
            connectionMode = ConnectionMode.DISCONNECTED;

            if (dataListenerThread != null && dataListenerThread.IsAlive) dataListenerThread.Abort();
            if (dataSenderThread != null && dataSenderThread.IsAlive) dataSenderThread.Abort();

            if (client != null && networkStream != null && client.Connected)
            {
                client.Close();
                networkStream.Close();
            }
        }
    }
}
