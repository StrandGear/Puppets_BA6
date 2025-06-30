using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Character/Abilities/AdjustTension")]
public class AdjustTension : MonoBehaviour
{
    [SerializeField] List<Transform> m_ropesAnchors = new List<Transform>();
    [SerializeField] float m_moveY = 5f;
    [SerializeField] float m_speed = 2f;

    private Vector3[] m_startPositions;
    private Vector3[] m_targetPositions;

    private bool m_moving = false;
    private bool m_isUp = true;
    private float m_lerpTime = 0f;

    private void Start()
    {
        // starting positions
        m_startPositions = new Vector3[m_ropesAnchors.Count];
        for (int i = 0; i < m_ropesAnchors.Count; i++)
        {
            m_startPositions[i] = m_ropesAnchors[i].position;
        }

        // target is same as start
        m_targetPositions = new Vector3[m_ropesAnchors.Count];
        System.Array.Copy(m_startPositions, m_targetPositions, m_startPositions.Length);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleMovement();
        }

        if (m_moving)
        {
            m_lerpTime += Time.deltaTime * m_speed;
            float t = Mathf.Clamp01(m_lerpTime);

            for (int i = 0; i < m_ropesAnchors.Count; i++)
            {
                m_ropesAnchors[i].position = Vector3.Lerp(
                    m_startPositions[i],
                    m_targetPositions[i],
                    t
                );
            }

            if (t >= 1f)
            {
                m_moving = false;

                // Swap start and target so it goes the other way next time
                var temp = m_startPositions;
                m_startPositions = m_targetPositions;
                m_targetPositions = temp;

                // Toggle state
                m_isUp = !m_isUp;
            }
        }
    }

    private void ToggleMovement()
    {
        if (m_moving) return; // prevent mid-move toggle

        // Set new target positions
        for (int i = 0; i < m_ropesAnchors.Count; i++)
        {
            if (!m_isUp)
                m_targetPositions[i] = m_startPositions[i] + new Vector3(0, m_moveY, 0);
            else
                m_targetPositions[i] = m_startPositions[i] - new Vector3(0, m_moveY, 0);
        }

        m_lerpTime = 0f;
        m_moving = true;
    }
}
