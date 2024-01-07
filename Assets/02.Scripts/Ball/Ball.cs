using UnityEngine;

public class Ball : MonoBehaviour, IHittable
{
    public HapticData HapticData;
    
    public void PlaySound(Vector3 point, float volume)
    {
        SoundManager.Instance.PlaySound(soundName: "Ball", point, volume);
    }

    public void PlayHaptic(float volume)
    {
        HapticManager.Instance.PlayHaptic(HapticData, volume);
    }
}