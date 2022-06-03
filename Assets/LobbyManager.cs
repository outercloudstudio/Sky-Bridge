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

        public void HandlePacket(Connection connection, Packet packet)
        {
            
        }

        private void Update()
        {
            connection.Update();
        }

        private void OnDestroy()
        {
            if (connection != null) connection.Disconnect();
        }
    }
}
