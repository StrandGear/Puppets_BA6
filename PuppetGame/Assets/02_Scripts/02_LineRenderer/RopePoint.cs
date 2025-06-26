using UnityEngine;

public class RopePoint : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;

    public RopePoint(Vector3 position)
    {
        this.position = position;
        velocity = Vector3.zero;
    }
}
