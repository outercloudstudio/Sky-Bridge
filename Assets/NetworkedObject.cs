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

            SkyBridge.SendEveryone(new Packet("REGISTER_NETWORKED_OBJECT").AddValue(ID).AddValue(name.Substring(0, name.Length - 7)).AddValue(transform.position).AddValue(transform.rotation));
        }

        public void RemoteRegister(string _ID)
        {
            isRegistered = true;

            ID = _ID;
        }
    }
}
