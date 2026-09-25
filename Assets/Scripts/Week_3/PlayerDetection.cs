using UnityEngine;
using System;



public class PlayerDetection : MonoBehaviour
{
    public enum DetectionRadius
    {
        Cone,
        Sphere
    }
    
    private Transform player;
    private RotateTurret rotateTurret;
    private DamagePlayer damagePlayer;
    private bool isPlayerDetected = false;

    [SerializeField] private DetectionRadius detectionRadius = DetectionRadius.Cone;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float detectionAngle = 45f;
    

    [Header("Line Renderer Components")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float startWidth = 0.1f;
    [SerializeField] private float endWidth = 0.1f;
    [SerializeField] private Color lineColor = Color.red;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = false;
        lineRenderer.positionCount = 4;

        
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rotateTurret = GetComponent<RotateTurret>();
        damagePlayer = GetComponent<DamagePlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (detectionRadius == DetectionRadius.Cone)
        {
            DrawCone();
        }
        else if (detectionRadius == DetectionRadius.Sphere)
        {
            DrawSphere();
        }

        //THIS ONE
        if (detectionRadius == DetectionRadius.Cone)
        {
            DetectionType(IsInCone(transform, player, detectionRange, detectionAngle));
        }
        else if (detectionRadius == DetectionRadius.Sphere)
        {
            DetectionType(IsInSphere(transform, player, detectionRange));
        }
    }

    public bool IsPlayerDetected()
    {
        return isPlayerDetected;
    }

    private void DetectionType(bool playerDetected)
    {
        if (playerDetected)
        {
            isPlayerDetected = true;
        }
    }

    bool IsInCone(Transform turret, Transform player, float range, float coneAngle)
    {
        if (player == null) return false;
        Vector3 localPosition = turret.InverseTransformPoint(player.position);
        Vector2 direction = new Vector2(localPosition.x, localPosition.z);
        isPlayerDetected = false;

        if (direction.magnitude > range || localPosition.z < 0f)
        {
            isPlayerDetected = false;
            return isPlayerDetected;
        }

        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        isPlayerDetected = Mathf.Abs(targetAngle) <= coneAngle / 2f;
        return isPlayerDetected;
    }

    bool IsInSphere(Transform turret, Transform player, float range)
    {
        if (player == null) return false;
        Vector3 localPosition = turret.InverseTransformPoint(player.position);
        Vector2 direction = new Vector2(localPosition.x, localPosition.z);
        isPlayerDetected = direction.magnitude <= range;
        return isPlayerDetected;
    }

    void DrawCone()
    {
        lineRenderer.loop = false;
        lineRenderer.positionCount = 4;

        float halfAngle = detectionAngle / 2f * Mathf.Deg2Rad;

        Vector3 left = new Vector3(-Mathf.Sin(halfAngle), 
        0f, Mathf.Cos(halfAngle)) * detectionRange;

        Vector3 right = new Vector3(Mathf.Sin(halfAngle), 
        0f, Mathf.Cos(halfAngle)) * detectionRange;

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, left);
        lineRenderer.SetPosition(2, right);
        lineRenderer.SetPosition(3, Vector3.zero);
    }

    void DrawSphere()
    {
        int segments = 36;
        float angleStep = 360f / segments;

        lineRenderer.loop = false;
        lineRenderer.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * detectionRange;
            lineRenderer.SetPosition(i, point);
        }
    }
}
