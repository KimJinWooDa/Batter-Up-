using Oculus.Haptics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HapticManager : MonoBehaviourSingleton<HapticManager>
{
    [TabGroup("Tab", "Haptic", SdfIconType.Hammer, TextColor = "white")]
    [TabGroup("Tab", "Haptic")][SerializeField] private Slider hapticSlider;

    public float HapticStrength;
    
    private void Start()
    {
        if (ES3.KeyExists(SaveDataKeys.Haptic))
        {
            HapticStrength = ES3.Load<float>(SaveDataKeys.Haptic);

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
        ES3.Save(SaveDataKeys.Haptic, HapticStrength);
    }

    protected override void OnDestroy()
    {
        ES3.Save(SaveDataKeys.Haptic, HapticStrength);
    }
}
