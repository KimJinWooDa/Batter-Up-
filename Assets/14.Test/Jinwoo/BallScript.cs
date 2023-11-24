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
               
                float forceMagnitude = hitDirection.magnitude * rb.mass;
                rb.AddForce(hitDirection.normalized * forceMagnitude * power, ForceMode.Impulse);
                
                AudioSource.Play();
                SingleHapticTest.PlayRightHapticClip();
            }
        }
    }

    Vector3 CalculateHitDirection(ContactPoint contact, Vector3 batVelocity, Vector3 batAngularVelocity)
    {
        // Calculate the normal at the point of contact
        Vector3 collisionNormal = contact.normal;

        // Calculate the bat's movement at the point of contact
        // This includes the linear velocity and the additional component due to angular velocity
        Vector3 batMovementAtContact = batVelocity + Vector3.Cross(batAngularVelocity, contact.point - transform.position);

        // Project the bat's movement onto the collision normal
        // This will give us the component of the bat's movement that is directed towards pushing the ball
        Vector3 projectedVelocity = Vector3.Project(batMovementAtContact, collisionNormal);

        // Return the projected velocity as the hit direction
        // This ensures that the ball is pushed in the direction the bat is moving, proportional to the bat's speed
        return projectedVelocity;
    }

}