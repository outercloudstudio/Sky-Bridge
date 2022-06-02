using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyBridge {
    public class SkyBridge
    {
        public enum ConnectionMode
        {
            OFFLINE,
            CONNECTING_TO_BRIDGE_SERVER,
            FAILED_TO_CONNECT_TO_BRIDGE_SERVER,
            CONNECTED_TO_BRIDGE_SERVER,
            DISCONNECTED_FROM_BRIDGE_SERVER,
            STARTING_HOST,
            HOSTING,
            CONNECTING_TO_HOST,
            FAILED_TO_CONNECT_TO_HOST,
            CONNECTED,
            DISCONNECTED
        }

        public ConnectionMode connectionMode = ConnectionMode.OFFLINE;
    }
}
