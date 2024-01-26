using UnityEngine;

public class Ball : MonoBehaviour, IHittable
{
    [Header("Audio")]
    public AudioSource AudioSource;
    public AudioClip BallHitSoundClip;
    
    [Space(3)]
    [Header("VFX")]
    public ParticleSystem ParticleSystem;

    private void Awake()
    {
        //Debug.Assert(AudioSource != null);
       // Debug.Assert(ParticleSystem != null);
    }
    
    public void OnHit(Vector3 contactPoint)
    {
        if (AudioSource != null)
        {
            AudioSource.PlayClipAtPoint(BallHitSoundClip, contactPoint);
        }

        if (ParticleSystem != null)
        {
            Instantiate(ParticleSystem, contactPoint, Quaternion.identity);
        }
       // SoundManager.Instance.PlaySound(soundName: SoundOfBall, point, volume);
       // HapticManager.Instance.PlayHaptic(HapticData, volume);
       //VFX
    }
    
    
}