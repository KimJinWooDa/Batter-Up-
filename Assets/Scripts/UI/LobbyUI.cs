// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEngine.UI;

// Fusion
using Fusion;


public class LobbyUI : MonoBehaviour
{
    // Cached references
    private NetworkRunnerController networkRunnerController = null;

    [Header("Room Elements")]
    [SerializeField] private Button RandomJoinButton = null;


    private void Start()
    {
        // Cache references
        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;

        // Assign button listeners
        RandomJoinButton.onClick.AddListener(JoinRandomRoom);

        // CreateRoomBtn.onClick.AddListener(() => CreateRoom(GameMode.Host, createRoomInputField.text));
        // JoinRoomButton.onClick.AddListener(() => CreateRoom(GameMode.Client, joinRoomByArgInputField.text));
    }

    private void CreateRoom(GameMode mode, string field)
    {
        if (field.Length >= 2)
        {
            Debug.Log($"------------ {mode} ------------");
            networkRunnerController.StartGame(mode, field);
        }
    }

    private void JoinRandomRoom()
    {
        Debug.Log($"------------ JoinRandomRoom ------------");
        networkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
    }
}