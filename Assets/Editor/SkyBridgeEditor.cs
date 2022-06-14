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
                GUILayout.TextField(SkyBridge.roomID);
                GUILayout.Label("Is Host: " + SkyBridge.isHost);
                GUILayout.Label("My ID: " + SkyBridge.client.ID);

                GUILayout.Space(10);

                GUILayout.Label("Clients: ");

                foreach (SkyBridge.Client client in SkyBridge.clients)
                {
                    GUILayout.Label(client.ID);
                }
            }
            else
            {
                GUILayout.Label("Enter Play Mode To Interact!");
            }
        }
    }
}
