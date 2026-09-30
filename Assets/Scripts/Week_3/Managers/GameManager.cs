using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    

    public bool enableDebug = true;
    private HealthSystem healthSystem;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject finishArea;

    [SerializeField] private List<Transform> pathPoints = new List<Transform>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        healthSystem = GetComponent<HealthSystem>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            TargetDetection.ShowAllShapes = !TargetDetection.ShowAllShapes;
        }

        if (finishArea.GetComponent<TargetDetection>().IsTargetDetected())
        {
            DamagePlayer();
            Destroy(finishArea.GetComponent<TargetDetection>().GetTarget().gameObject);
        }
    }

    public void DamagePlayer()
    {
        healthSystem.Damage(1);
        if (healthSystem.GetHealth() <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void FinishGame()
    {
        winUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public List<Transform> GetPathPoints()
    {
        return pathPoints;
    }

    public EnemyManager GetEnemyManager()
    {
        return EnemyManager._instance;
    }

    void OnDrawGizmos()
    {
        if (pathPoints.Count > 0)
        {
            for (int i = 0; i < pathPoints.Count; i++)
            {
                if (pathPoints[i] != null)
                {
                    Gizmos.color = Color.red;
                    if (i == pathPoints.Count)
                    {
                        Gizmos.DrawSphere(pathPoints[i].position, 0.5f);
                    }
                    else
                    {
                        Gizmos.DrawLine(pathPoints[i].position, pathPoints[(i + 1) % pathPoints.Count].position);
                    }
                }
            }
        }
    }
}
