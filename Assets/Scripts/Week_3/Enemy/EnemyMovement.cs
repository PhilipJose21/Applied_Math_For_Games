using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> pathPoints = new List<Transform>();
    
    readonly List<PathLogic> pathLogics = new List<PathLogic>();
    Enemy enemy;

    int currentPointIndex = 0;
    bool onCurve = false;
    float t = 0f;                       // progress along the current curve (0 to 1)
    Vector3 p0, p1, p2;                 // current curve points
    float curveLength = 1f;

    void Start()
    {
        enemy = GetComponent<Enemy>();

        GameManager gameManager = GameManager._instance;
        if (gameManager != null)
        {
            pathPoints = gameManager.GetPathPoints();
        }

        // Cache the PathLogic on each node (null = plain straight node)
        pathLogics.Clear();
        foreach (Transform point in pathPoints)
        {
            pathLogics.Add(point.GetComponent<PathLogic>());
        }
    }

    void Update()
    {
        if (pathPoints.Count < 2) return;

        if (onCurve) MoveAlongCurve();
        else MoveStraight();
    }

    void MoveStraight()
    {
        PathLogic logic = pathLogics[currentPointIndex];
        bool isCurveNode = logic != null && logic.IsCurve;

        // Straight node: walk to the node itself.
        // Curve node: walk to where the curve starts.
        Vector3 target = pathPoints[currentPointIndex].position;
        if (isCurveNode)
        {
            BuildCurve(logic);
            target = p0;
        }

        float speed = enemy.GetSpeed();
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if ((target - transform.position).sqrMagnitude < 0.0001f)
        {
            if (isCurveNode)
            {
                onCurve = true;
                t = 0f;
            }
            else
            {
                AdvanceToNextPoint();
            }
        }
    }

    void MoveAlongCurve()
    {
        PathLogic logic = pathLogics[currentPointIndex];

        t += enemy.GetSpeed() * logic.curveSpeedMultiplier * Time.deltaTime / curveLength;
        transform.position = PathLogic.QuadraticBezier(p0, p1, p2, Mathf.Clamp01(t));

        if (t >= 1f)
        {
            onCurve = false;
            AdvanceToNextPoint();
        }
    }

    void BuildCurve(PathLogic logic)
    {
        int count = pathPoints.Count;
        // Clamped (not wrapped) so the first/last nodes don't curve toward the opposite end of the path
        Vector3 prev = pathPoints[Mathf.Max(currentPointIndex - 1, 0)].position;
        Vector3 next = pathPoints[Mathf.Min(currentPointIndex + 1, count - 1)].position;

        logic.GetCurvePoints(prev, next, out p0, out p1, out p2);
        curveLength = PathLogic.ApproximateLength(p0, p1, p2);
    }

    void AdvanceToNextPoint()
    {
        currentPointIndex = (currentPointIndex + 1) % pathPoints.Count; // loops back to the first point
    }
}