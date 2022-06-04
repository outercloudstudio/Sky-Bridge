using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SkyBridge
{
    [CustomEditor(typeof(LobbyManager))]
    public class LobbyManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LobbyManager lobbyManager = (LobbyManager)target;

            if (EditorApplication.isPlaying)
            {
                GUILayout.Label("State: " + lobbyManager.connection.connectionMode);

                if (lobbyManager.connection != null)
                {
                    if (lobbyManager.connection.connectionMode == Connection.ConnectionMode.OFFLINE)
                    {
                        if (GUILayout.Button("Connect"))
                        {
                            lobbyManager.ConnectToBridgeServer();
                        }
                    }else if (lobbyManager.connection.connectionMode == Connection.ConnectionMode.CONNECTED)
                    {
                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button("Host"))
                        {
                            lobbyManager.Host();
                        }

                        if (GUILayout.Button("Join"))
                        {
                            lobbyManager.Join();
                        }

                        GUILayout.EndHorizontal();
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
