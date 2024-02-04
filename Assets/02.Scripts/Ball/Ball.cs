using System;
using AutoSet.Utils;
using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour, IHittable
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
    [SerializeField, AutoSet] private NetworkObject networkObject;
    [SerializeField] private MeshRenderer meshRenderer;
    
    private Rigidbody otherRigidBody;
    
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
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcApplyForceAndTorque(Vector3 force, Vector3 torque)
    {
        if (otherRigidBody != null)
        {
            otherRigidBody.AddForce(force, ForceMode.Impulse);
            otherRigidBody.AddTorque(torque, ForceMode.Impulse);
            otherRigidBody = null;
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Vector3 pointOfImpact = other.contacts[0].point;
            Vector3 forceDirection = other.gameObject.transform.position - pointOfImpact;
            forceDirection.Normalize();
            otherRigidBody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 torqueDirection = Vector3.Cross(forceDirection, Vector3.up);

            RpcApplyForceAndTorque(forceDirection * rigidBody.velocity.magnitude * impulsePower, 
                torqueDirection * rigidBody.velocity.magnitude * impulsePower);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Zone"))
        {
            //Despawned(networkObject.Runner, true);
            meshRenderer.enabled = false;
        }
    }
}