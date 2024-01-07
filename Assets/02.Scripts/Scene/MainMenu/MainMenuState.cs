using UnityEngine;

public class MainMenuState : MonoBehaviour
{
    public string SoundName;
    private void Start()
    {
        SoundManager.Instance.PlayBgm(SoundName);
    }

}
