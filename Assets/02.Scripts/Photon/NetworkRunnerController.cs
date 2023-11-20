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

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    // Cached references
    private NetworkRunner networkRunnerInstance;

    // Events
    //public event Action OnStartedRunnerConnection;
    //public event Action OnPlayerJoinedSuccessfully;

    private NetworkRunner _runner;

    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0,0,200,40), "Host"))
            {
                StartGame(GameMode.Host, String.Empty);
            }
            if (GUI.Button(new Rect(0,40,200,40), "Join"))
            {
                StartGame(GameMode.Client, String.Empty);
            }
        }
    }
    
    public async void StartGame(GameMode mode, string roomName)
    {
        //OnStartedRunnerConnection?.Invoke();
        
         if (networkRunnerInstance == null)
         {
             networkRunnerInstance = Instantiate(networkRunnerPrefab);
         }
        
         // Register so we will get the callbacks as well
         networkRunnerInstance.AddCallbacks(this);

        // ProvideInput means that that player is recording and sending inputs to the server.
        networkRunnerInstance.ProvideInput = true;

         var startGameArgs = new StartGameArgs()
         {
             GameMode = mode,
             SessionName = roomName,
             PlayerCount = 2,
             SceneManager = networkRunnerInstance.GetComponent<INetworkSceneManager>()
             //SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
         };
        
         var result = await networkRunnerInstance.StartGame(startGameArgs);
         if (result.Ok)
         {
             const string SCENE_NAME = "MainGame";
             networkRunnerInstance.SetActiveScene(SCENE_NAME);
         }
         else
         {
             Debug.LogError($"Failed to start: {result.ShutdownReason}");
         }
       
    }

    public void ShutDownRunner()
    {
        networkRunnerInstance.Shutdown();
    }

    #region Network Runner Callbacks

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner == null)
        {
            Debug.LogError("OnPlayerJoined: runner is null");
            return;
        }
        
        //OnPlayerJoinedSuccessfully?.Invoke();

        Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
        
        NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        
        _spawnedCharacters.Add(player, networkPlayerObject);
        DontDestroyOnLoad(networkPlayerObject);
        Debug.Log("OnPlayerJoined: Player spawned successfully");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
        
        Debug.Log("OnPlayerLeft");
    }

    private bool _mouseButton0;
    private void Update()
    {
        _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        if (_mouseButton0)
            data.buttons |= NetworkInputData.MOUSEBUTTON1;
        _mouseButton0 = false;

        input.Set(data);
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

    #endregion
}
