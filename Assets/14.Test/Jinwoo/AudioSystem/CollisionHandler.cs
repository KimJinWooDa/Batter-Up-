using UnityEngine;
using Cysharp.Threading.Tasks;

public class CollisionHandler : MonoBehaviour
{
    private AudioManager audioManager;

    public void Initialize(AudioManager manager)
    {
        audioManager = manager;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var intensity = CalculateImpactIntensity(collision);

        PlaySoundFromSoundDataHolder(collision);

       // PlaySoundBasedOnIntensity(intensity, collision);
    }

    private void PlaySoundFromSoundDataHolder(Collision collision)
    {
        SoundDataHolder soundDataHolder = collision.gameObject.GetComponent<SoundDataHolder>();
        if (soundDataHolder != null && soundDataHolder.soundData != null)
        {
            AudioSource.PlayClipAtPoint(soundDataHolder.soundData.audioClip, collision.contacts[0].point, soundDataHolder.soundData.volume);
        }
    }

    private void PlaySoundBasedOnIntensity(float intensity, Collision collision)
    {
        var soundData = GetSoundDataBasedOnIntensity(intensity);
        if (soundData != null)
        {
            audioManager.PlaySound(soundData, collision.transform.position, GetFadeInDuration(intensity), this.GetCancellationTokenOnDestroy()).Forget();
        }
    }

    private float CalculateImpactIntensity(Collision collision)
    {
        return collision.relativeVelocity.magnitude;
    }

    private SoundData GetSoundDataBasedOnIntensity(float intensity)
    {
        // 여기서는 예시로 빈 SoundData를 반환하고 있습니다. 실제 구현에서는 강도에 따라 다른 SoundData를 반환해야 합니다.
        return new SoundData();
    }

    private float GetFadeInDuration(float intensity)
    {
        return intensity > 5 ? 0.5f : 0.1f;
    }
}