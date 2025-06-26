using UnityEngine;

public class LineRenderer_Test : MonoBehaviour
{
    [SerializeField] Transform[] points;
    [SerializeField] LineController line;
    private void Start()
    {
        line.SetupLine(points);
    }
}
