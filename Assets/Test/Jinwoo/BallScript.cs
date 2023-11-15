using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float power = 0.01f;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private SingleHapticTest SingleHapticTest;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bat"))
        {
            BatScript bat = collision.gameObject.GetComponent<BatScript>();
            if (bat != null)
            {
                Vector3 batVelocity = bat.GetVelocity();
                Vector3 batAngularVelocity = bat.GetAngularVelocity();
                ContactPoint contact = collision.contacts[0];

                // 충돌 지점에서의 방망이 움직임을 고려한 타격 방향 계산
                Vector3 hitDirection = CalculateHitDirection(contact, batVelocity, batAngularVelocity);

                // 공에 힘 가하기
                AudioSource.Play();
                rb.AddForce(hitDirection * power, ForceMode.Impulse);
                SingleHapticTest.PlayRightHapticClip();
            }
        }
    }

    Vector3 CalculateHitDirection(ContactPoint contact, Vector3 batVelocity, Vector3 batAngularVelocity)
    {
        // 타격 방향 계산 로직
        // 여기서는 충돌 지점과 방망이의 속도, 각속도를 고려한 복잡한 계산을 수행합니다.

        // 충돌 지점에서의 방망이 움직임을 고려
        Vector3 batMovementAtContact = batVelocity + Vector3.Cross(batAngularVelocity, contact.point - transform.position);

        // 타격 방향은 방망이의 움직임과 충돌 법선 벡터를 기반으로 계산
        Vector3 hitDirection = batMovementAtContact.normalized + contact.normal;

        // 공에 가할 힘의 크기 조정 (이 값은 게임의 물리적 느낌에 따라 조정 가능)
        hitDirection *= batMovementAtContact.magnitude;

        return hitDirection;
    }
}