// System
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Fusion
using Fusion;


[DisallowMultipleComponent]
public class NetworkDataHandlerManager : NetworkBehaviour
{
    [Networked, Capacity(8), UnitySerializeField]
    private NetworkDictionary<string, double> performanceScoreDatabase => default;

    public override void Spawned()
    {
        base.Spawned();

        // Register this instance
        GlobalManagers.Instance.NetworkRunnerController.NetworkDataHandlerManager = this;
    }

    public void RefreshPerformanceScore()
    {
        var activePlayers = Runner.ActivePlayers;
        var sum = 0d;

        foreach (var player in activePlayers)
        {
            if (player.IsRealPlayer)
            {
                sum += Runner.GetPlayerRtt(player);
            }
        }

        var performanceScore = sum / activePlayers.Count();
        var playerId = Runner.LocalPlayer.PlayerId.ToString();

        WritePerformanceScoreRpc(playerId, performanceScore);
    }

    [Rpc]
    private void WritePerformanceScoreRpc(string playerId, double performanceScore)
    {
        if (performanceScoreDatabase.ContainsKey(playerId))
        {
            performanceScoreDatabase.Set(playerId, performanceScore);
        }
        else
        {
            performanceScoreDatabase.Add(playerId, performanceScore);
        }
    }
}
