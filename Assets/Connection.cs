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

        private Thread connectThread;

        private Thread dataListenerThread;
        private Thread dataSenderThread;

        private TcpClient client;
        private NetworkStream networkStream;

        public delegate void ConnectionModeUpdated(Connection connection, ConnectionMode connectionMode);
        public ConnectionModeUpdated onConnectionModeUpdated;

        public delegate void PacketRecieved(Connection connection, Packet packet);
        public PacketRecieved onPacketRecieved;

        public string IP;
        public int port;

        private List<Packet> sendQueue = new List<Packet>();

        public Connection()
        {
            
        }

        public void Connect(string _IP, int _port)
        {
            IP = _IP;
            port = _port;

            connectionMode = ConnectionMode.CONNECTING;
            if(onConnectionModeUpdated != null) onConnectionModeUpdated(this, connectionMode);

            connectThread = new Thread(ConnectThreaded);
            connectThread.Start();
        }

        public void Assign(TcpClient _client, NetworkStream _networkStream)
        {
            IP = ((IPEndPoint)_client.Client.RemoteEndPoint).Address.ToString();
            port = ((IPEndPoint)_client.Client.RemoteEndPoint).Port;

            client = _client;
            networkStream = _networkStream;

            connectionMode = ConnectionMode.CONNECTED;
            if (onConnectionModeUpdated != null) onConnectionModeUpdated(this, connectionMode);

            StartThreads();
        }

        public void ConnectThreaded()
        {
            client = new TcpClient(IP, port);

            networkStream = client.GetStream();

            connectionMode = ConnectionMode.CONNECTED;
            if (onConnectionModeUpdated != null) onConnectionModeUpdated(this, connectionMode);

            StartThreads();
        }

        public void StartThreads()
        {
            dataSenderThread = new Thread(SendLoop);
            dataListenerThread = new Thread(ListenLoop);

            dataSenderThread.Start();
            dataListenerThread.Start();
        }

        public void QueuePacket(Packet packet)
        {
            lock (sendQueue)
            {
                sendQueue.Add(packet);
            }
        }

        public void SendLoop()
        {
            try
            {
                while (true)
                {
                    if (sendQueue.Count > 0)
                    {
                        lock (sendQueue)
                        {
                            byte[] sendBuffer = new byte[0];

                            int packetsPacked = 0;

                            while (true)
                            {
                                Packet packet = sendQueue[0];

                                byte[] packetBytes = packet.ToBytes();

                                if (sendBuffer.Length + packetBytes.Length >= SkyBridge.bufferSize) break;

                                Debug.Log("Sending Packet " + packet.packetType + " to " + IP + ":" + port);

                                byte[] extendedBytes = new byte[sendBuffer.Length + packetBytes.Length];

                                Buffer.BlockCopy(sendBuffer, 0, extendedBytes, 0, sendBuffer.Length);

                                Buffer.BlockCopy(packetBytes, 0, extendedBytes, sendBuffer.Length, packetBytes.Length);

                                sendBuffer = extendedBytes;

                                packetsPacked++;

                                sendQueue.RemoveAt(0);

                                if (sendQueue.Count == 0) break;
                            }

                            if (packetsPacked == 0)
                            {
                                Packet packet = sendQueue[0];

                                Debug.LogWarning("Dropping Packet Because It Is Too Large To Send! " + packet.packetType + " Length: " + packet.ToBytes().Length);

                                sendQueue.RemoveAt(0);
                            }

                            networkStream.Write(sendBuffer, 0, sendBuffer.Length);
                        }
                    }

                    Thread.Sleep(Mathf.FloorToInt(1f / SkyBridge.sendRate * 1000f));
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public void ListenLoop()
        {
            try
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

                        Debug.Log("Recieved Packet " + packet.packetType + " from " + IP + ":" + port);
                        if (onPacketRecieved != null) onPacketRecieved(this, packet);

                        readPos += packetLength;
                    }
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            Debug.Log("Disconnected connection " + IP + ":" + port);

            connectionMode = ConnectionMode.DISCONNECTED;
            if (onConnectionModeUpdated != null) onConnectionModeUpdated(this, connectionMode);

            if (connectThread != null && connectThread.IsAlive) connectThread.Abort();
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
