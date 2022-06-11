using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace SkyBridge {
    public class SkyBridge : MonoBehaviour
    {
        public class Client
        {
            public string ID;

            public Client(string _ID)
            {
                ID = _ID;
            }
        }

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

        public static Client client;
        public static Connection connection;

        private void Update()
        {
            connection.Update(Time.deltaTime);
        }

        public void HandlePacket(Connection connection, Packet packet)
        {
            
        }
    }
}
