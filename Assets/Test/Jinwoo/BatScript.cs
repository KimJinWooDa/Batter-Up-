using UnityEngine;

public class BatScript : MonoBehaviour
{
    private Vector3 lastPosition;
    private float batSpeed;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        batSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;
    }

    public float GetBatSpeed()
    {
        return batSpeed;
    }
}