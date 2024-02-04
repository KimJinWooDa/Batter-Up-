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
using Sirenix.OdinInspector;


[DisallowMultipleComponent]
public class NetworkRunnerController : MonoBehaviour, INetworkRunnerCallbacks
{
    [Title("Prefabs")]
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private NetworkPrefabRef networkDataHandlerManagerPrefab = NetworkPrefabRef.Empty;

    [Title("Settings")]
    [SerializeField, Range(1, 10)] private int maxPlayerCount = 2;
    [SerializeField] private string gameSceneName = "MainGame";

    // Cached references
    public NetworkDataHandlerManager NetworkDataHandlerManager { get; set; } = null;
    private NetworkRunner networkRunnerInstance;

    // Constants
    private const string LOBBY_SCENE = "Lobby";

    // Events
    public event Action OnStartedRunnerConnection;
    public event Action OnPlayerJoinedSuccessfully;

    // Coroutines
    private Coroutine restructureRoutine = null;

    /// <summary>
    /// 자동으로 매치메이킹을 해 상대를 찾아주는 함수
    /// </summary>
    public void FindMatch()
    {
        StartGame(GameMode.AutoHostOrClient, null);
    }

    /// <summary>
    /// 방을 만들어주는 함수
    /// </summary>
    /// <param name="roomName">방 이름</param>
    public void CreateRoom(string roomName)
    {
        StartGame(GameMode.Host, $"[Room] {roomName}");
    }

    /// <summary>
    /// 방에 들어가는 함수
    /// </summary>
    /// <param name="roomName">방 이름</param>
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
            PlayerCount = maxPlayerCount,
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
        if (networkRunnerInstance.IsSceneAuthority) networkRunnerInstance.LoadScene(gameSceneName);

        yield break;

#pragma warning disable 0162

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

#pragma warning restore 0162
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
        if (runner.ActivePlayers.Count() == maxPlayerCount)
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