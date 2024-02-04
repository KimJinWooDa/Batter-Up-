// System
using System.Collections;
using System.Collections.Generic;
using AutoSet.Utils;

// Unity
using UnityEngine;

// Fusion
using Fusion;

// LiNEARJUN
using NetworkCharacterController = LiNEARJUN.Network.NetworkCharacterController;

[DisallowMultipleComponent]
public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private GameObject localObject = null;
    public bool IsTest;
    [SerializeField] private float rotationSpeed = 3f;
    
    [Header("Prefab")]
    [SerializeField] private GameObject vrRigPrefab = null;

    [Header("Model")]
    [SerializeField] private Transform modelHead = null;
    [SerializeField] private Transform modelLeftHand = null;
    [SerializeField] private Transform modelRightHand = null;

    // Physics
    private float horizontal = 0f;
    private float vertical = 0f;
    private float rawHorizontalInput;
    
    // Cached variables
    [SerializeField] private NetworkCharacterController networkCharacterController = null;
    private PlayerRigMapper playerRigMapper = null;

    // Constants
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    /// <summary>
    /// Called when the object is spawned.
    /// </summary>
    public override void Spawned()
    {
        // Set the local object
        if (HasInputAuthority)
        {
            localObject.SetActive(true);

            // Create the rig
            var rig = SpawnLocalObject(vrRigPrefab);

            // Set local position to zero
            rig.transform.localPosition = Vector3.zero;

            // Cache the rig mapper
            playerRigMapper = rig.GetComponent<PlayerRigMapper>();
        }
        else localObject.SetActive(false);
    }

    /// <summary>
    /// Happens before anything else Fusion does, network application, reconlation etc.
    /// Called at the start of the Fusion update loop, before the Fusion simulation loop.
    /// It fires before Fusion does Any work, every screen refresh.
    /// </summary>
    public void BeforeUpdate()
    {
        // Get normal input
        if (Runner.LocalPlayer.IsRealPlayer != Object.HasInputAuthority) return;
        
        if (IsTest)
        {
            horizontal = Input.GetAxisRaw(HORIZONTAL);
            vertical = Input.GetAxisRaw(VERTICAL);

            if (Input.GetKey(KeyCode.K))
            {
                rawHorizontalInput = -1f;
            }
            else if (Input.GetKey(KeyCode.L))
            {
                rawHorizontalInput = 1f;
            }
            else
            {
                rawHorizontalInput = 0f;
            }
        }
        else
        {
            horizontal = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).x;
            vertical = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y;

            rawHorizontalInput = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).x;
        }


    }

    float SnapInput(float input, float threshold = 0.5f)
    {
        if (Mathf.Abs(input) > threshold)
        {
            return Mathf.Sign(input);
        }
        return 0f;
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
            //var worldDirection = playerRigMapper.CenterEye.TransformDirection(input.MovementsDirection);
            //worldDirection.y = 0f;
            float snappedHorizontalInput = SnapInput(rawHorizontalInput);
            var yLerp = Mathf.Lerp(transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.y + snappedHorizontalInput * 45f, Runner.DeltaTime * rotationSpeed);
            var rotation = Quaternion.Euler(0, yLerp, 0);

            networkCharacterController.Move(input.MovementsDirection * moveSpeed, rotation);

            //transform.Rotate(0, snappedHorizontalInput * 45f * Time.deltaTime * rotationSpeed, 0);

            // Update the model
            if (modelHead == null || modelLeftHand == null || modelRightHand == null) return;

            modelHead.rotation = input.HeadRotation * Quaternion.Euler(-90f, 0f, 0f);

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
        //data.HeadPosition = headPosition;
        data.HeadRotation = playerRigMapper.RigHeadRotation;
        data.LeftHandPosition = playerRigMapper.RigLeftHandPosition;
        data.LeftHandRotation = playerRigMapper.RigLeftHandRotation;
        data.RightHandPosition = playerRigMapper.RigRightHandPosition;
        data.RightHandRotation =  playerRigMapper.RigRightHandRotation;

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
