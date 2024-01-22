using AutoSet.Utils;
using UnityEngine;
public class Bat : MonoBehaviour
{
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private float velocity;
    [SerializeField] private float power = 10f;
    [SerializeField, AutoSet] private Rigidbody rigidbody;
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
        var a = other.gameObject.GetComponent<IHittable>();
        if (a == null)
        {
            return;
        }
        
        Rigidbody rb = other.rigidbody;
        
        var b = other.contacts[0].point;
        Vector3 forceDirection = (rb.transform.localPosition - b).normalized;
        float forceMagnitude = rigidbody.velocity.magnitude * rigidbody.mass * velocity;
        
        rb.AddForce(forceDirection * forceMagnitude * power, ForceMode.Impulse);
        a.PlaySound(b, velocity);
        a.PlayHaptic(velocity);
    }

}
