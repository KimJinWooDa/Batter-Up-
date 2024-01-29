// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Fusion
using Fusion;
using WebSocketSharp;


[DisallowMultipleComponent]
public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private GameObject localObject = null;

    [Header("Prefab")]
    [SerializeField] private GameObject vrRigPrefab = null;

    [Header("Model")]
    [SerializeField] private Transform modelHead = null;
    [SerializeField] private Transform modelLeftHand = null;
    [SerializeField] private Transform modelRightHand = null;

    // Physics
    private float horizontal = 0f;
    private float vertical = 0f;

    // Cached variables
    private NetworkCharacterController networkCharacterController = null;
    private PlayerRigMapper playerRigMapper = null;

    // Constants
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";


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
        // Cache
        networkCharacterController = GetComponent<NetworkCharacterController>();

        // Set the local object
        if (HasInputAuthority) localObject.SetActive(true);
        else localObject.SetActive(false);

        // Create the rig
        var rig = SpawnLocalObject(vrRigPrefab);

        // Set local position to zero
        rig.transform.localPosition = Vector3.zero;

        // Cache the rig mapper
        playerRigMapper = rig.GetComponent<PlayerRigMapper>();
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
            // Get normal input
            horizontal = Input.GetAxisRaw(HORIZONTAL);
            vertical = Input.GetAxisRaw(VERTICAL);

            // Get the rig input
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
        if (Runner.TryGetInputForPlayer<NetworkInputData>(Object.InputAuthority, out var input))
        {
            // Update the character controller
            networkCharacterController.Move(input.MovementsDirection * moveSpeed * Runner.DeltaTime);

            // Update the model
            if (modelHead == null || modelLeftHand == null || modelRightHand == null) return;

            //modelHead.position = rigData.HeadPosition;
            modelHead.rotation = input.HeadRotation;

            modelLeftHand.position = input.LeftHandPosition;
            modelLeftHand.rotation = input.LeftHandRotation;

            modelRightHand.position = input.RightHandPosition;
            modelRightHand.rotation = input.RightHandRotation;
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

    /// <summary>
    /// Spawn a local object.
    /// </summary>
    /// <param name="prefab"></param>
    public GameObject SpawnLocalObject(GameObject prefab)
    {
        if(HasInputAuthority)
        {
            // Spawn the prefab
            if (prefab != null) return Instantiate(prefab, Vector3.zero, Quaternion.identity, localObject.transform);
        }

        return null;
    }
}
