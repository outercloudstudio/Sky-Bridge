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
                GUILayout.Label("State: " + lobbyManager.state.ToString());

                if (lobbyManager.state == LobbyManager.State.CONNECTED)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Host Game"))
                    {
                        lobbyManager.HostGame();
                    }

                    if (GUILayout.Button("Join Game"))
                    {
                        lobbyManager.JoinGame();
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    if (GUILayout.Button("Connect"))
                    {
                        lobbyManager.ConnectToBridgeServer();
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
