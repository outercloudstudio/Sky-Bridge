using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyBridge
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager me;

        private void Awake()
        {
            if(me == null)
            {
                me = this;

                DontDestroyOnLoad(gameObject);
            }
            else{
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if(SkyBridge.roomMode == SkyBridge.RoomMode.HOST)
            {
                SkyBridge.hostBridgeServerConnection.onPacketRecieved = HandlePacket;
            }
        }

        public void HandlePacket(Connection connection, Packet packet)
        {
            if(packet.packetType == Packet.PacketType.SIGNAL_JOIN)
            {
                string IP = (string)packet.values[0].unserializedValue;
                int port = (int)packet.values[1].unserializedValue;

                connection.QueuePacket(new Packet(Packet.PacketType.ERROR).AddValue(002).AddValue("Room Full").AddValue(IP).AddValue(port));
            }
        }
    }
}
