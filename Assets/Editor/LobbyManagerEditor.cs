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
        string roomID = "Room ID";
        string roomPassword = "12345";
        int maxPlayers = 8;

        public override void OnInspectorGUI()
        {
            LobbyManager lobbyManager = (LobbyManager)target;

            if (EditorApplication.isPlaying)
            {
                GUILayout.Label("State: " + lobbyManager.state.ToString());

                if (lobbyManager.state == LobbyManager.State.WAITING_FOR_ACTION)
                {
                    roomName = GUILayout.TextField(roomName);

                    roomID = GUILayout.TextField(roomID);

                    roomPassword = GUILayout.PasswordField(roomPassword, '*');

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(maxPlayers.ToString());

                    maxPlayers = Mathf.FloorToInt(GUILayout.HorizontalSlider(maxPlayers, 2, 16));

                    GUILayout.EndHorizontal();

                    GUILayout.Space(15);

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Host Game"))
                    {
                        lobbyManager.HostGame(roomName, roomPassword, maxPlayers);
                    }

                    if (GUILayout.Button("Join Game"))
                    {
                        lobbyManager.JoinGame(roomID, roomPassword);
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
