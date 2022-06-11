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

        public void Host(int maxPlayers)
        {
            connection.SendPacket(new Packet("HOST").AddValue(maxPlayers));
        }

        public void Join(string ID)
        {
            SkyBridge.currentRoom = new SkyBridge.Room()
            {
                ID = ID
            };

            connection.SendPacket(new Packet("JOIN").AddValue(ID));
        }

        public void HandlePacket(Connection connection, Packet packet)
        {
            if (packet.packetType == "HOST_INFO")
            {
                string ID = packet.GetString(0);
                string clientID = packet.GetString(1);

                SkyBridge.isHost = true;

                SkyBridge.currentRoom = new SkyBridge.Room()
                {
                    ID = ID
                };

                SkyBridge.client = new SkyBridge.Client(clientID);
                SkyBridge.connection = connection;

                SceneManager.LoadScene("Game");
            }else if (packet.packetType == "JOIN_REJECTED")
            {
                string reason = packet.GetString(0);

                Debug.LogWarning("Failed to join room! " + reason);
            }
            else if (packet.packetType == "JOIN_ACCEPTED")
            {
                string clientID = packet.GetString(0);

                SkyBridge.currentRoom = new SkyBridge.Room()
                {
                    ID = "Unkown"
                };

                SkyBridge.client = new SkyBridge.Client(clientID);
                SkyBridge.connection = connection;

                SceneManager.LoadScene("Game");
            }
        }

        private void Update()
        {
            connection.Update(Time.deltaTime);
        }
    }
}
