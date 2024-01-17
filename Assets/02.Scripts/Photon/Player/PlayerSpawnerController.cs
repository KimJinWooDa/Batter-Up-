// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Fusion
using Fusion;


[DisallowMultipleComponent]
public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [Header("References")]
    [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;

    [Header("Settings")]
    [SerializeField] private List<Transform> spawnPointList = new List<Transform>();

    // Database
    private Dictionary<PlayerRef, NetworkObject> spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();


    public override void Spawned()
    {
        // Check if we are the server
        if (Runner.IsServer)
        {
            // Spawn all the players that are already connected to the server
            foreach (var player in Runner.ActivePlayers)
            {
                SpawnPlayer(player);
            }
        }
    }

    /// <summary>
    /// Spawns the player on the server.
    /// </summary>
    /// <param name="playerRef"></param>
    private void SpawnPlayer(PlayerRef playerRef)
    {
        // Check if we are the server
        if (Runner.IsServer)
        {
            // Calculate the spawn point index
            var index = playerRef.PlayerId % spawnPointList.Count;

            // Get the spawn point using the index
            var spawnPoint = spawnPointList[index].position;

            // Spawn the player
            var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);

            // Set the player object on the runner (so we can know which player is which object)
            Runner.SetPlayerObject(playerRef, playerObject);

            // Add the player to the database
            spawnedPlayers.Add(playerRef, playerObject);
        }
    }

    /// <summary>
    /// Despawns the player on the server.
    /// </summary>
    /// <param name="playerRef"></param>
    private void DespawnPlayer(PlayerRef playerRef)
    {
        // Check if we are the server
        if (Runner.IsServer)
        {
            if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
            {
                // Despawn the player
                Runner.Despawn(playerNetworkObject);
            }

            // Reset player object
            Runner.SetPlayerObject(playerRef, null);

            // Remove the player from the database
            spawnedPlayers.Remove(playerRef);
        }
    }

    /// <summary>
    /// Called when a player has joined the game.
    /// </summary>
    /// <param name="player"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }
}
