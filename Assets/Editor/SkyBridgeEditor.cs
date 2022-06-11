using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SkyBridge
{
    [CustomEditor(typeof(SkyBridge))]
    public class SkyBridgeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SkyBridge skyBridge = (SkyBridge)target;

            if (EditorApplication.isPlaying)
            {
                GUILayout.Label("State: " + SkyBridge.connection.connectionMode);
                GUILayout.Label("Room ID: " + SkyBridge.currentRoom.ID);
                GUILayout.Label("Is Host: " + SkyBridge.isHost);
                GUILayout.Label("My ID: " + SkyBridge.client.ID);

                if (GUILayout.Button("Send"))
                {
                    SkyBridge.connection.SendPacket(new Packet("DEBUG"));
                }
            }
            else
            {
                GUILayout.Label("Enter Play Mode To Interact!");
            }
        }
    }
}
