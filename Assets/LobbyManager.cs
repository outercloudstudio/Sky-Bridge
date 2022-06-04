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

        public void Join()
        {
            connection.SendPacket(new Packet("JOIN"));
        }

        public void HandlePacket(Connection connection, Packet packet)
        {
            if(packet.packetType == "HOST_INFO")
            {
                string ID = packet.GetString(0);

                SkyBridge.isHost = true;

                SkyBridge.currentRoom = new SkyBridge.Room()
                {
                    ID = ID
                };

                SkyBridge.bridgeServerConncection = connection;

                SceneManager.LoadScene("Game");
            }
        }

        private void Update()
        {
            connection.Update();
        }
    }
}
