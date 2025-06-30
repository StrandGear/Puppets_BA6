using UnityEngine;

public class FollowMovement : MonoBehaviour
{
    [SerializeField] GameObject m_Target;

    [SerializeField] float m_delay = 0f;

    [SerializeField] Constrains m_constrains;

    Transform m_transform;

    Vector3 m_newPos;

    Vector3 m_mask;

    private void Start()
    {
        if (m_Target == null) { Debug.LogWarning("No target to follow, script is disabled"); this.enabled = false; return; }

        m_transform = transform;

        m_mask = new Vector3(
            m_constrains.HasFlag(Constrains.X) ? 0f : 1f,
            m_constrains.HasFlag(Constrains.Y) ? 0f : 1f,
            m_constrains.HasFlag(Constrains.Z) ? 0f : 1f
            ) ;
    }

    private void Update()
    {
        Vector3 targetPos = m_Target.transform.position;
        Vector3 currentPos = m_transform.position;

        m_newPos = new Vector3(
            m_mask.x * targetPos.x + (1 - m_mask.x) * currentPos.x,
            m_mask.y * targetPos.y + (1 - m_mask.y) * currentPos.y,
            m_mask.z * targetPos.z + (1 - m_mask.z) * currentPos.z 
            );

        m_transform.position = m_newPos;
    }
}

[System.Flags]
public enum Constrains
{
    None = 0,
    X = 1 << 0,
    Y = 1 << 1,
    Z = 1 << 2
}