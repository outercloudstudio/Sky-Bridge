using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SkyBridge
{
    [CustomEditor(typeof(LobbyManager))]
    public class LobbyManagerEditor : Editor
    {
        string roomID = "Room ID";
        int maxPlayers = 1;

        public override void OnInspectorGUI()
        {
            LobbyManager lobbyManager = (LobbyManager)target;

            if (EditorApplication.isPlaying)
            {
                GUILayout.Label("State: " + lobbyManager.connection.connectionMode);

                if (lobbyManager.connection != null)
                {
                    if (lobbyManager.connection.connectionMode == Connection.ConnectionMode.OFFLINE || lobbyManager.connection.connectionMode == Connection.ConnectionMode.DISCONNECTED)
                    {
                        if (GUILayout.Button("Connect"))
                        {
                            lobbyManager.ConnectToBridgeServer();
                        }
                    }else if (lobbyManager.connection.connectionMode == Connection.ConnectionMode.CONNECTED)
                    {
                        roomID = GUILayout.TextField(roomID);

                        GUILayout.BeginHorizontal();

                        GUILayout.Label("Max players: ");

                        string unparsedMaxPlayers = GUILayout.TextField(maxPlayers.ToString());

                        try
                        {
                            maxPlayers = int.Parse(unparsedMaxPlayers);
                        }
                        catch
                        {

                        }

                        maxPlayers = Mathf.Clamp(maxPlayers, 1, 8);

                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button("Host"))
                        {
                            lobbyManager.Host(maxPlayers);
                        }

                        if (GUILayout.Button("Join"))
                        {
                            lobbyManager.Join(roomID);
                        }

                        GUILayout.EndHorizontal();

                        if (GUILayout.Button("Disconnect"))
                        {
                            lobbyManager.connection.Disconnect();
                        }
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
