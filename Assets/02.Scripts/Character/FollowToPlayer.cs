using UnityEngine;

public class FollowToPlayer : MonoBehaviour
{
    public Transform Target;
    public Transform MirrorTransform;
	 
    private void Update()
    {
        Vector3 localPlayer = MirrorTransform.InverseTransformPoint(Target.position);
        transform.position = MirrorTransform.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));
	 
	 
        Vector3 localMirror = MirrorTransform.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
        transform.LookAt(localMirror);
    }
}