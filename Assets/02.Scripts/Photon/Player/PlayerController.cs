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
    private Rigidbody rigid = null;

    // Constants
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";


    /// <summary>
    /// Called when the object is spawned.
    /// </summary>
    public override void Spawned()
    {
        // Cache
        rigid = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Happens before anything else Fusion does, network application, reconlation etc.
    /// Called at the start of the Fusion update loop, before the Fusion simulation loop.
    /// It fires before Fusion does Any work, every screen refresh.
    /// </summary>
    public void BeforeUpdate()
    {
        // We are the local machine if this state is true
        if (Runner.LocalPlayer.IsValid == Object.HasInputAuthority)
        {
            horizontal = Input.GetAxis(HORIZONTAL);
            vertical = Input.GetAxis(VERTICAL);
        }
    }

    public override void FixedUpdateNetwork()
    {
        /*
         * will return false if : 
         * the client does not have state authority or input authority
         * the requested type of input does not exist in the simulation
         */
        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            rigid.velocity = new Vector3(input.HorizontalInput * moveSpeed, 0f, input.VerticalInput * moveSpeed);
        }
    }

    /// <summary>
    /// Get the player input data.
    /// </summary>
    /// <returns>Returns the player input data.</returns>
    public PlayerData GetPlayerNetworkInput()
    {
        // Create the data we want to send
        PlayerData data = new PlayerData()
        {
            HorizontalInput = horizontal,
            VerticalInput = vertical
        };

        // Return the data
        return data;
    }
}
