using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip audioClip;
    public float volume = 1.0f;
    public float pitch = 1.0f;
}