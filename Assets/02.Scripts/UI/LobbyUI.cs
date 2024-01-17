// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEngine.UI;


[DisallowMultipleComponent]
public class LobbyUI : MonoBehaviour
{
    // Cached references
    private NetworkRunnerController networkRunnerController = null;


    private void OnGUI()
    {
        int buttonWidth = 200;
        int buttonHeight = 50;

        int posX = Screen.width - buttonWidth - 10;
        int posY = 10;

        if (GUI.Button(new Rect(posX, posY, buttonWidth, buttonHeight), "Join Random Room"))
        {
            FindMatch();
        }
    }

    private void Start()
    {
        // Cache references
        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;
    }

    public void CreateRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            networkRunnerController.CreateRoom(roomName);
        }
    }

    public void JoinRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            networkRunnerController.JoinRoom(roomName);
        }
    }

    private void FindMatch()
    {
        networkRunnerController.FindMatch();
    }
}