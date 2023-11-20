using Oculus.Haptics;
using UnityEngine;

public class SingleHapticTest : MonoBehaviour
{
    public HapticClip clip; 
    private HapticClipPlayer player;
    public Controller LeftHapticControl;
    public Controller RightHapticControl;
    void Awake()
    {
        player = new HapticClipPlayer(clip);
    }
	 
    public void PlayLeftHapticClip()
    {
        player.Play(LeftHapticControl);
    }
	 
    public void PlayRightHapticClip()
    {
        player.Play(RightHapticControl);
    }
	 
    public void AnotherGameEvent()
    {
        player.Stop();
    }
    
  
}