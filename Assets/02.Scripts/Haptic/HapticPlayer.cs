using Oculus.Interaction.HandGrab;
using Sirenix.OdinInspector;
using UnityEngine;

public class HapticPlayer : MonoBehaviour
{
    public bool IsGrabbed;
    
    [SerializeField] private HandGrabInteractor m_GrabInteractor;
    
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
            IsGrabbed = true;
        }
    }
    
    private void OnGrabInteractableUnSelected(HandGrabInteractable interactable)
    {
        var a = interactable.gameObject.GetComponentsInParent<Bat>();
        if (a != null)
        {
            IsGrabbed = false;
        }
        
        HapticManager.Instance.StopHaptic();
    }
    
    

    private void OnDestroy()
    {
        m_GrabInteractor.WhenInteractableSelected.Action -= OnGrabInteractableSelected;
        m_GrabInteractor.WhenInteractableUnselected.Action -= OnGrabInteractableUnSelected;
    }
}
