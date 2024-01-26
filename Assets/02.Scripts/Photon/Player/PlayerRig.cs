// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Fusion
using Fusion;


public class PlayerRig : NetworkBehaviour
{
    [Header("VR Rig")]
    [SerializeField] private Transform rigHead = null;
    [SerializeField] private Transform rigLeftHand = null;
    [SerializeField] private Transform rigRightHand = null;

    [Header("Model")]
    [SerializeField] private Transform modelHead = null;
    [SerializeField] private Transform modelLeftHand = null;
    [SerializeField] private Transform modelRightHand = null;


    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        // Update the VR rig
        if (HasInputAuthority)
        {
            if(rigHead == null || rigLeftHand == null || rigRightHand == null) return;
            if(modelHead == null || modelLeftHand == null || modelRightHand == null) return;

            modelHead.position = rigHead.position;
            modelHead.rotation = rigHead.rotation;

            modelLeftHand.position = rigLeftHand.position;
            modelLeftHand.rotation = rigLeftHand.rotation;

            modelRightHand.position = rigRightHand.position;
            modelRightHand.rotation = rigRightHand.rotation;
        }
    }
}
