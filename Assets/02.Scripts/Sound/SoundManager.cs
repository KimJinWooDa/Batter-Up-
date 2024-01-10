using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [TabGroup("Tab","BGM", SdfIconType.FileMusic, TextColor = "orange")]
    [TabGroup("Tab","BGM")] [SerializeField] private AudioMixerGroup bgmAudioMixerGroup;
    [TabGroup("Tab","BGM")] [SerializeField] private AudioSource currentBgmSound;
    [TabGroup("Tab","BGM")] [SerializeField] private Sound[] bgmSounds;
    
    [TabGroup("Tab","SFX", SdfIconType.Soundwave, TextColor = "yellow")]
    [TabGroup("Tab","SFX")] [SerializeField] private AudioMixerGroup sfxAudioMixerGroup;
    [TabGroup("Tab","SFX")] [SerializeField] private Sound[] sfxSounds;

    [TabGroup("Tab","Master", SdfIconType.Mastodon, TextColor = "blue")]
    [TabGroup("Tab","Master")] [SerializeField] private AudioMixerGroup masterAudioMixerGroup;

    [TabGroup("Tab","Slider", SdfIconType.Sliders, TextColor = "green")]
    [TabGroup("Tab","Slider")] [SerializeField] private Slider masterSlider;
    [TabGroup("Tab","Slider")] [SerializeField] private Slider bgmSlider;
    [TabGroup("Tab","Slider")] [SerializeField] private Slider sfxSlider;

    public float MasterMixerGroupVolume { get; private set; }
    public float BgmMixerGroupVolume { get; private set; }
    public float SfxMixerGroupVolume { get; private set; }
    
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private new async void Awake()
    {
        await InitializeSounds(bgmSounds, bgmAudioMixerGroup);
        await InitializeSounds(sfxSounds, sfxAudioMixerGroup);
    }

    private void Start()
    {
        if (ES3.KeyExists(SaveDataKeys.BGMVolumeKey) || ES3.KeyExists(SaveDataKeys.SFXVolumeKey) || ES3.KeyExists(SaveDataKeys.MasterVolumeKey))
        {
            BgmMixerGroupVolume = ES3.Load<float>(SaveDataKeys.BGMVolumeKey);
            SfxMixerGroupVolume = ES3.Load<float>(SaveDataKeys.SFXVolumeKey);
            MasterMixerGroupVolume = ES3.Load<float>(SaveDataKeys.MasterVolumeKey);

            masterSlider.value = MasterMixerGroupVolume;
            bgmSlider.value = BgmMixerGroupVolume;
            sfxSlider.value = SfxMixerGroupVolume;
            
            bgmAudioMixerGroup.audioMixer.SetFloat(SaveDataKeys.BGMVolumeKey, Mathf.Log10(BgmMixerGroupVolume) * 20);
            sfxAudioMixerGroup.audioMixer.SetFloat(SaveDataKeys.SFXVolumeKey, Mathf.Log10(SfxMixerGroupVolume) * 20);
            masterAudioMixerGroup.audioMixer.SetFloat(SaveDataKeys.MasterVolumeKey, Mathf.Log10(MasterMixerGroupVolume) * 20);
        }
    }

    private async UniTask InitializeSounds(Sound[] sounds, AudioMixerGroup mixerGroup)
    {
        foreach (var sound in sounds)
        {
            sound.AudioSource = gameObject.AddComponent<AudioSource>();

            sound.AudioSource.clip = sound.AudioClip;
            sound.AudioSource.volume = sound.Volume;
            sound.AudioSource.pitch = sound.Pitch;
            sound.AudioSource.loop = sound.IsLoop;
            sound.AudioSource.playOnAwake = sound.IsPlayOnAwake;
            sound.AudioSource.outputAudioMixerGroup = mixerGroup;

            if (sound.IsPlayOnAwake)
            {
                if (sound.IsDelayed)
                {
                    await WaitForSecondsUniTask(sound.DelayTime, cancellationTokenSource.Token);
                }
                sound.AudioSource.Play();

            }
        }
    }
    
    private async UniTask WaitForSecondsUniTask(float seconds, CancellationToken cancellationToken)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: cancellationToken);
    }
    
    private void StopAllTasks()
    {
        cancellationTokenSource.Cancel();
    }

    public void PlayBgm(string soundName)
    {
        Sound bgm = bgmSounds.FirstOrDefault(o => o.SoundName == soundName);
        if (bgm == null)
        {
            Debug.LogWarning("BGM:[" + soundName + "] does not Exist!");
            return;
        }

        currentBgmSound?.Stop();
        bgm.AudioSource.Play();
        currentBgmSound = bgm.AudioSource;

    }

    public void PlaySound(string soundName, Vector3 position, float volume)
    {
        Sound sfx = sfxSounds.FirstOrDefault(o => o.SoundName == soundName);
        if (sfx == null)
        {
            Debug.LogWarning($"SFX:[{soundName}] does not Exist!");
            return;
        }
        
        GameObject tempSound = new GameObject(soundName);
        //GameObject tempSound = AudioSourcePool.Instance.Get();
        tempSound.transform.position = position;
        AudioSource audioSource = tempSound.AddComponent<AudioSource>();
        audioSource.clip = sfx.AudioClip;

        float strength = MathUtil.Remap(volume, 0, 5, 0, SfxMixerGroupVolume);
        Debug.Log(strength);
        audioSource.volume = strength;
        audioSource.spatialBlend = sfx.SpatialBlend;
        audioSource.outputAudioMixerGroup = sfxAudioMixerGroup;

        audioSource.Play();

        //await UniTask.Delay(TimeSpan.FromSeconds(audioSource.clip.length));
        Destroy(tempSound, sfx.AudioClip.length);
        //todo : refactoring Pool
        //AudioSourcePool.Instance.ReturnToPool(tempSound);
    }

    private AudioClip RandomClipWithoutRepeat(Sound sound)
    {
        if (sound.RandomClips.Length == 0)
        {
            return sound.AudioClip;
        }

        int randomOffset = UnityEngine.Random.Range(1, sound.RandomClips.Length);
        int index = (sound.PreviousAudioClipIndex + randomOffset) % sound.RandomClips.Length;
        sound.PreviousAudioClipIndex = index;
        return sound.RandomClips[index];
    }

    public void BgmVolumeChanged(Slider slider)
    {
        UpdateAudioMixerGroup(slider.value, Sound.SoundType.BGM);
    }

    public void SfxVolumeChanged(Slider slider)
    {
        UpdateAudioMixerGroup(slider.value, Sound.SoundType.SFX);
    }

    public void MasterVolumeChanged(Slider slider)
    {
        UpdateAudioMixerGroup(slider.value);
    }
    
    public void UpdateAudioMixerGroup(float volume, Sound.SoundType sound = Sound.SoundType.Etc)
    {
        switch (sound)
        {
            case Sound.SoundType.BGM:
                BgmMixerGroupVolume = volume;
                bgmAudioMixerGroup.audioMixer.SetFloat(SaveDataKeys.BGMVolumeKey, Mathf.Log10(BgmMixerGroupVolume) * 20);
                
                break;
            case Sound.SoundType.SFX:
                SfxMixerGroupVolume = volume;
                sfxAudioMixerGroup.audioMixer.SetFloat(SaveDataKeys.SFXVolumeKey, Mathf.Log10(SfxMixerGroupVolume) * 20);
               
                break;
            default:
                MasterMixerGroupVolume = volume;
                masterAudioMixerGroup.audioMixer.SetFloat(SaveDataKeys.MasterVolumeKey, Mathf.Log10(MasterMixerGroupVolume) * 20);
               
                break;
        }
        
        ES3.Save(SaveDataKeys.BGMVolumeKey, BgmMixerGroupVolume);
        ES3.Save(SaveDataKeys.SFXVolumeKey, SfxMixerGroupVolume);
        ES3.Save(SaveDataKeys.MasterVolumeKey, MasterMixerGroupVolume);
    }

    private void OnDestroy()
    {
        ES3.Save(SaveDataKeys.BGMVolumeKey, BgmMixerGroupVolume);
        ES3.Save(SaveDataKeys.SFXVolumeKey, SfxMixerGroupVolume);
        ES3.Save(SaveDataKeys.MasterVolumeKey, MasterMixerGroupVolume);
    }
}
