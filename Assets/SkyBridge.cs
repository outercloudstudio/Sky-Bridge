using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace SkyBridge {
    public class SkyBridge : MonoBehaviour
    {
        public class Room
        {
            public string ID;
        }

        public static int bufferSize = 4096;
        public static int sendRate = 60;
        public static float timeout = 30;
        public static float keepalive = 5;

        public static bool isHost = false;

        public static Room currentRoom;

        public static Connection bridgeServerConncection;
        public static Connection hostConncection;

        public static int maxPlayers = 8;

        public static Connection[] connections;

        public static Thread[] listenThreads;

        private void Start()
        {
            connections = new Connection[maxPlayers];
            listenThreads = new Thread[maxPlayers];

            if (isHost)
            {
                bridgeServerConncection.onPacketRecieved = HandleBridgeServerPacket;
            }
        }

        private void Update()
        {
            bridgeServerConncection.Update(Time.deltaTime);

            foreach (Connection connection in connections)
            {
                if (connection != null) connection.Update(Time.deltaTime);
            }
        }

        public static int GetOpenConnectionIndex()
        {
            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i] == null) return i;
            }

            return -1;
        }

        public void ListenForConnection(int index, TcpListener listener)
        {
            listener.Start();

            Debug.Log("Started Listener!");

            TcpClient client = listener.AcceptTcpClient();

            Debug.Log("Accpeted client!");

            NetworkStream networkStream = client.GetStream();

            connections[index].Assign(client, networkStream);
        }

        public void HandlePacket(Connection connection, Packet packet)
        {
            
        }

        public static int GetOpenTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);

            l.Start();

            int port = ((IPEndPoint)l.LocalEndpoint).Port;

            l.Stop();

            return port;
        }

        public void HandleBridgeServerPacket(Connection connection, Packet packet)
        {
            if (packet.packetType == "JOIN_ATTEMPT")
            {
                string ID = packet.GetString(0);
                string IP = packet.GetString(1);

                int openIndex = GetOpenConnectionIndex();

                if (openIndex == -1)
                {
                    connection.SendPacket(new Packet("JOIN_ATTEMPT_REJECTED").AddValue(ID).AddValue("Game Full"));

                    return;
                }

                int port = GetOpenTcpPort();

                TcpListener listener = new TcpListener(IPAddress.Parse(IP), port);

                connections[openIndex] = new Connection();

                connections[openIndex].onPacketRecieved = HandlePacket;

                listenThreads[openIndex] = new Thread(() => { ListenForConnection(openIndex, listener); });
                listenThreads[openIndex].Start();

                Debug.Log("Opened connection at " + port + " for " + IP);

                connection.SendPacket(new Packet("JOIN_ATTEMPT_ACCEPTED").AddValue(ID).AddValue(port));
            }
        }
    }
}
