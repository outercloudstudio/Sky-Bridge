using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyBridge {
    public class SkyBridge
    {
        public static int bufferSize = 4096;
        public static int sendRate = 60;

        public static Connection hostBridgeServerConnection;

        public static int maxPlayers = 8;
    }
}
