using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class AudioManager : MonoBehaviour
{
    private AudioSourcePool audioSourcePool;
    private AudioSource musicSource;
    private Dictionary<string, SoundData> soundLibrary; 

    public float GlobalVolume { get; set; } = 1.0f;

    public AudioManager(AudioSourcePool pool, AudioSource musicAudioSource)
    {
        audioSourcePool = pool;
        musicSource = musicAudioSource;
        soundLibrary = new Dictionary<string, SoundData>();
    }
    
    public async UniTask PlaySound(SoundData soundData, Vector3 position, float fadeInDuration = 0, CancellationToken cancellationToken = default)
    {
        var sourceWrapper = audioSourcePool.GetAvailableSource();
        if (sourceWrapper != null)
        {
            await sourceWrapper.Play(soundData, position, fadeInDuration, cancellationToken);
        }
    }

    public void PlayMusic(string key, bool loop = true)
    {
        if (soundLibrary.TryGetValue(key, out SoundData soundData))
        {
            musicSource.clip = soundData.audioClip;
            musicSource.loop = loop;
            musicSource.volume = soundData.volume * GlobalVolume;
            musicSource.Play();
        }
    }

    public void SetGlobalVolume(float volume)
    {
        GlobalVolume = volume;
        musicSource.volume *= GlobalVolume;
    }
}