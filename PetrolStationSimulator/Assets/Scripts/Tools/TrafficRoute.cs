using System.Collections.Generic;
using UnityEngine;

public class TrafficRoute : MonoBehaviour
{
    [Header("Route Settings")]
    [Tooltip("Place your cubes/transforms in order here")]
    public List<Transform> controlPoints = new List<Transform>();
    
    [Tooltip("How smooth the curve should be (higher = smoother)")]
    public int resolution = 10;

    // We pre-calculate the path points so the cars can easily follow them at uniform speeds
    [HideInInspector]
    public List<Vector3> pathPoints = new List<Vector3>();

    void Awake()
    {
        CalculatePath();
    }

    // This calculates the path in the editor so you can see it without pressing Play
    void OnValidate()
    {
        CalculatePath();
    }

    void CalculatePath()
    {
        pathPoints.Clear();
        if (controlPoints.Count < 2) return;

        // Simple straight line if only 2 points
        if (controlPoints.Count == 2)
        {
            pathPoints.Add(controlPoints[0].position);
            pathPoints.Add(controlPoints[1].position);
            return;
        }

        // Catmull-Rom Spline calculation (passes exactly through your cubes)
        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            Vector3 p0 = controlPoints[Mathf.Max(i - 1, 0)].position;
            Vector3 p1 = controlPoints[i].position;
            Vector3 p2 = controlPoints[Mathf.Min(i + 1, controlPoints.Count - 1)].position;
            Vector3 p3 = controlPoints[Mathf.Min(i + 2, controlPoints.Count - 1)].position;

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;
                pathPoints.Add(GetCatmullRomPosition(t, p0, p1, p2, p3));
            }
        }
        
        // Add the very last point
        pathPoints.Add(controlPoints[controlPoints.Count - 1].position);
    }

    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
    }

    // Draw the curve in the Editor!
    void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Count < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
        }

        // Draw spheres at the actual cubes
        Gizmos.color = Color.yellow;
        foreach (var point in controlPoints)
        {
            if (point != null) Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
}