using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bat"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;

            Transform batTransform = collision.transform;
            Vector3 batRotation = batTransform.rotation.eulerAngles;
            Vector3 hitDirection = CalculateHitDirection(hitNormal, batRotation, hitPoint);

            BatScript batScript = collision.gameObject.GetComponent<BatScript>();
            if (batScript != null)
            {
                float batSpeed = batScript.GetBatSpeed();
                rb.AddForce(hitDirection * batSpeed);
            }
        }
    }
    
    Vector3 CalculateHitDirection(Vector3 hitNormal, Vector3 batRotation, Vector3 hitPoint)
    {
        // 법선 벡터와 충돌 지점을 바탕으로 반사 벡터 계산
        Vector3 reflection = Vector3.Reflect(-hitNormal, hitPoint.normalized);

        // 방망이의 회전을 고려하여 타격 방향 조정
        Quaternion rotation = Quaternion.Euler(batRotation);
        Vector3 rotatedReflection = rotation * reflection;

        // 방망이의 속도 및 각도에 따라 타격 방향의 세기 조정
        // 이 부분은 게임의 물리 엔진 및 플레이어의 피드백에 따라 조정할 수 있음
        rotatedReflection = rotatedReflection.normalized * hitNormal.magnitude;

        return rotatedReflection;
    }

}