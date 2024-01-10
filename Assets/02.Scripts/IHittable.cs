using UnityEngine;

public interface IHittable
{
    void PlaySound(Vector3 point, float volume);
    void PlayHaptic(float volume);
}
