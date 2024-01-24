using UnityEngine;

namespace Autohand.Demo{
    public class JointBreakStopForce : MonoBehaviour
    {
        void OnJointBreak(float breakForce) {
            if(gameObject.TryGetComponent(out Rigidbody body)) {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
        }
    }
}