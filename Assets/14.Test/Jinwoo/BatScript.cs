using UnityEngine;

public class BatScript : MonoBehaviour
{
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 currentVelocity;
    private Vector3 currentAngularVelocity;

    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        // 선형 속도 계산
        currentVelocity = (transform.position - lastPosition) / Time.deltaTime;

        // 각속도 계산
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
        currentAngularVelocity = deltaRotation.eulerAngles / Time.deltaTime;

        // 이전 프레임 정보 업데이트
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }

    public Vector3 GetAngularVelocity()
    {
        return currentAngularVelocity;
    }
}