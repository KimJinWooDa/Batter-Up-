// Unity
using UnityEngine;

// Fusion
using Fusion;


public struct NetworkInputData : INetworkInput
{
    public Vector3 MovementsDirection;

    // Rig
    public Vector3 HeadPosition;
    public Quaternion HeadRotation;

    public Vector3 LeftHandPosition;
    public Quaternion LeftHandRotation;

    public Vector3 RightHandPosition;
    public Quaternion RightHandRotation;
}
