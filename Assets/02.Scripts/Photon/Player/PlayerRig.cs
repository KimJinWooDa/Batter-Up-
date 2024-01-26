// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Fusion
using Fusion;


[DisallowMultipleComponent]
public class PlayerRig : NetworkBehaviour, IBeforeUpdate
{
    [Header("Prefab")]
    [SerializeField] private GameObject vrRigPrefab = null;

    [Header("Model")]
    [SerializeField] private Transform modelHead = null;
    [SerializeField] private Transform modelLeftHand = null;
    [SerializeField] private Transform modelRightHand = null;

    // Cached variables
    private PlayerController playerController = null;
    private PlayerRigMapper playerRigMapper = null;

    // Rig Data
    private Vector3 headPosition = Vector3.zero;
    private Quaternion headRotation = Quaternion.identity;
    private Vector3 leftHandPosition = Vector3.zero;
    private Quaternion leftHandRotation = Quaternion.identity;
    private Vector3 rightHandPosition = Vector3.zero;
    private Quaternion rightHandRotation = Quaternion.identity;


    /// <summary>
    /// Called when the object is spawned.
    /// </summary>
    public override void Spawned()
    {
        // Create the rig
        if (Runner.LocalPlayer.IsRealPlayer == Object.HasInputAuthority)
        {
            // Cache the player controller
            playerController = GetComponent<PlayerController>();

            // Create the rig
            var rig = playerController.SpawnLocalObject(vrRigPrefab);

            // Cache the rig mapper
            playerRigMapper = rig.GetComponent<PlayerRigMapper>();
        }
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
            // Get the rig data
            headPosition = playerRigMapper.RigHeadPosition;
            headRotation = playerRigMapper.RigHeadRotation;

            leftHandPosition = playerRigMapper.RigLeftHandPosition;
            leftHandRotation = playerRigMapper.RigLeftHandRotation;

            rightHandPosition = playerRigMapper.RigRightHandPosition;
            rightHandRotation = playerRigMapper.RigRightHandRotation;
        }
    }

    public override void FixedUpdateNetwork()
    {
        /*
         * will return false if : 
         * the client does not have state authority or input authority
         * the requested type of input does not exist in the simulation
         */
        if (Runner.TryGetInputForPlayer<NetworkRigData>(Object.InputAuthority, out var rigData))
        {
            // Update the model
            if (modelHead == null || modelLeftHand == null || modelRightHand == null) return;

            modelHead.position = rigData.HeadPosition;
            modelHead.rotation = rigData.HeadRotation;

            modelLeftHand.position = rigData.LeftHandPosition;
            modelLeftHand.rotation = rigData.LeftHandRotation;

            modelRightHand.position = rigData.RightHandPosition;
            modelRightHand.rotation = rigData.RightHandRotation;
        }
    }

    /// <summary>
    /// Get the player rig data.
    /// </summary>
    /// <returns>Returns the player rig data.</returns>
    public NetworkRigData GetPlayerNetworkRig()
    {
        // Create the data we want to send
        var data = new NetworkRigData();

        // Set rig data
        data.HeadPosition = headPosition;
        data.HeadRotation = headRotation;
        data.LeftHandPosition = leftHandPosition;
        data.LeftHandRotation = leftHandRotation;
        data.RightHandPosition = rightHandPosition;
        data.RightHandRotation = rightHandRotation;

        // Return the data
        return data;
    }
}
