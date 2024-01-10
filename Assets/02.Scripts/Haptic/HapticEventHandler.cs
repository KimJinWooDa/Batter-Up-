using Oculus.Haptics;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Sirenix.OdinInspector;
using UnityEngine;


public class HapticEventHandler : MonoBehaviour
{
    [SerializeField] private PokeInteractor m_PokeInteractor;
    [SerializeField] private HandGrabInteractor m_GrabInteractor;
    [SerializeField] private RayInteractor m_RayInteractor;

    public enum HandTypeEnum
    {
        Left,
        Right,
        Both
    }

    [Space(10)] [TitleGroup("Haptic")] public HandTypeEnum HandType;

    private HapticClipPlayer m_TargetHapticPlayer;

    private float m_HapticStrength => ES3.Load<float>(SaveDataKeys.HapticStrengthKey);

    private void Start()
    {
        m_PokeInteractor.WhenInteractableSelected.Action += OnPokeInteractableSelected;
        m_GrabInteractor.WhenInteractableSelected.Action += OnGrabInteractableSelected;
        m_RayInteractor.WhenInteractableSelected.Action += OnRayInteractableSelected;

        m_PokeInteractor.WhenInteractableUnselected.Action += OnPokeInteractableUnSelected;
        m_GrabInteractor.WhenInteractableUnselected.Action += OnGrabInteractableUnSelected;
        m_RayInteractor.WhenInteractableUnselected.Action += OnRayInteractableUnSelected;

        m_RayInteractor.WhenInteractableSet.Action += OnRayInteractableHover;
    }

    private void OnPokeInteractableSelected(PokeInteractable interactable)
    {
        WhenInteractableSelected(interactable.GetComponent<HapticData>());
    }

    private void OnGrabInteractableSelected(HandGrabInteractable interactable)
    {
        WhenInteractableSelected(interactable.GetComponent<HapticData>());
    }

    private void OnRayInteractableSelected(RayInteractable interactable)
    {
        WhenInteractableSelected(interactable.GetComponent<HapticData>());
    }

    private void OnPokeInteractableUnSelected(PokeInteractable interactable)
    {
        WhenInteractableUnSelected(interactable.GetComponent<HapticData>());
    }

    private void OnGrabInteractableUnSelected(HandGrabInteractable interactable)
    {
        WhenInteractableUnSelected(interactable.GetComponent<HapticData>());
    }

    private void OnRayInteractableUnSelected(RayInteractable interactable)
    {
        WhenInteractableUnSelected(interactable.GetComponent<HapticData>());
    }

    private void OnRayInteractableHover(RayInteractable interactable)
    {
        //WhenInteractableUnSelected(interactable.GetComponent<HapticData>());
    }


    private void WhenInteractableSelected(HapticData data)
    {
        if (data == null || !data.WhenSelect)
        {
            return;
        }

        PlayHapticFeedback(data);
    }
    
    private void WhenInteractableUnSelected(HapticData data)
    {
        if(data == null)
        {
            return;
        }

        if (data.WhenUnSelect)
        {
            PlayHapticFeedback(data);
        }
        else if (!data.WhenUnSelect && m_TargetHapticPlayer != null)
        {
            m_TargetHapticPlayer?.Stop();
            m_TargetHapticPlayer?.Dispose();
            m_TargetHapticPlayer = null;
        }
    }

    private void PlayHapticFeedback(HapticData data)
    {
        Controller targetController =
            HandType == HandTypeEnum.Right ? Controller.Right : Controller.Left;
        m_TargetHapticPlayer?.Dispose();
        m_TargetHapticPlayer = new HapticClipPlayer(data.HapticClip)
        {
            amplitude = data.Amplitude * m_HapticStrength,
            frequencyShift = data.FrequencyShift,
            isLooping = data.IsLooping
        };
        m_TargetHapticPlayer.Play(targetController);
    }
    
    private void OnDestroy()
    {
        m_TargetHapticPlayer?.Dispose();

        m_PokeInteractor.WhenInteractableSelected.Action -= OnPokeInteractableSelected;
        m_GrabInteractor.WhenInteractableSelected.Action -= OnGrabInteractableSelected;
        m_RayInteractor.WhenInteractableSelected.Action -= OnRayInteractableSelected;

        m_PokeInteractor.WhenInteractableUnselected.Action -= OnPokeInteractableUnSelected;
        m_GrabInteractor.WhenInteractableUnselected.Action -= OnGrabInteractableUnSelected;
        m_RayInteractor.WhenInteractableUnselected.Action -= OnRayInteractableUnSelected;

        m_RayInteractor.WhenInteractableSet.Action -= OnRayInteractableHover;
    }
}
