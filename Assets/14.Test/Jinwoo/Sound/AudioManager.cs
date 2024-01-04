using UnityEngine;

public class AudioManager
{
    public void PlaySound(SoundData soundData, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(soundData.audioClip, position, soundData.volume);
    }
}