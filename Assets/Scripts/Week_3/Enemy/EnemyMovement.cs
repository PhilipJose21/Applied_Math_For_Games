using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> pathPoints = new List<Transform>();
    int currentPointIndex = 0;
    Vector3 startPosition;
    void Start()
    {
        GameManager gameManager = GameManager._instance;
        if (gameManager != null)
        {
            pathPoints = gameManager.GetPathPoints();
        }
    }

    void Update()
    {
        MoveToPoint();
    }

    void MoveToPoint()
    {
        startPosition = this.transform.position;
        var target = pathPoints[currentPointIndex];
        float speed = GetComponent<Enemy>().GetSpeed();
        var distanceToNextPoint = Vector3.Distance(startPosition, pathPoints[currentPointIndex].position);
        var lerpedTime = Mathf.Clamp01(Time.deltaTime * speed / distanceToNextPoint);

        transform.position = startPosition + (target.position - startPosition) * lerpedTime;    
        // transform.position = startPosition + (pathPoints[currentPointIndex].position - startPosition) * (speed/2) * Time.deltaTime;
        if (distanceToNextPoint < 0.1f)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathPoints.Count)
            {
                currentPointIndex = 0; // Reset to the first point if reached the end
            }
        }
    }
}
