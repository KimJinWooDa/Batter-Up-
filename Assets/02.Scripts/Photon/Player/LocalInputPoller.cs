﻿// System
using System;
using System.Collections;
using System.Collections.Generic;
using AutoSet.Utils;

// Unity
using UnityEngine;

// Fusion
using Fusion;
using Fusion.Sockets;


public class LocalInputPoller : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField, AutoSet] private PlayerController playerController = null;
    [SerializeField, AutoSet] private PlayerRig playerRig = null;

    /// <summary>
    /// Called when the object is spawned.
    /// </summary>
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
            var networkInputData = playerController.GetPlayerNetworkInput();
            input.Set(networkInputData);

            /*
            var networkRigData = playerRig.GetPlayerNetworkRig();
            input.Set(networkRigData);*/
        }
    }

    #endregion

    #region Unused Network Events

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
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

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
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

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    #endregion
}