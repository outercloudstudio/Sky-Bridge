using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyBridge
{
    public class NetworkedObject : MonoBehaviour
    {
        public string owner;
        public bool isOwner;

        public string ID;

        public bool isRegistered;

        public string prefabKey;

        private void Awake()
        {
            if (SkyBridge.justRegisteredRemoteNetworkedObject) return;

            isOwner = true;
            owner = SkyBridge.client.ID;

            Register();
        }

        public void Register()
        {
            isRegistered = true;

            ID = Guid.NewGuid().ToString();

            SkyBridge.registeredNetworkedObjects.Add(ID, this);

            SkyBridge.SendEveryone(new Packet("REGISTER_NETWORKED_OBJECT").AddValue(ID).AddValue(owner).AddValue(prefabKey).AddValue(transform.position).AddValue(transform.rotation));
        }

        public void RemoteRegister(string _ID, string _owner)
        {
            isRegistered = true;

            ID = _ID;
            owner = _owner;
        }

        private void OnDestroy()
        {
            if (!isOwner) return;

            SkyBridge.registeredNetworkedObjects.Remove(ID);

            SkyBridge.SendEveryone(new Packet("UNREGISTER_NETWORKED_OBJECT").AddValue(ID));
        }
    }
}
