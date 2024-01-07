using Oculus.Haptics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HapticManager : Singleton<HapticManager>
{
    [TabGroup("Tab","Haptic", SdfIconType.Hammer, TextColor = "white")]
    [TabGroup("Tab","Haptic")] [SerializeField] private Slider hapticSlider;
    [TabGroup("Tab","Haptic")] [SerializeField] private HapticPlayer LeftHapticPlayer;
    [TabGroup("Tab","Haptic")] [SerializeField] private HapticPlayer RightHapticPlayer;
    public float HapticStrength;
    private Controller targetController;
    private HapticClipPlayer currentHapticClipPlayer;
    private void Start()
    {
        if (ES3.KeyExists(SaveDataKeys.HapticStrengthKey))
        {
            HapticStrength = ES3.Load<float>(SaveDataKeys.HapticStrengthKey);

            hapticSlider.value = HapticStrength;
        }
    }
    
    public void HapticStrengthChanged(Slider slider)
    {
        UpdateAudioMixerGroup(slider.value);
    }

    public void UpdateAudioMixerGroup(float volume)
    {
        HapticStrength = volume;
        ES3.Save(SaveDataKeys.HapticStrengthKey, HapticStrength);
    }

    public void PlayHaptic(HapticData hapticData, float volume)
    { 
        if (RightHapticPlayer.IsGrabbed)
        {
            targetController = Controller.Right;
        }
        else if(LeftHapticPlayer.IsGrabbed)
        {
            targetController = Controller.Left;
        }
        else if(RightHapticPlayer.IsGrabbed && LeftHapticPlayer.IsGrabbed)
        {
            targetController = Controller.Both;
        }
        else
        {
            return;
        }
        
        float strength = MathUtil.Remap(volume, 0, 5, 0, HapticStrength);

        var haptic = new HapticClipPlayer(hapticData.HapticClip)
        {
            amplitude = strength,
            frequencyShift = hapticData.FrequencyShift,
            isLooping = hapticData.IsLooping
        };
        
        haptic.Play(targetController);
        currentHapticClipPlayer = haptic;
    }

    public void StopHaptic()
    {
        currentHapticClipPlayer?.Stop();
    }
    
    private void OnDestroy()
    {
        currentHapticClipPlayer?.Dispose();
        ES3.Save(SaveDataKeys.HapticStrengthKey, HapticStrength);
    }
}
