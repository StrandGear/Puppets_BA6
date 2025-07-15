using UnityEngine;

public class Respawn : MonoBehaviour
{
    Vector2 lastCheckpointPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Checkpoint")
            transform.position = new Vector2 (collision.transform.position.x, transform.position.y);
    }
}
