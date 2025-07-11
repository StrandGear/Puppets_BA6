using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] float m_Speed;

    [SerializeField] float m_Distance;

    [SerializeField] Direction m_direction;

    Vector2 m_targetPosition, m_endPoint, m_startPoint;

    private void OnEnable()
    {
        m_endPoint = transform.position;
        m_startPoint = transform.position;
    }

    private void Start()
    {
        /*m_endPoint = transform.position;
        m_startPoint = transform.position;*/

        if (m_direction == Direction.Horizontal)
        {
            m_endPoint.x += m_Distance;
            m_startPoint.x -= m_Distance;
        }
        else
        {
            m_endPoint.y += m_Distance;
            m_startPoint.y -= m_Distance;
        }

        m_targetPosition = m_endPoint;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, m_targetPosition, m_Speed * Time.deltaTime); 

        if (m_direction == Direction.Vertical)
        {
            if (transform.position.y >= m_endPoint.y) { m_targetPosition.y = m_startPoint.y; }
            else if (transform.position.y <= m_startPoint.y) { m_targetPosition.y = m_endPoint.y; }
        }
        else
        {
            if (transform.position.x >= m_endPoint.x) { m_targetPosition.x = m_startPoint.x; }
            else if (transform.position.x <= m_startPoint.x) { m_targetPosition.x = m_endPoint.x; }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, m_startPoint, Color.cyan);
        Debug.DrawLine(transform.position, m_endPoint, Color.darkCyan);
    }
}


public enum Direction
{
    Horizontal,
    Vertical
}