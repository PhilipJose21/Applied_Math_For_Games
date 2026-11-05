using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponent : MonoBehaviour
{
    public static float focalLenth = 300f;
    public static int sortingBase = 3000;   // draw order of the player's depth; keeps everything above a background at order 0
    public Vector2 vanishingPoint = Vector2.zero;

    // static copy so every script can use it without a reference to this component
    public static Vector2 vanishing = Vector2.zero;

    private void Awake()
    {
        vanishing = vanishingPoint;
    }

    // lets you drag the vanishing point around in the Inspector while the game is running
    private void Update()
    {
        vanishing = vanishingPoint;
    }

    // yellow circle = where items appear and the point they spread out from
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(vanishingPoint.x, vanishingPoint.y, 0f), 0.3f);
    }

    // 1 at z = 0, smaller the further away (bigger z), bigger than 1 when z < 0
    public static float GetPerspective(float z)
    {
        return focalLenth / Mathf.Max(0.01f, focalLenth + z);
    }

    // world position (x, y, z) -> screen position
    public static Vector2 Project(Vector3 p)
    {
        float perspective = GetPerspective(p.z);
        // at z = 0 this returns (p.x, p.y) exactly; further away it slides toward the vanishing point
        Vector2 fromVanishing = new Vector2(p.x, p.y) - vanishing;
        return vanishing + fromVanishing * perspective;
    }
}