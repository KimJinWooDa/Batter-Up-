using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = new AudioManager();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float intensity = collision.relativeVelocity.magnitude;

        var soundDataHolder = collision.gameObject.GetComponent<SoundDataHolder>();
        if (soundDataHolder != null && soundDataHolder.soundData != null)
        {
            float adjustedVolume = Mathf.Clamp(intensity / 10, 0.1f, 1f); 
            Debug.Log($"[{gameObject.name}], intensity : {intensity}, Volume : {adjustedVolume}");
            soundDataHolder.soundData.volume = adjustedVolume;

            audioManager.PlaySound(soundDataHolder.soundData, transform.position);
        }
    }
}