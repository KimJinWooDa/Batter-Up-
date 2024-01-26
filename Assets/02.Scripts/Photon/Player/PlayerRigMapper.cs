using UnityEngine;


[DisallowMultipleComponent]
public class PlayerRigMapper : MonoBehaviour
{
    [Header("VR Rig")]
    [SerializeField] private Transform rigHead = null;
    [SerializeField] private Transform rigLeftHand = null;
    [SerializeField] private Transform rigRightHand = null;

    // Open variables
    public Vector3 RigHeadPosition => rigHead.position;
    public Quaternion RigHeadRotation => rigHead.rotation;
    public Vector3 RigLeftHandPosition => rigLeftHand.position;
    public Quaternion RigLeftHandRotation => rigLeftHand.rotation;
    public Vector3 RigRightHandPosition => rigRightHand.position;
    public Quaternion RigRightHandRotation => rigRightHand.rotation;

}
