using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyBridge
{
    public class LobbyManager : MonoBehaviour
    {
        public Connection connection;

        void Start()
        {
            connection = new Connection();
            connection.Connect("localhost", 25565);
        }
    }
}
