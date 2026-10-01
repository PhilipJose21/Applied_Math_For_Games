using UnityEngine;

public enum PathType
{
    StraightLine,
    Curve
}

public class PathLogic : MonoBehaviour
{
    [Header("Path Type")]
    public PathType pathType = PathType.StraightLine;

    [Header("Curve Settings")]
    [Range(0.05f, 0.5f)] public float entryDistance = 0.5f;//how far the entry curve is from previous node

    [Range(0.05f, 0.5f)] public float exitDistance = 0.5f;//how far the exit curve is from next node

    public Vector3 controlOffset = Vector3.zero;//offset for the control point

    [Min(0.1f)] public float curveSpeedMultiplier = 1f;//multiplier for curve speed

    [Header("Editor Preview")]
    public bool drawPreview = true;
    [Range(4, 64)] public int previewSteps = 24;

    public bool IsCurve => pathType == PathType.Curve;

    public void GetCurvePoints(Vector3 prev, Vector3 next, out Vector3 p0, out Vector3 p1, out Vector3 p2)
    {
        Vector3 pos = transform.position;
        p0 = Vector3.Lerp(pos, prev, entryDistance);
        p1 = pos + controlOffset;
        p2 = Vector3.Lerp(pos, next, exitDistance);
    }

    /// <summary>Bezier lerp: a lerp between two lerps.</summary>
    public static Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }

    /// <summary>Approximate curve length so enemies keep a roughly constant speed.</summary>
    public static float ApproximateLength(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float chord = Vector3.Distance(p0, p2);
        float controlNet = Vector3.Distance(p0, p1) + Vector3.Distance(p1, p2);
        return Mathf.Max((2f * chord + controlNet) / 3f, 0.001f);
    }

    // Preview reads the same path list the enemies use (GameManager.GetPathPoints).
    void OnDrawGizmos()
    {
        if (!drawPreview || pathType != PathType.Curve) return;

        GameManager gm = GameManager._instance != null
            ? GameManager._instance
            : FindFirstObjectByType<GameManager>();
        if (gm == null) return;

        var points = gm.GetPathPoints();
        int i = points.IndexOf(transform);
        if (i < 0 || points.Count < 2) return;

        // Clamped, matching EnemyMovement
        Vector3 prev = points[Mathf.Max(i - 1, 0)].position;
        Vector3 next = points[Mathf.Min(i + 1, points.Count - 1)].position;

        GetCurvePoints(prev, next, out Vector3 p0, out Vector3 p1, out Vector3 p2);

        Gizmos.color = Color.cyan;
        Vector3 last = p0;
        for (int s = 1; s <= previewSteps; s++)
        {
            Vector3 point = QuadraticBezier(p0, p1, p2, s / (float)previewSteps);
            Gizmos.DrawLine(last, point);
            last = point;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p0, 0.1f);
        Gizmos.DrawSphere(p2, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(p1, 0.1f);
    }
}