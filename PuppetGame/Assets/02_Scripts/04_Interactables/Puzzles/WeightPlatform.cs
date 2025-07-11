using UnityEngine;

public class WeightPlatform : Puzzle
{
    [Header("P.S. The pass here is platform goes up so character is loose.")]
    [Space(10)]

    [SerializeField] Vector2 m_upPosition;
    [SerializeField] Vector2 m_downPosition;

    [SerializeField] float m_moveSpeed;

    Vector2 m_initPosition;
    private Vector2 m_targetPosition;

    [SerializeField] Timer m_positionResetTimer;

    private void Awake()
    {
        m_initPosition = transform.position;
        m_targetPosition = m_initPosition;

        m_positionResetTimer = new Timer(0.4f, ()=> { m_targetPosition = m_initPosition; });
    }

    private void Update()
    {
        m_positionResetTimer?.Update();

        transform.position = Vector2.MoveTowards(transform.position, m_targetPosition, m_moveSpeed * Time.deltaTime);
    }

    protected override void CharacterEnteredWithoutPassedRequirment()
    {
        m_positionResetTimer.Stop();

        m_targetPosition = m_downPosition;
    }

    protected override void CharacterEnteredWithPassedRequirment()
    {
        m_positionResetTimer.Stop();
        m_targetPosition = m_upPosition;
    }
    protected override void CharacterExited()
    {
        Debug.Log("InitPos = " + m_initPosition);

        m_positionResetTimer.Start();

    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, m_upPosition, Color.cyan);
        Debug.DrawLine(transform.position, m_downPosition, Color.darkCyan);
    }
}
