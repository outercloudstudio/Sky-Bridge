using System.Collections;
using System.Collections.Generic;
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

        public static bool isHost = false;

        public static Room currentRoom;

        public static Connection bridgeServerConncection;

        public static int maxPlayers = 8;

        public static Connection[] connections;

        private void Start()
        {
            connections = new Connection[maxPlayers];

            if (isHost)
            {
                bridgeServerConncection.onPacketRecieved = HandleBridgeServerPacket;
            }
        }

        private void Update()
        {
            bridgeServerConncection.Update();

            foreach (Connection connection in connections)
            {
                if (connection != null) connection.Update();
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

        public void HandleBridgeServerPacket(Connection connection, Packet packet)
        {
            if (packet.packetType == "JOIN_ATTEMPT")
            {
                string ID = packet.GetString(0);
                string IP = packet.GetString(1);

                if(GetOpenConnectionIndex() == -1)
                {
                    connection.SendPacket(new Packet("JOIN_ATTEMPT_REJECTED").AddValue(ID).AddValue("Game Full"));

                    return;
                }

                connection.SendPacket(new Packet("JOIN_ATTEMPT_REJECTED").AddValue(ID).AddValue("Game Full"));
            }
        }
    }
}
