// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Fusion
using Fusion;
using Fusion.Sockets;


public class LocalInputPoller : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private PlayerController player = null;

    public override void Spawned()
    {
        if (Runner.LocalPlayer == Object.InputAuthority)
        {
            Runner.AddCallbacks(this);
        }
    }

    #region Used Network Events

    /// <summary>
    /// Only if local we get input callbacks, no need to check if we are local.
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="input"></param>
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (runner != null && runner.IsRunning)
        {
            var data = player.GetPlayerNetworkInput();
            input.Set(data);
        }
    }

    #endregion

    #region Unused Network Events

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    #endregion
}
