using UnityEngine;

public class Respawn : MonoBehaviour
{
    Vector2 lastCheckpointPos;

    private PuzzleBlock currPuzzle;

    private void Start()
    {
        lastCheckpointPos = transform.position; // fallback spawn
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Checkpoint")
        {
            currPuzzle = collision.GetComponentInParent<PuzzleBlock>();
            lastCheckpointPos = new Vector2(collision.transform.position.x, transform.position.y);
        }
        if (collision.tag == "Death")
        {
            if (currPuzzle != null)
                currPuzzle.ResetPuzzle();

            CharacterDies();
        }
    }

    void CharacterDies()
    {
        //timer + animation
        transform.position = lastCheckpointPos;
    }
}
