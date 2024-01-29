using AutoSet.Utils;
using Oculus.Haptics;
using UnityEngine;
public class Bat : MonoBehaviour
{
    [SerializeField, AutoSetFromParent] private Player player;
    [SerializeField] private float power = 1.5f;
    //[SerializeField, AutoSet] private Rigidbody rigidbody;
    
    public enum HandTypeEnum
    {
        Left,
        Right,
        Both
    }

    [Space(3)] 
    public HandTypeEnum HandType;

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
    
    private void Start()
    {
        previousPosition = transform.position;
        transform.localRotation = Quaternion.Euler(328.599274f,115.034142f,52.5605927f);
    }

    private void Update()
    {
        currentPosition = transform.position;
        var distance = Vector3.Distance(previousPosition, currentPosition);
        velocity = distance / Time.deltaTime;
        previousPosition = currentPosition;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //Todo : CollisionEnter -> Ball, 
        //if (!player.IsMyTurn) return;
        if (other.gameObject.TryGetComponent<IHittable>(out var o))
        {
            Rigidbody rigidBody = other.rigidbody;
            var contactPoint = other.contacts[0].point;
            
            Vector3 forceDirection = (rigidBody.transform.position - contactPoint).normalized;
            rigidBody.AddForce(forceDirection * velocity * power, ForceMode.Impulse);
            
            o.OnHit(contactPoint, this);
            PlayHaptic();
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
