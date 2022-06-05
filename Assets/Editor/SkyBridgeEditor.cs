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
                if(SkyBridge.bridgeServerConncection != null) GUILayout.Label("State: " + SkyBridge.bridgeServerConncection.connectionMode);
                GUILayout.Label("Room ID: " + SkyBridge.currentRoom.ID);
                GUILayout.Label("Is Host: " + SkyBridge.isHost);

                if (GUILayout.Button("Send"))
                {
                    foreach (Connection connection in SkyBridge.connections)
                    {
                        if (connection != null) connection.SendPacket(new Packet("DEBUG"));
                    }
                }
            }
            else
            {
                GUILayout.Label("Enter Play Mode To Interact!");
            }
        }
    }
}
