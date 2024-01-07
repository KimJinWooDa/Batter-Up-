using Oculus.Haptics;
using UnityEngine;

public class HapticData : MonoBehaviour
{
    public enum InteractableTypeEnum
    {
        Poke,
        Ray,
        Grab,
        Everyone,
    }
    [SerializeField] private InteractableTypeEnum m_InteractableType;
    public InteractableTypeEnum InteractableType => m_InteractableType;
    [SerializeField] private HapticClip m_HapticClip;
    public HapticClip HapticClip => m_HapticClip;
    [SerializeField] private float m_Amplitude;
    public float Amplitude => m_Amplitude;
    [SerializeField] private float m_FrequencyShift;
    public float FrequencyShift => m_FrequencyShift;
    [SerializeField] private bool m_IsLooping;
    public bool IsLooping => m_IsLooping;
    [SerializeField] private bool m_WhenSelect;
    public bool WhenSelect => m_WhenSelect;
    [SerializeField] private bool m_WhenUnSelect;
    public bool WhenUnSelect => m_WhenUnSelect;

    [SerializeField] private bool m_WhenHover;
    public bool WhenHover => m_WhenHover;
}
