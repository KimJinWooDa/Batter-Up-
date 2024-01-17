// System
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
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
    [SerializeField] private NetworkPrefabRef networkDataHandlerManagerPrefab = NetworkPrefabRef.Empty;

    // Cached references
    public NetworkDataHandlerManager NetworkDataHandlerManager = null;
    private NetworkRunner networkRunnerInstance;

    // Constants
    private const string GAME_SCENE_NAME = "MainGame";
    private const string LOBBY_SCENE = "Lobby";
    private const int MAX_PLAYER_COUNT = 2;

    // Events
    public event Action OnStartedRunnerConnection;
    public event Action OnPlayerJoinedSuccessfully;

    // Coroutines
    private Coroutine restructureRoutine = null;

    /// <summary>
    /// �ڵ����� ��ġ����ŷ�� �� ��븦 ã���ִ� �Լ�
    /// </summary>
    public void FindMatch()
    {
        StartGame(GameMode.AutoHostOrClient, null);
    }

    /// <summary>
    /// ���� ������ִ� �Լ�
    /// </summary>
    /// <param name="roomName">�� �̸�</param>
    public void CreateRoom(string roomName)
    {
        StartGame(GameMode.Host, $"[Room] {roomName}");
    }

    /// <summary>
    /// �濡 ���� �Լ�
    /// </summary>
    /// <param name="roomName">�� �̸�</param>
    public void JoinRoom(string roomName)
    {
        StartGame(GameMode.Client, $"[Room] {roomName}");
    }

    /// <summary>
    /// Starts the game using the provided mode and room name.
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="roomName"></param>
    private async void StartGame(GameMode mode, string roomName)
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
            PlayerCount = MAX_PLAYER_COUNT,
            SceneManager = networkRunnerInstance.GetComponent<INetworkSceneManager>()
        };

        // Get the result
        var result = await networkRunnerInstance.StartGame(startGameArgs);

        // Analyze the result
        if (result.Ok)
        {
            print("Start simple match making [filter : number of players == 2]");
        }
        else
        {
        }
    }

    /// <summary>
    /// Shuts down the runner.
    /// </summary>
    public void ShutDownRunner()
    {
        networkRunnerInstance.Shutdown();
    }

    private IEnumerator RestructureRoutine()
    {
        // Skip restructure routine for just now
        if (networkRunnerInstance.IsSceneAuthority) networkRunnerInstance.LoadScene(GAME_SCENE_NAME);

        yield break;

        print("Restructure Routine started");

        // Wait until network data handler is spawned
        while (NetworkDataHandlerManager == null) yield return null;

        // Refresh the performance score
        NetworkDataHandlerManager.RefreshPerformanceScore();

        if(networkRunnerInstance.IsServer)
        {
            // Only wait for 1 second
            yield return new WaitForSeconds(1f);
        }

        yield break;
    }

    #region Network Runner Callbacks

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Invoke event
        OnPlayerJoinedSuccessfully?.Invoke();

        // Check if we are the server
        if (runner.IsServer)
        {
            if (NetworkDataHandlerManager == null)
            {
                // Frist find the network data handler
                var findResult = FindObjectOfType<NetworkDataHandlerManager>();

                if(findResult == null)
                {
                    // Spawn the data handler manager
                    var spawnedObject = networkRunnerInstance.Spawn(networkDataHandlerManagerPrefab, Vector3.zero, Quaternion.identity).gameObject;

                    // Make it dont destroy on load
                    runner.SceneManager.MakeDontDestroyOnLoad(spawnedObject);
                }
                else
                {
                    // Set the reference
                    NetworkDataHandlerManager = findResult;
                }
            }
        }

        // Check if room is full
        if (runner.ActivePlayers.Count() == MAX_PLAYER_COUNT)
        {
            // Start restructure routine
            if (restructureRoutine != null)
            {
                return;
            }
            restructureRoutine = StartCoroutine(RestructureRoutine());
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene(LOBBY_SCENE);
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    #endregion

    #region Unused Network Runner Callbacks

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
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