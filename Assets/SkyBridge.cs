using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace SkyBridge {
    public class SkyBridge : MonoBehaviour
    {
        public class Client
        {
            public string ID;

            public Client(string _ID)
            {
                ID = _ID;
            }
        }

        public class RemoteFunction
        {
            public string ID;

            public delegate void Handler(Connection connection, string source, Packet packet);
            public Handler onHandler;
        }

        public static int bufferSize = 4096;
        public static int sendRate = 60;
        public static float timeout = 30;
        public static float keepalive = 5;

        public static bool isHost = false;

        public static string roomID;

        public static Client client;
        public static Connection connection;
        public static List<Client> clients;

        public static int maxPlayers;

        public static List<RemoteFunction> remoteFunctions = new List<RemoteFunction>();

        private void Start()
        {
            connection.onPacketRecieved = HandlePacket;

            AddRemoteFunction("REGISTER_NETWORKED_OBJECT", RegisterNetworkedObject);
        }

        public void RegisterNetworkedObject(Connection connection, string source, Packet packet)
        {
            string ID = packet.GetString(0);
            string name = packet.GetString(1);
            Vector3 position = packet.GetVector3(2);
            Quaternion rotation = packet.GetQuaternion(3);

            //Debug.Log(position);
            //Debug.Log(rotation);
            //Debug.Log(name);

            GameObject o = Instantiate(Resources.Load<GameObject>(name), position, rotation);

            NetworkedObject networkedObject = o.GetComponent<NetworkedObject>();
            networkedObject.ID = ID;
            networkedObject.isRegistered = true;
        }

        private void Update()
        {
            connection.Update(Time.deltaTime);
        }

        public static void AddRemoteFunction(string ID, RemoteFunction.Handler handler)
        {
            remoteFunctions.Add(new RemoteFunction() { 
                ID = ID,
                onHandler = handler
            });
        }

        public static void Send(Packet packet, string target)
        {
            SendSmartPacket(packet, target);
        }

        public static void SendEveryone(Packet packet)
        {
            foreach (Client _client in clients)
            {
                if (_client.ID == client.ID) continue;

                SendSmartPacket(packet, _client.ID);
            }
        }

        public static void SendEveryoneExcept(Packet packet, string target)
        {
            foreach (Client _client in clients)
            {
                if (_client.ID == client.ID) continue;

                if (_client.ID == target) continue;

                SendSmartPacket(packet, _client.ID);
            }
        }

        public static void SendSmartPacket(Packet packet, string target)
        {
            packet.values.Insert(0, new Packet.SerializedValue(target));
            packet.values.Insert(0, new Packet.SerializedValue(packet.packetType));
            packet.packetType = "RELAY";

            connection.SendPacket(packet);
        }

        public void HandlePacket(Connection connection, Packet packet)
        {
            if (packet.packetType == "DEBUG")
            {
                Debug.Log(packet.GetString(0));
            } else if (packet.packetType == "PLAYER_JOINED") {
                string ID = packet.GetString(0);

                clients.Add(new Client(ID));
            } else if (packet.packetType == "RELAY")
            {
                string ID = packet.GetString(0);
                string source = packet.GetString(1);

                Packet newPacket = new Packet(ID);
                newPacket.values = packet.values.GetRange(2, packet.values.Count - 2);

                foreach (RemoteFunction remoteFunction in remoteFunctions.FindAll(remoteFunction => remoteFunction.ID == ID))
                {
                    remoteFunction.onHandler(connection, source, newPacket);
                }
            }
        }
    }
}
