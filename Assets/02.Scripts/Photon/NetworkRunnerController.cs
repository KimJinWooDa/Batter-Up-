// System
using System;
using System.Collections.Generic;

// Unity 
using UnityEngine;
using UnityEngine.SceneManagement;

// Fusion
using Fusion;
using Fusion.Sockets;


[DisallowMultipleComponent]
public class NetworkRunnerController : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;

    // Cached references
    private NetworkRunner networkRunnerInstance;

    // Constants
    private const string GAME_SCENE_NAME = "MainGame";

    // Events
    public event Action OnStartedRunnerConnection;
    public event Action OnPlayerJoinedSuccessfully;


    /// <summary>
    /// Starts the game using the provided mode and room name.
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="roomName"></param>
    public async void StartGame(GameMode mode, string roomName)
    {
        // Invoke event
        OnStartedRunnerConnection?.Invoke();

        // Check if we have an network runner instance, if not create one
        if (networkRunnerInstance == null) networkRunnerInstance = Instantiate(networkRunnerPrefab);

        // Register so we will get the callbacks as well
        networkRunnerInstance.AddCallbacks(this);

        // ProvideInput means that that player is recording and sending inputs to the server.
        networkRunnerInstance.ProvideInput = true;

        // Start the game with the provided arguments
        var startGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName,
            PlayerCount = 2,
            SceneManager = networkRunnerInstance.GetComponent<INetworkSceneManager>()
        };

        // Get the result
        var result = await networkRunnerInstance.StartGame(startGameArgs);

        // Analyze the result
        if (result.Ok)
        {
            await networkRunnerInstance.LoadScene(GAME_SCENE_NAME);
        }
        else
        {
            Debug.LogError($"Failed to start: {result.ShutdownReason}");
        }
    }

    /// <summary>
    /// Shuts down the runner.
    /// </summary>
    public void ShutDownRunner()
    {
        networkRunnerInstance.Shutdown();
    }

    #region Network Runner Callbacks

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Invoke event
        OnPlayerJoinedSuccessfully?.Invoke();

        Debug.Log("OnPlayerJoined");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerLeft");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");

        const string LOBBY_SCENE = "Lobby";
        SceneManager.LoadScene(LOBBY_SCENE);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("OnConnectRequest");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFailed");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("OnUserSimulationMessage");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("OnSessionListUpdated");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("OnCustomAuthenticationResponse");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("OnHostMigration");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("OnReliableDataReceived");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadDone");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadStart");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    #endregion
}