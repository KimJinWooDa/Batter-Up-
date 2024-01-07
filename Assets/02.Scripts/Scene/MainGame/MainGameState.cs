using UnityEngine;

public class MainGameState : MonoBehaviour
{
    public string SoundName;
    private void Start()
    {
        SoundManager.Instance.PlayBgm(SoundName);
    }

}
