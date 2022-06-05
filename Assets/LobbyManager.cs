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
                Debug.LogWarning("Failed to join room!");
            }
        }

        private void Update()
        {
            connection.Update();
        }
    }
}
