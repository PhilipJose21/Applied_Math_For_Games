using UnityEngine;
using System;
using System.Collections.Generic;


[ExecuteAlways]
public class TargetDetection : MonoBehaviour
{
    public enum DetectionRadius
    {
        Cone,
        Sphere,
        ObjectSize
    }
    
    
    private List<Transform> allTargets = new List<Transform>();
    private List<Transform> allTargetsInRadius = new List<Transform>();
    private Transform target;
    private RotateTurret rotateTurret;
    private bool isTargetDetected = false;

    [Header("Detection Settings")]
    [SerializeField] private DetectionRadius detectionRadius = DetectionRadius.Cone;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float detectionAngle = 45f;
    

    [Header("Line Renderer Components")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float startWidth = 0.1f;
    [SerializeField] private float endWidth = 0.1f;
    [SerializeField] private Color lineColor = Color.red;

    [Header("Visibility")]
    [SerializeField] private bool showDetectionShape = true;
    public static bool ShowAllShapes = true;

    void Start()
    {
        SetupLineRenderer();
        rotateTurret = GetComponent<RotateTurret>();
        EnemyManager.OnEnemyListUpdated += UpdateEnemyList;
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

    void Update()
    {
        DrawDetectionShape();
        getTargetsInRadius();
        
        if (!Application.isPlaying) return;

        isTargetDetected = allTargetsInRadius.Count > 0;
    }

    private void UpdateEnemyList(object sender, EventArgs e)
    {
        allTargets = EnemyManager._instance.GetAllTargets();
    }
    
    void getTargetsInRadius()
    {
        //remove targets no longer in radius
        for (int i = allTargetsInRadius.Count - 1; i >= 0; i--)
        {
            if (!IsDetected(allTargetsInRadius[i]))
            {
                allTargetsInRadius.RemoveAt(i);
            }
        }

        // adds new targets that enter radius at the end of list
        for (int i = 0; i < allTargets.Count; i++)
        {
            var t = allTargets[i];
            if (IsDetected(t) && !allTargetsInRadius.Contains(t))
            {
                allTargetsInRadius.Add(t);
            }
        }
    }

    bool IsDetected(Transform target)
    {
        if (target==null) return false;
        switch (detectionRadius)
        {
            case DetectionRadius.Cone:
                return IsInCone(transform, target, detectionRange, detectionAngle);
            case DetectionRadius.Sphere:
                return IsInSphere(transform, target, detectionRange);
            case DetectionRadius.ObjectSize:
                return IsInObjectSize(transform, target);
            default:
                return false;
        }
    }


    public List<Transform> GetTargetsInRadius()
    {
        return allTargetsInRadius;
    }

    public Transform GetTarget()
    {
        if (allTargetsInRadius.Count > 0)
        {
            return allTargetsInRadius[0];
        }
        else
        {
            return null;
        }
    }

    public bool IsTargetDetected()
    {
        return isTargetDetected;
    }

    public float GetDetectionAngle()
    {
        return detectionAngle;
    }

    public float GetDetectionRange()
    {
        return detectionRange;
    }

    private void DetectionType(bool targetDetected)
    {
        isTargetDetected = targetDetected;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    bool IsInObjectSize(Transform turret, Transform target)
    {
        if (target == null) return false;//checks if player is in the scene

        float scale = 1.5f;
        float range = this.transform.localScale.x / scale; //makes the range of the detection area the size of the object
        float distance = Vector3.Distance(target.position, transform.position); //gets distance

        isTargetDetected = distance <= range; 
        return isTargetDetected;
    }

    bool IsInCone(Transform turret, Transform target, float range, float coneAngle)
    {
        if (target == null) return false;

        Vector3 local = turret.InverseTransformPoint(target.position);
        Vector2 dir = new Vector2(local.x, local.z);

        if (dir.magnitude > range || local.z < 0f) return false;

        float targetAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        return Mathf.Abs(targetAngle) <= coneAngle / 2f;
    }

    bool IsInSphere(Transform turret, Transform target, float range)
    {
        if (target == null) return false;//checks if player is in the scene

        //checks if the player is within the range of the turret
        Vector3 localPosition = turret.InverseTransformPoint(target.position);
        Vector2 direction = new Vector2(localPosition.x, localPosition.z);
        isTargetDetected = direction.magnitude <= range;
        return isTargetDetected;
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

    void SetupLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
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
        if (lineRenderer == null) return;

        // Edit mode: only the checkbox matters. Play mode: checkbox AND global toggle.
        bool visible = showDetectionShape && (!Application.isPlaying || ShowAllShapes);
        lineRenderer.enabled = visible;
        if (!visible) return;

        if (detectionRadius == DetectionRadius.Cone)
        {
            DrawCone();
        }
        else if (detectionRadius == DetectionRadius.Sphere)
        {
            DrawSphere();
        }
    }

    public void SetShapeVisible(bool value)
    {
        showDetectionShape = value;
        DrawDetectionShape();
    }

}
