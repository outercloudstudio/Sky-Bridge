using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyBridge
{
    public class LobbyManager : MonoBehaviour
    {
        public Connection connection;

        public Packet packet;

        void Start()
        {
            connection = new Connection();
            connection.onPacketRecieved += _packet =>
            {
                Debug.Log(_packet);
                packet = _packet;
            };

            connection.Connect("localhost", 25565);
        }
    }
}
