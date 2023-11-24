using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioSourceWrapper : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isPlaying;
    private float fadeOutDuration = 0;
    private float fadeInDuration = 0;
    private float targetVolume = 1.0f;

    public AudioSourceWrapper(AudioSource source)
    {
        audioSource = source;
        isPlaying = false;
    }

    public bool IsPlaying => isPlaying;

    public async UniTask Play(SoundData soundData, Vector3 position, float fadeInDuration = 0, CancellationToken cancellationToken = default)
    {
        SetAudioSourceProperties(soundData);
        audioSource.transform.position = position;
        if (fadeInDuration > 0)
        {
            this.fadeInDuration = fadeInDuration;
            audioSource.volume = 0;
            await FadeInAsync(cancellationToken); 
        }
        else
        {
            audioSource.volume = soundData.volume;
        }
        audioSource.Play();
        isPlaying = true;
    }

    public async UniTaskVoid Stop(float fadeOutDuration = 0)
    {
        if (fadeOutDuration > 0)
        {
            this.fadeOutDuration = fadeOutDuration;
            await FadeOutAsync(this.GetCancellationTokenOnDestroy());
        }
        else
        {
            audioSource.Stop();
            isPlaying = false;
        }
    }

    private async UniTask FadeInAsync(CancellationToken cancellationToken)
    {
        float startVolume = 0;
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += (targetVolume - startVolume) * (Time.deltaTime / fadeInDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        audioSource.volume = targetVolume;
    }

    private async UniTask FadeOutAsync(CancellationToken cancellationToken)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * (Time.deltaTime / fadeOutDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
        isPlaying = false;
    }

    public void SetVolume(float volume)
    {
        targetVolume = volume;
        if (fadeInDuration <= 0) 
        {
            audioSource.volume = volume;
        }
    }

    private void SetAudioSourceProperties(SoundData soundData)
    {
        audioSource.clip = soundData.audioClip;
        targetVolume = soundData.volume;
        audioSource.pitch = soundData.pitch;
    }
}
