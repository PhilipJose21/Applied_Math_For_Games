using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public static EventHandler OnCoinChanged;
    public static EventHandler OnSelectedTowerChanged;
    public static EventHandler OnTowerUpgraded;

    public bool enableDebug = true;
    private HealthSystem healthSystem;
    private TargetDetection finishAreaDetection;

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject finishArea;
    [SerializeField] private GameObject selectedTower;
    [SerializeField] private int coins;

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
        finishAreaDetection = finishArea.GetComponent<TargetDetection>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            TargetDetection.ShowAllShapes = !TargetDetection.ShowAllShapes;
        }

        if (finishAreaDetection.IsTargetDetected())
        {
            var target = finishAreaDetection.GetTarget();
            if (target != null)
            {if (target.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    healthSystem.Damage(enemy.GetCurrentHealth());
                    enemy.Death();
                }
            }
        }
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public List<Transform> GetPathPoints() => pathPoints;

    public EnemyManager GetEnemyManager() => EnemyManager._instance;

    public int GetCoinCount() => coins;

    public int GetCurrentWave() => EnemyManager._instance != null ? Mathf.Max(1, EnemyManager._instance.GetCurrentWaveNumber()) : 1;

    public GameObject GetSelectedTower() => selectedTower;

    public void AddCoin(int amount)
    {
        coins += amount;
        OnCoinChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveCoin(int amount)
    {
        coins -= amount;
        OnCoinChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedTower(GameObject tower)
    {
        selectedTower = tower;
        OnSelectedTowerChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UpgradeSelectedTower()
    {
        if (selectedTower != null)
        {
            var tower = selectedTower.GetComponent<BaseTower>();
            if (tower != null)
            {
                if (tower.GetUpgrade() == null)
                    return;

                if (coins >= tower.GetUpgradeCost())
                {
                    RemoveCoin(tower.GetUpgradeCost());
                    tower.Upgrade(tower.GetLevel() + 1);
                    OnTowerUpgraded?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    public void SellTower()
    {
        if (selectedTower != null)
        {
            var tower = selectedTower.GetComponent<BaseTower>();
            if (tower != null)
            {
                AddCoin(tower.GetSellValue());
                Destroy(selectedTower);
                SetSelectedTower(null);
            }
        }
    }

    public void CloseUpgradeUI()
    {
        SetSelectedTower(null);
    }

    void OnDrawGizmos()
    {
        // if (pathPoints.Count > 0)
        // {
        //     for (int i = 0; i < pathPoints.Count - 1; i++)
        //     {
        //         if (pathPoints[i] != null && pathPoints[i + 1] != null)
        //         {
        //             Gizmos.color = Color.red;
        //             Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
        //         }
        //     }
        // }
    }
}