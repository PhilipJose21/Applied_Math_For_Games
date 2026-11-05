using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponent : MonoBehaviour
{
    public static float focalLenth = 300f;
    public Vector2 vanishingPoint = Vector2.zero;
    public static Vector2 vanishing = Vector2.zero;

    private void Awake()
    {
        vanishing = vanishingPoint;
    }

    private void Update()
    {
        vanishing = vanishingPoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(vanishingPoint.x, vanishingPoint.y, 0f), 0.3f);
    }

    public static float GetPerspective(float z)
    {
        return focalLenth / Mathf.Max(0.01f, focalLenth + z);
    }

    public static Vector2 Project(Vector3 p)
    {
        float perspective = GetPerspective(p.z);
        Vector2 fromVanishing = new Vector2(p.x, p.y) - vanishing;
        return vanishing + fromVanishing * perspective;
    }
}