using AutoSet.Utils;
using UnityEngine;

public class Ball : MonoBehaviour, IHittable
{
    [Header("Audio")]
    public AudioSource AudioSource;
    public AudioClip BallHitSoundClip;
    
    [Space(3)]
    [Header("VFX")]
    public ParticleSystem ParticleSystem;

    [Space(3)]
    [Header("Haptic")]
    public HapticData HapticData;

  
    [Space(3)]
    [Header("Settings")]
    [SerializeField] private float impulsePower = 1.2f;
    [SerializeField, AutoSet] private Rigidbody rigidBody;


    public void OnHit(Vector3 contactPoint, Bat bat)
    {
        if (AudioSource != null)
        {
            //Todo : RPC
            AudioSource.PlayClipAtPoint(BallHitSoundClip, contactPoint);
        }

        if (ParticleSystem != null)
        {
            //Todo : RPC
            Instantiate(ParticleSystem, contactPoint, Quaternion.identity);
        }

        if (HapticData != null)
        {
            bat.HapticData = HapticData;
        }
        
       // SoundManager.Instance.PlaySound(soundName: SoundOfBall, point, volume);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Vector3 pointOfImpact = other.contacts[0].point;

            Vector3 forceDirection = other.gameObject.transform.position - pointOfImpact;
            forceDirection.Normalize();
            
            var a = other.gameObject.GetComponent<Rigidbody>();
            a.AddForce(forceDirection * rigidBody.velocity.magnitude * impulsePower, ForceMode.Impulse);

            Vector3 torqueDirection = Vector3.Cross(forceDirection, Vector3.up);
            a.AddTorque(torqueDirection * rigidBody.velocity.magnitude * impulsePower, ForceMode.Impulse);
        }
    }
}