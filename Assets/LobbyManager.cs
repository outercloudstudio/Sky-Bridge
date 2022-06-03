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

        public enum State
        {
            OFFLINE,
            CONNECTING,
            CONNECTED,
            WAITING_FOR_ACTION,
            HOSTING_ROOM,
            JOINING_ROOM,
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

        public void HostGame(string roomName, string roomPassword, int maxPlayers)
        {
            state = State.HOSTING_ROOM;
            connection.QueuePacket(new Packet(Packet.PacketType.HOST_GAME).AddValue(roomName).AddValue(Guid.NewGuid().ToString()).AddValue(roomPassword));

            SkyBridge.hostBridgeServerConnection = connection;
            SkyBridge.maxPlayers = maxPlayers;

            SceneManager.LoadScene("Game");
        }

        public void JoinGame(string roomID, string roomPassword)
        {
            state = State.JOINING_ROOM;
            connection.QueuePacket(new Packet(Packet.PacketType.JOIN_GAME).AddValue(roomID).AddValue(roomPassword));
        }

        public void HandlePacket(Connection connection, Packet packet)
        {
            if (packet.packetType == Packet.PacketType.READY)
            {
                state = State.WAITING_FOR_ACTION;
            }else if (packet.packetType == Packet.PacketType.ERROR)
            {
                connection.Disconnect();
            }
        }

        public void ConnectionModeUpdated(Connection connection, Connection.ConnectionMode connectionMode)
        {
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
