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
    }
}
