using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyBridge
{
    public class LobbyManager : MonoBehaviour
    {
        public Connection connection;

        public Packet packet;

        public enum State
        {
            OFFLINE,
            CONNECTING,
            CONNECTED,
            HOSTING_ROOM,
            DISCONNECTED,
        }

        public State state;

        public void ConnectToBridgeServer()
        {
            connection = new Connection();

            connection.onPacketRecieved += HandlePacket;
            connection.onConnectionModeUpdated += ConnectionModeUpdated;

            connection.Connect("localhost", 25565);
        }

        public void HostGame()
        {
            connection.QueuePacket(new Packet(Packet.PacketType.HOST_GAME));
        }

        public void JoinGame()
        {
            
        }

        public void HandlePacket(Connection connection, Packet _packet)
        {
            Debug.Log(_packet.packetType);
            packet = _packet;
        }

        public void ConnectionModeUpdated(Connection connection, Connection.ConnectionMode connectionMode)
        {
            Debug.Log("Connection Mode Updated: " + connectionMode);

            if(connectionMode == Connection.ConnectionMode.CONNECTING)
            {
                state = State.CONNECTING;
            }
            else if (connectionMode == Connection.ConnectionMode.CONNECTED)
            {
                state = State.CONNECTED;
            }
            else if(connectionMode == Connection.ConnectionMode.DISCONNECTED)
            {
                state = State.DISCONNECTED;
            }
        }
    }
}
