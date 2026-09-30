using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> pathPoints = new List<Transform>();
    void Start()
    {
        GameManager gameManager = GameManager._instance;
        pathPoints = gameManager.GetPathPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
