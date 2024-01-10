using UnityEngine;

[System.Serializable]
public class Sound
{
    public enum SoundType
    {
        BGM,
        SFX,
        Etc,
    }
    public SoundType Type;
    
    public string SoundName;
    [HideInInspector] public AudioSource AudioSource;
    public AudioClip AudioClip;
    public AudioClip[] RandomClips;
    public bool IsPlayOnAwake;
    public bool IsLoop;

    [Range(0,1f)] public float Volume = 1f;
    [Range(0,1f)] public float Pitch = 1f;

    public bool IsDelayed = false;
    public float DelayTime;
    public bool UseRandomPlay;
    public int PreviousAudioClipIndex = -1;

    [Range(0,1f)] public float SpatialBlend = 0f;
}
