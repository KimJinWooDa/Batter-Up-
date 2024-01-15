using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Eve_LocomotionController : MonoBehaviour
{
    public Transform CenterEyeTransform;
    
    
    private bool isGrabRightController; 
    private bool isGrabLeftController;
    private bool isGrabBothController;
        
    private Vector3 originPosition;
    private Quaternion originRotation;
    private Vector3 currentEyePosition;
    
    [SerializeField] private float speed;
    public float Speed => speed;

    [Range(0,10f)] [SerializeField] private float sensitivity;
    public float Sensitivity => sensitivity;
    public Transform OVRCameraRigTransform;
    private void Awake()
    {
        Assert.IsNotNull(CenterEyeTransform);
    }

    private void Start()
    {
        isGrabRightController = false;
        isGrabLeftController = false;
        isGrabBothController = false;
        
        originPosition = CenterEyeTransform.position;
        originRotation = CenterEyeTransform.rotation;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            isGrabLeftController = true;
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            isGrabLeftController = false;
        }
        
        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            isGrabRightController = true;
        }
        else if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            isGrabRightController = false;
        }

        if (isGrabLeftController && isGrabRightController)
        {
            isGrabBothController = true;
        }
        else
        {
            isGrabBothController = false;
            currentEyePosition = Vector3.zero;
        }
    }
    
    private void FixedUpdate()
    {
        if (!isGrabBothController)
        {
            return;
        }
        
        var directoin = Direction() * (Speed * Time.deltaTime);
        transform.Translate(directoin, Space.World);
    }
    
    private Vector3 Direction()
    {
        currentEyePosition = CenterEyeTransform.position;
        currentEyePosition.y = 0f;

        var playerTransform = OVRCameraRigTransform.position;
        playerTransform.y = 0f;
        
        var direction = ((currentEyePosition - playerTransform).normalized) * Sensitivity;
        direction.y = 0f;
    
        return direction;
    }

    public void ChangeSensitivity(float value)
    {
        sensitivity = value;
    }

    public void ChangeSpeed(float value)
    {
        speed = value;
    }
}
