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

        public enum PacketPersistance
        {
            UNPERSISTENT,
            PERSISTENT
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

        public static Dictionary<string, NetworkedObject> registeredNetworkedObjects = new Dictionary<string, NetworkedObject>();

        public static bool justRegisteredRemoteNetworkedObject;

        public static List<Packet> persistentPackets = new List<Packet>();

        private void Start()
        {
            connection.onPacketRecieved = HandlePacket;

            AddRemoteFunction("REGISTER_NETWORKED_OBJECT", RegisterNetworkedObject);
            AddRemoteFunction("UNREGISTER_NETWORKED_OBJECT", UnregisterNetworkedObject);
            AddRemoteFunction("NETWORKED_TRANSFORM_UPDATE", NetworkedTransformUpdate);
            AddRemoteFunction("NETWORKED_RIGIDBODY_UPDATE", NetworkedRigidbodyUpdate);
            AddRemoteFunction("ADD_CLIENT", AddClient);
        }

        #region Native Remote Function Handlers
        
        public void RegisterNetworkedObject(Connection connection, string source, Packet packet)
        {
            string ID = packet.GetString(0);
            string owner = packet.GetString(1);
            string prefabKey = packet.GetString(2);
            Vector3 position = packet.GetVector3(3);
            Quaternion rotation = packet.GetQuaternion(4);

            justRegisteredRemoteNetworkedObject = true;

            GameObject o = Instantiate(Resources.Load<GameObject>(prefabKey), position, rotation);

            NetworkedObject networkedObject = o.GetComponent<NetworkedObject>();
            networkedObject.RemoteRegister(ID, owner);

            registeredNetworkedObjects.Add(ID, networkedObject);

            justRegisteredRemoteNetworkedObject = false;
        }

        public void UnregisterNetworkedObject(Connection connection, string source, Packet packet)
        {
            string ID = packet.GetString(0);

            NetworkedObject networkedObject = registeredNetworkedObjects[ID];
            Destroy(networkedObject.gameObject);

            registeredNetworkedObjects.Remove(ID);
        }

        public void NetworkedTransformUpdate(Connection connection, string source, Packet packet)
        {
            string ID = packet.GetString(0);
            Vector3 pos = packet.GetVector3(1);
            Quaternion rot = packet.GetQuaternion(2);

            if (!registeredNetworkedObjects.ContainsKey(ID)) return;

            NetworkedObject networkObject = registeredNetworkedObjects[ID];

            NetworkedTransform networkTransform = networkObject.GetComponent<NetworkedTransform>();

            networkTransform.OnUpdate(pos, rot);
        }

        public void NetworkedRigidbodyUpdate(Connection connection, string source, Packet packet)
        {
            string ID = packet.GetString(0);
            Vector3 pos = packet.GetVector3(1);
            Quaternion rot = packet.GetQuaternion(2);
            Vector3 vel = packet.GetVector3(3);
            Vector3 angVel = packet.GetVector3(4);

            if (!registeredNetworkedObjects.ContainsKey(ID)) return;

            NetworkedObject networkObject = registeredNetworkedObjects[ID];

            NetworkedRigidbody networkRigidbody = networkObject.GetComponent<NetworkedRigidbody>();

            networkRigidbody.OnUpdate(pos, rot, vel, angVel);
        }

        public void AddClient(Connection connection, string source, Packet packet)
        {
            string ID = packet.GetString(0);

            clients.Add(new Client(ID));

            foreach (Packet _packet in persistentPackets)
            {
                Send(_packet, ID);
            }
        }
        #endregion

        private void Update()
        {
            connection.Update(Time.deltaTime);

            ThreadManager.Update();
        }

        public static void AddRemoteFunction(string ID, RemoteFunction.Handler handler)
        {
            remoteFunctions.Add(new RemoteFunction() { 
                ID = ID,
                onHandler = handler
            });
        }

        public static void Send(Packet packet, string target, Connection.PacketReliability reliability = Connection.PacketReliability.RELIABLE)
        {
            SendSmartPacket(packet, target, reliability);
        }

        public static void SendEveryone(Packet packet, PacketPersistance persistance = PacketPersistance.UNPERSISTENT, Connection.PacketReliability reliability = Connection.PacketReliability.RELIABLE)
        {
            if (persistance == PacketPersistance.PERSISTENT) persistentPackets.Add(packet);

            foreach (Client _client in clients)
            {
                if (_client.ID == client.ID) continue;

                SendSmartPacket(packet, _client.ID, reliability);
            }
        }

        public static void SendEveryoneExcept(Packet packet, string target, PacketPersistance persistance = PacketPersistance.UNPERSISTENT, Connection.PacketReliability reliability = Connection.PacketReliability.RELIABLE)
        {
            if (persistance == PacketPersistance.PERSISTENT) persistentPackets.Add(packet);

            foreach (Client _client in clients)
            {
                if (_client.ID == client.ID) continue;

                if (_client.ID == target) continue;

                SendSmartPacket(packet, _client.ID, reliability);
            }
        }

        public static void SendSmartPacket(Packet packet, string target, Connection.PacketReliability reliability = Connection.PacketReliability.RELIABLE)
        {
            packet.values.Insert(0, new Packet.SerializedValue(target));
            packet.values.Insert(0, new Packet.SerializedValue(packet.packetType));
            packet.packetType = "RELAY";

            connection.SendPacket(packet, reliability);
        }

        public void HandlePacket(Connection connection, Packet packet)
        {
            if (packet.packetType == "PLAYER_JOINED") {
                string ID = packet.GetString(0);

                clients.Add(new Client(ID));

                if (!isHost) return;

                foreach (Client client in clients)
                {
                    Send(new Packet("ADD_CLIENT").AddValue(client.ID), ID);
                }

                foreach (KeyValuePair<string, NetworkedObject> key in registeredNetworkedObjects)
                {
                    Send(new Packet("REGISTER_NETWORKED_OBJECT").AddValue(key.Key).AddValue(key.Value.owner).AddValue(key.Value.prefabKey).AddValue(key.Value.transform.position).AddValue(key.Value.transform.rotation), ID);
                }

                foreach (Packet _packet in persistentPackets)
                {
                    Send(_packet, ID);
                }
            }
            else if (packet.packetType == "PLAYER_LEFT")
            {
                string ID = packet.GetString(0);

                clients.RemoveAt(clients.FindIndex(_client => _client.ID == ID));

                if (!isHost) return;

                foreach (KeyValuePair<string, NetworkedObject> key in registeredNetworkedObjects)
                {
                    if (key.Value.owner != ID) return;

                    Packet unregisterPacket = new Packet("UNREGISTER_NETWORKED_OBJECT").AddValue(key.Value);

                    SendEveryone(unregisterPacket);

                    UnregisterNetworkedObject(null, client.ID, unregisterPacket);
                }
            }
            else if (packet.packetType == "RELAY")
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
