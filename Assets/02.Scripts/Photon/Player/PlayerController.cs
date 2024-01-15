// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Fusion
using Fusion;


[DisallowMultipleComponent]
public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 6f;

    // Physics
    private float horizontal = 0f;
    private float vertical = 0f;

    // Cached variables
    private NetworkCharacterController networkCharacterController = null;

    // Constants
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";


    /// <summary>
    /// Called when the object is spawned.
    /// </summary>
    public override void Spawned()
    {
        // Cache
        networkCharacterController = GetComponent<NetworkCharacterController>();
    }

    /// <summary>
    /// Happens before anything else Fusion does, network application, reconlation etc.
    /// Called at the start of the Fusion update loop, before the Fusion simulation loop.
    /// It fires before Fusion does Any work, every screen refresh.
    /// </summary>
    public void BeforeUpdate()
    {
        // We are the local machine if this state is true
        if (Runner.LocalPlayer.IsRealPlayer == Object.HasInputAuthority)
        {
            horizontal = Input.GetAxisRaw(HORIZONTAL);
            vertical = Input.GetAxisRaw(VERTICAL);
        }
    }

    public override void FixedUpdateNetwork()
    {
        /*
         * will return false if : 
         * the client does not have state authority or input authority
         * the requested type of input does not exist in the simulation
         */
        if (Runner.TryGetInputForPlayer<NetworkInputData>(Object.InputAuthority, out var input))
        {
            networkCharacterController.Move(input.MovementsDirection * moveSpeed * Runner.DeltaTime);
        }
    }

    /// <summary>
    /// Get the player input data.
    /// </summary>
    /// <returns>Returns the player input data.</returns>
    public NetworkInputData GetPlayerNetworkInput()
    {
        // Create the data we want to send
        NetworkInputData data = new NetworkInputData();

        // Set input data
        data.MovementsDirection.x = horizontal;
        data.MovementsDirection.z = vertical;

        // Return the data
        return data;
    }
}
