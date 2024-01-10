using UnityEngine;
public class Bat : MonoBehaviour
{
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
        var a = other.gameObject.GetComponent<IHittable>();
        if (a == null)
        {
            return;
        }

        var b = other.contacts[0].point;
        a.PlaySound(b, velocity);
        a.PlayHaptic(velocity);
    }

   
}
