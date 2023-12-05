using UnityEngine;

public class SoundDataHolder : MonoBehaviour, ISoundDataHolder
{
    public SoundData soundData;

    public SoundData GetSoundData()
    {
        return soundData;
    }
}