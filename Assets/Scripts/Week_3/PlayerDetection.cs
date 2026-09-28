using UnityEngine;
using System;



[ExecuteAlways]
public class PlayerDetection : MonoBehaviour
{
    public enum DetectionRadius
    {
        Cone,
        Sphere,
        ObjectSize
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
        SetupLineRenderer();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rotateTurret = GetComponent<RotateTurret>();
        damagePlayer = GetComponent<DamagePlayer>();
    }

    //TO MAKE THE LINERENDERER VISIBLE IN EDITOR
    void OnEnable()
    {
        SetupLineRenderer();
    }

    void OnValidate()
    {
        SetupLineRenderer();
        DrawDetectionShape();
    }

    // Update is called once per frame
    void Update()
    {
        DrawDetectionShape();

        if (!Application.isPlaying)
        {
            return;
        }

        if (detectionRadius == DetectionRadius.Cone)
        {
            DetectionType(IsInCone(transform, player, detectionRange, detectionAngle));
        }
        else if (detectionRadius == DetectionRadius.Sphere)
        {
            DetectionType(IsInSphere(transform, player, detectionRange));
        }
        else if (detectionRadius == DetectionRadius.ObjectSize)
        {
            DetectionType(IsInObjectSize(transform, player));
        }
    }

    void SetupLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = false;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    void DrawDetectionShape()
    {
        if (lineRenderer == null)
        {
            return;
        }

        if (detectionRadius == DetectionRadius.Cone)
        {
            DrawCone();
        }
        else if (detectionRadius == DetectionRadius.Sphere)
        {
            DrawSphere();
        }
    }

    public bool IsPlayerDetected()
    {
        return isPlayerDetected;
    }

    public float GetDetectionAngle()
    {
        return detectionAngle;
    }

    public float GetDetectionRange()
    {
        return detectionRange;
    }

    private void DetectionType(bool playerDetected)
    {
        isPlayerDetected = playerDetected;
    }

    bool IsInObjectSize(Transform turret, Transform player)
    {
        if (player == null) return false;//checks if player is in the scene

        float scale = 1.5f;
        float range = this.transform.localScale.x / scale; //makes the range of the detection area the size of the object
        float distance = Vector3.Distance(player.position, transform.position); //gets distance

        isPlayerDetected = distance <= range; 
        return isPlayerDetected;
    }

    bool IsInCone(Transform turret, Transform player, float range, float coneAngle)
    {
        if (player == null) return false;//checks if player is in the scene

        //Calculate local pos of player to the turret
        Vector3 localPosition = turret.InverseTransformPoint(player.position);
        Vector2 direction = new Vector2(localPosition.x, localPosition.z);
        isPlayerDetected = false;

        //check if player is in the range and if the player is in front of the turret
        if (direction.magnitude > range || localPosition.z < 0f)
        {
            isPlayerDetected = false;
            return isPlayerDetected;
        }

        //how far the player is off to the side from the turrets forward direction
        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        isPlayerDetected = Mathf.Abs(targetAngle) <= coneAngle / 2f;
        return isPlayerDetected;
    }

    bool IsInSphere(Transform turret, Transform player, float range)
    {
        if (player == null) return false;//checks if player is in the scene

        //checks if the player is within the range of the turret
        Vector3 localPosition = turret.InverseTransformPoint(player.position);
        Vector2 direction = new Vector2(localPosition.x, localPosition.z);
        isPlayerDetected = direction.magnitude <= range;
        return isPlayerDetected;
    }

    void DrawCone()
    {
        int segments = 20;
        lineRenderer.loop = false;
        lineRenderer.positionCount = segments + 3;

        float halfAngle = detectionAngle / 2f;
        lineRenderer.SetPosition(0, Vector3.zero);

        for (int i = 0; i <= segments; i++)
        {
            float a = Mathf.Lerp(-halfAngle, halfAngle, i / (float)segments) * Mathf.Deg2Rad;
            Vector3 p = new Vector3(Mathf.Sin(a), 0f, Mathf.Cos(a)) * detectionRange;
            lineRenderer.SetPosition(i + 1, p);
        }

        lineRenderer.SetPosition(segments + 2, Vector3.zero);
    }

    void DrawSphere()
    {
        //Sets up variables for drawing the linerenderer circle
        int segments = 36;
        float angleStep = 360f / segments;

        //changes components of the linerenderer to draw a circle
        lineRenderer.loop = false;
        lineRenderer.positionCount = segments + 1;

        //Draw a circle in the XZ plane
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * detectionRange;
            lineRenderer.SetPosition(i, point);
        }
    }
}
