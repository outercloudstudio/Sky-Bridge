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
                GUILayout.Label("State: " + SkyBridge.bridgeServerConncection.connectionMode);
                GUILayout.Label("Room ID: " + SkyBridge.currentRoom.ID);
                GUILayout.Label("Is Host: " + SkyBridge.isHost);
            }
            else
            {
                GUILayout.Label("Enter Play Mode To Interact!");
            }
        }
    }
}
