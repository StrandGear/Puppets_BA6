using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{
    public Transform anchorTarget;
    public Rigidbody2D endAnchorRb;
    public float followSpeed = 10f;

    void FixedUpdate()
    {
        Vector2 target = anchorTarget.position;
        Vector2 current = endAnchorRb.position;
        Vector2 move = (target - current) * followSpeed;
        endAnchorRb.linearVelocity = move;
    }
}
