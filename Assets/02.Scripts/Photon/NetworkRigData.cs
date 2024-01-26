// Unity
using UnityEngine;

// Fusion
using Fusion;


public struct NetworkRigData : INetworkInput
{
    public Vector3 HeadPosition;
    public Quaternion HeadRotation;

    public Vector3 LeftHandPosition;
    public Quaternion LeftHandRotation;

    public Vector3 RightHandPosition;
    public Quaternion RightHandRotation;
}
