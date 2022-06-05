using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SkyBridge
{
    public class LobbyManager : MonoBehaviour
    {
        public Connection connection;

        public void ConnectToBridgeServer()
        {
            connection = new Connection();

            connection.onPacketRecieved += HandlePacket;

            connection.Connect("localhost", 25565);
        }

        public void Host()
        {
            connection.SendPacket(new Packet("HOST"));
        }

        public void Join(string ID)
        {
            connection.SendPacket(new Packet("JOIN").AddValue(ID));
        }

        public void HandlePacket(Connection connection, Packet packet)
        {
            if (packet.packetType == "HOST_INFO")
            {
                string ID = packet.GetString(0);

                SkyBridge.isHost = true;

                SkyBridge.currentRoom = new SkyBridge.Room()
                {
                    ID = ID
                };

                SkyBridge.bridgeServerConncection = connection;

                SceneManager.LoadScene("Game");
            }else if (packet.packetType == "JOIN_ATTEMPT_REJECTED")
            {
                string reason = packet.GetString(0);

                Debug.LogWarning("Failed to join room! " + reason);
            }
            else if (packet.packetType == "JOIN_ATTEMPT_ACCEPTED")
            {
                connection.Disconnect("Uneeded Sky Bridge Server Connection");

                string IP = packet.GetString(0);
                int port = packet.GetInt(1);

                SkyBridge.connections = new Connection[SkyBridge.maxPlayers];

                Connection hostConnection = new Connection();
                hostConnection.onPacketRecieved = HandlePacket;

                SkyBridge.connections[0] = hostConnection;

                hostConnection.Connect(IP, port);

                SkyBridge.currentRoom = new SkyBridge.Room() {
                    ID = "Unkown"
                };

                SceneManager.LoadScene("Game");
            }
        }

        private void Update()
        {
            connection.Update(Time.deltaTime);
        }
    }
}
