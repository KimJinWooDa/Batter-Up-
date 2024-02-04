using AutoSet.Utils;
using Fusion;
using Oculus.Haptics;
using UnityEngine;
public class Bat : NetworkBehaviour
{
    [SerializeField, AutoSetFromParent] private Player player;
    [SerializeField] private float power = 1.5f;
    //[SerializeField, AutoSet] private Rigidbody rigidbody;
    private float lastSwingTime = -2.0f; 
    [SerializeField] private float swingCooldown = 2.0f; 
    public enum HandTypeEnum
    {
        Left,
        Right,
        Both
    }

    [Space(3)] 
    public HandTypeEnum HandType;
    [Space(3)] 
    public AudioSource AudioSource;
    
    private HapticClipPlayer targetHapticPlayer;
    private HapticData hapticData;

    public HapticData HapticData
    {
        get { return hapticData; }
        set { hapticData = value; }
    }
    
    private float hapticStrength => ES3.Load<float>(SaveDataKeys.Haptic);

    
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private float velocity;
    [SerializeField] private float checkVelocity = 5f;
    private bool canSwing;
    
    private void Start()
    {
        previousPosition = transform.position;
        transform.localRotation = Quaternion.Euler(328.599274f,115.034142f,52.5605927f);
        //canSwing = false;
    }

    private void Update()
    {
        /*if (Time.time - lastSwingTime >= swingCooldown)
        {
            canSwing = true;
            lastSwingTime = Time.time; 
        }*/
        
        currentPosition = transform.position;
        var distance = Vector3.Distance(previousPosition, currentPosition);
        velocity = distance / Time.deltaTime;
        previousPosition = currentPosition;
        
        if (velocity >= checkVelocity)
        {
            float strength = MathUtil.Remap(velocity, 0f, 10f, 0f, 1);
            AudioSource.volume = strength;
            if (AudioSource.isPlaying)
            {
                AudioSource.Stop(); //?
                AudioSource.Play();
                return;
            }
            AudioSource.Play();
        }
        else
        {
            AudioSource.Stop();
        }
    }
    
    private Rigidbody otherRigidbody = null;
    private void OnCollisionEnter(Collision other)
    {
        //if(!canSwing) return;
        
        //Todo : CollisionEnter -> Ball, 
        //if (!player.IsMyTurn) return;
        if (other.gameObject.TryGetComponent<IHittable>(out var o))
        {
            otherRigidbody = other.rigidbody;
            var contactPoint = other.contacts[0].point;
            
            Vector3 forceDirection = (otherRigidbody.transform.position - contactPoint).normalized;
            //rigidBody.AddForce(forceDirection * velocity * power, ForceMode.Impulse);
            RpcAddForceToBall(forceDirection * velocity * power);

            o.OnHit(contactPoint, this);
            PlayHaptic();
            
            canSwing = false;
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcAddForceToBall(Vector3 force)
    {
        if(otherRigidbody != null)
        {
            otherRigidbody.AddForce(force, ForceMode.Impulse);
            otherRigidbody = null;
        }
    }

    private void PlayHaptic()
    {
        Controller targetController =
            HandType == HandTypeEnum.Right ? Controller.Right : Controller.Left;
        targetHapticPlayer?.Dispose();
        float strength = MathUtil.Remap(velocity, 0f, 10f, 0f, hapticStrength);
        targetHapticPlayer = new HapticClipPlayer(HapticData.HapticClip)
        {
            amplitude = HapticData.Amplitude * strength,
            frequencyShift = HapticData.FrequencyShift,
        };
        targetHapticPlayer.Play(targetController);
    }

    private void OnDestroy()
    {
        targetHapticPlayer?.Dispose();
    }
}
