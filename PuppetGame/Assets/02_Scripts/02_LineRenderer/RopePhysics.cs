using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class RopePhysics : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public int segmentCount = 10;
    public float segmentLength = 0.2f;
    [Range(0, 1)] public float tension = 0.9f; // 1 = tight, 0 = very loose

    private List<RopePoint> ropePoints;
    private LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segmentCount;

        ropePoints = new List<RopePoint>();
        Vector3 dir = (endPoint.position - startPoint.position).normalized;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, (float)i / (segmentCount - 1));
            ropePoints.Add(new RopePoint(pos));
        }
    }

    private void Update()
    {
        SimulatePhysics(Time.deltaTime);
        ApplyConstraints();
        DrawRope();
    }

    void SimulatePhysics(float dt)
    {
        for (int i = 1; i < segmentCount - 1; i++)
        {
            RopePoint p = ropePoints[i];

            // simple verlet-style integration
            Vector3 gravity = Vector3.down * 9.81f;
            Vector3 acceleration = gravity;

            p.velocity += acceleration * dt * (1 - tension);
            p.position += p.velocity * dt;
        }
    }

    void ApplyConstraints()
    {
        // First and last points stay fixed to transforms
        ropePoints[0].position = startPoint.position;
        ropePoints[segmentCount - 1].position = endPoint.position;

        // Distance constraint enforcement
        for (int i = 0; i < 10; i++) // Repeat to stabilize
        {
            for (int j = 0; j < segmentCount - 1; j++)
            {
                RopePoint p1 = ropePoints[j];
                RopePoint p2 = ropePoints[j + 1];

                Vector3 delta = p2.position - p1.position;
                float currentDist = delta.magnitude;
                float error = currentDist - segmentLength;
                Vector3 change = delta.normalized * error * 0.5f * tension;

                // skip fixed points
                if (j != 0)
                    p1.position += change;
                if (j != segmentCount - 2)
                    p2.position -= change;
            }
        }
    }

    void DrawRope()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            lr.SetPosition(i, ropePoints[i].position);
        }
    }
}
