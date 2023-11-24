using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject Ball;
    public Transform BallSpawnTransform;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) || OVRInput.GetDown(OVRInput.Button.Three))
        {
            Instantiate(Ball, BallSpawnTransform.position, Quaternion.identity);
        }
    }
}
