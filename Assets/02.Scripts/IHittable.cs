using UnityEngine;

public interface IHittable
{
    void OnHit(Vector3 contactPoint, Bat bat);
}
