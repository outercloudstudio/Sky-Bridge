using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SkyBridge
{
    [CustomEditor(typeof(LobbyManager))]
    public class LobbyManagerEditor : Editor
    {
        string roomName = "Room Name";

        public override void OnInspectorGUI()
        {
            LobbyManager lobbyManager = (LobbyManager)target;

            if (EditorApplication.isPlaying)
            {
                GUILayout.Label("State: " + lobbyManager.state.ToString());

                if (lobbyManager.state == LobbyManager.State.WAITING_FOR_ACTION)
                {
                    GUILayout.BeginHorizontal();

                    roomName = GUILayout.TextField(roomName);

                    if (GUILayout.Button("Host Game"))
                    {
                        lobbyManager.HostGame(roomName);
                    }

                    if (GUILayout.Button("Join Game"))
                    {
                        lobbyManager.JoinGame();
                    }
                    GUILayout.EndHorizontal();
                }
                else if(lobbyManager.state == LobbyManager.State.OFFLINE || lobbyManager.state == LobbyManager.State.DISCONNECTED)
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
