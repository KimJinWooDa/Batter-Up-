using AutoSet.Utils;
using Oculus.Interaction.Locomotion;
using UnityEngine;

public class PlayerLocomotorInject : MonoBehaviour
{
    [SerializeField, AutoSetFromChildren] PlayerLocomotor playerLocomotor;
    
    private void Awake()
    {
        playerLocomotor.InjectPlayerOrigin(transform.root);
    }
}
