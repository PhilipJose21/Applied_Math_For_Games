using System.Collections.Generic;
using UnityEngine;

public class SphereMesh : MonoBehaviour
{
    #region Static Variables

    private Vector3[] verts;
    private int[][] faces;

    private readonly List<int> visibleEdges = new();

    #endregion

    [SerializeField] private Material lineMat;
    [SerializeField] private Vector3 position = Vector3.zero;
    [SerializeField] private float scale = 1;

    [Header("Sphere Detail")]
    [Range(6, 64)] [SerializeField] private int segments = 12;
    [Tooltip("Slices from top pole to bottom pole")]
    [Range(3, 64)] [SerializeField] private int rings = 8;

    [Header("Rotation (degrees)")]
    [SerializeField] private float xRotation = 0;
    [SerializeField] private float yRotation = 0;
    [SerializeField] private Vector2 rotationSpeed = new(30f, 45f);

    [Range(1, 500)] [SerializeField] private float focalLength = 20f;

    private Mesh mesh;
    private readonly List<Vector3> actualPosition = new();

    #region Unity Functions

    private void Awake()
    {
        segments = Mathf.Max(6, segments);
        rings = Mathf.Max(3, rings);
        BuildSphere();

        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = lineMat;
    }

    void LateUpdate()
    {
        // spin
        xRotation = (xRotation + rotationSpeed.x * Time.deltaTime) % 360f;
        yRotation = (yRotation + rotationSpeed.y * Time.deltaTime) % 360f;

        actualPosition.Clear();
        foreach (var point in verts)
        {
            actualPosition.Add(Project(point));
        }

        mesh.vertices = actualPosition.ToArray();
        visibleEdges.Clear();

        foreach (var f in faces)
        {
            if (!FacesCamera(actualPosition[f[0]], actualPosition[f[1]], actualPosition[f[2]])) continue;
            for (int i = 0; i < f.Length; i++)
            {
                visibleEdges.Add(f[i]);
                visibleEdges.Add(f[(i + 1) % f.Length]);
            }
        }

        mesh.SetIndices(visibleEdges, MeshTopology.Lines, 0);
        mesh.RecalculateBounds();
    }

    #endregion

    #region Helper

    private void BuildSphere()
    {
        var v = new List<Vector3>
        {
            new(0, 1, 0),
            new(0, -1, 0)
        };

        for (int r = 1; r < rings; r++)
        {
            float phi = Mathf.PI * r / rings;
            float y = Mathf.Cos(phi);
            float radius = Mathf.Sin(phi);

            for (int s = 0; s < segments; s++)
            {
                float theta = 2f * Mathf.PI * s / segments;
                v.Add(new Vector3(radius * Mathf.Cos(theta), y, radius * Mathf.Sin(theta)));
            }
        }

        int Idx(int ring, int seg) => 2 + (ring - 1) * segments + (seg % segments);

        var f = new List<int[]>();

        for (int s = 0; s < segments; s++)
            f.Add(new[] { 0, Idx(1, s), Idx(1, s + 1) });

        for (int r = 1; r < rings - 1; r++)
        {
            for (int s = 0; s < segments; s++)
            {
                int a = Idx(r, s);
                int b = Idx(r, s + 1);
                int c = Idx(r + 1, s + 1);
                int d = Idx(r + 1, s);
                f.Add(new[] { a, d, c, b });
            }
        }

        for (int s = 0; s < segments; s++)
            f.Add(new[] { 1, Idx(rings - 1, s + 1), Idx(rings - 1, s) });

        verts = v.ToArray();
        faces = f.ToArray();
    }

    private Vector2 Project(Vector3 point)
    {
        point *= .5f;

        point = Rotate(point);

        point += position;

        var p = focalLength / (focalLength + point.z);

        return new Vector2(point.x, point.y) * scale * p;
    }

    private Vector3 Rotate(Vector3 point)
    {
        float ry = yRotation * Mathf.Deg2Rad;
        float rx = xRotation * Mathf.Deg2Rad;

        float x1 = point.x * Mathf.Cos(ry) - point.z * Mathf.Sin(ry);
        float z1 = point.x * Mathf.Sin(ry) + point.z * Mathf.Cos(ry);

        float y2 = point.y * Mathf.Cos(rx) - z1 * Mathf.Sin(rx);
        float z2 = point.y * Mathf.Sin(rx) + z1 * Mathf.Cos(rx);

        return new Vector3(x1, y2, z2);
    }

    private bool FacesCamera(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b - a).x * (c - a).y - (b - a).y * (c - a).x > 0;
    }

    #endregion
}