using Oculus.Haptics;
using Oculus.Interaction.HandGrab;
using Sirenix.OdinInspector;
using UnityEngine;

public class HapticPlayer : MonoBehaviour
{
    [TabGroup("Tab", "Haptic")]
    [SerializeField] private HapticPlayer LeftHapticPlayer;
    
    [TabGroup("Tab", "Haptic")]
    [SerializeField] private HapticPlayer RightHapticPlayer;
    
    [SerializeField] private HandGrabInteractor m_GrabInteractor;
    
    private bool isGrabbed;
    private float hapticStrength;
    private Controller targetController;
    private HapticClipPlayer currentHapticClipPlayer;

    private void Start()
    {
        m_GrabInteractor.WhenInteractableSelected.Action += OnGrabInteractableSelected;
        m_GrabInteractor.WhenInteractableUnselected.Action += OnGrabInteractableUnSelected;
    }

    private void OnGrabInteractableSelected(HandGrabInteractable interactable)
    {
        var a = interactable.gameObject.GetComponentsInParent<Bat>();
        if (a != null)
        {
            isGrabbed = true;
        }
    }
    
    private void OnGrabInteractableUnSelected(HandGrabInteractable interactable)
    {
        var a = interactable.gameObject.GetComponentsInParent<Bat>();
        if (a != null)
        {
            isGrabbed = false;
        }
        
        StopHaptic();
    }
    
    public void PlayHaptic(HapticData hapticData, float volume)
    {
        if (RightHapticPlayer.isGrabbed)
        {
            targetController = Controller.Right;
        }
        else if (LeftHapticPlayer.isGrabbed)
        {
            targetController = Controller.Left;
        }
        else if (RightHapticPlayer.isGrabbed && LeftHapticPlayer.isGrabbed)
        {
            targetController = Controller.Both;
        }
        else
        {
            return;
        }

        float strength = MathUtil.Remap(volume, 0, 5, 0, hapticStrength);

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
        m_GrabInteractor.WhenInteractableSelected.Action -= OnGrabInteractableSelected;
        m_GrabInteractor.WhenInteractableUnselected.Action -= OnGrabInteractableUnSelected;
    }
}
