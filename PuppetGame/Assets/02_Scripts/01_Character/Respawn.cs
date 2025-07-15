using UnityEngine;

public class Respawn : MonoBehaviour
{
    Vector2 lastCheckpointPos;

    private PuzzleBlock currPuzzle;

    [SerializeField] Animator curtainAnimation;

    private Timer respawnTimer;

    private void Start()
    {
        lastCheckpointPos = transform.position; // fallback spawn
    }
    private void Update()
    {
        if (respawnTimer != null)
            respawnTimer.Update();
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
        curtainAnimation.SetTrigger("PlayCurtainAnim");

        Time.timeScale = 0f;
        respawnTimer = new Timer(0.3f, () =>
        {
            Time.timeScale = 1f;
            transform.position = lastCheckpointPos;
        });

        respawnTimer.Start();
    }
}
