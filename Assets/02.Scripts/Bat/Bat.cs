using AutoSet.Utils;
using UnityEngine;
public class Bat : MonoBehaviour
{
    [SerializeField] private float power = 1.5f;
    [SerializeField, AutoSet] private Rigidbody rigidbody;
    
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private float velocity;
    
    private void Start()
    {
        previousPosition = transform.position;
    }

    private void Update()
    {
        currentPosition = transform.position;
        var distance = Vector3.Distance(previousPosition, currentPosition);
        velocity = distance / Time.deltaTime;
        previousPosition = currentPosition;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //Todo : CollisionEnter -> Ball, 
        if (other.gameObject.TryGetComponent<IHittable>(out var o))
        {
            Rigidbody rigidBody = other.rigidbody;
            var contactPoint = other.contacts[0].point;
            
            Vector3 forceDirection = (rigidBody.transform.position - contactPoint).normalized;
            rigidBody.AddForce(forceDirection * velocity * power, ForceMode.Impulse);
            
            o.OnHit(contactPoint);
        }
    }
}
