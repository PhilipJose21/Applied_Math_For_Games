using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyManager : MonoBehaviour
{
    public static EventHandler OnEnemyListUpdated;
    public static EventHandler OnWaveEnded;
    public static EnemyManager _instance;

    [SerializeField] private float spawnRate = 1f;

    [Header("Enemy List")]
    [SerializeField] private List<EnemyWaveSO> waveList = new List<EnemyWaveSO>();
    [SerializeField] private List<Transform> allTargets = new List<Transform>();

    private List<Transform> spawnEnemyList = new List<Transform>();
    private Transform spawnPoint;
    private int currentWaveIndex = 0;
    private int spawnEnemyIndex = 0;
    private float cooldown;
    private bool waveStarted = false;
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }   

        StartWave();
        spawnPoint = GameManager._instance.GetPathPoints()[0];
    }

    void OnEnable()
    {
        RefreshTargets();
    }

    void OnValidate()
    {
        RefreshTargets();
    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemy();
        CheckTargetList();
        
        if (!waveStarted && allTargets.Count == 0 && currentWaveIndex < waveList.Count)
        {
            StartWave();
        }
        if (currentWaveIndex >= waveList.Count && allTargets.Count == 0)
        {
            Debug.Log("All waves completed");
            OnWaveEnded?.Invoke(this, EventArgs.Empty);
        }
    }

    void StartWave()
    {
        if (currentWaveIndex >= waveList.Count)
        {
            waveStarted = false;
            return;
        }

        spawnEnemyList = waveList[currentWaveIndex].enemyPrefabs;
        spawnEnemyIndex = 0;
        waveStarted = true;
        Debug.Log("Wave Started");
    }

    void SpawnEnemy()
    {
        if (waveStarted == false) return;
        if (spawnEnemyList.Count == 0) return;
        cooldown -= Time.deltaTime;
        if (cooldown > 0f) return;

        Transform enemyPrefab = spawnEnemyList[spawnEnemyIndex];
        if (enemyPrefab == null || spawnPoint == null)
        {
            waveStarted = false;
            return;
        }

        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);


        RefreshTargets();
        spawnEnemyIndex++;
        cooldown = spawnRate;

        if (spawnEnemyIndex >= spawnEnemyList.Count)
        {
            waveStarted = false;
            currentWaveIndex++;
        }
    }

    void CheckTargetList()
    {
        for (int i = allTargets.Count - 1; i >= 0; i--)
        {
            if (allTargets[i] == null)
            {
                allTargets.RemoveAt(i);
                OnEnemyListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    void RefreshTargets()
    {
        // remove destroyed enemies first so the list doesn't fill up with nulls
        for (int i = allTargets.Count - 1; i >= 0; i--)
        {
            if (allTargets[i] == null)
            {
                allTargets.RemoveAt(i);
                OnEnemyListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        // add any new enemies
        GameObject[] found = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < found.Length; i++)
        {
            Transform t = found[i].transform;
            if (!allTargets.Contains(t))
            {
                allTargets.Add(t);
                OnEnemyListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void RemoveTarget(Transform target)
    {
        if (allTargets.Contains(target))
        {
            allTargets.Remove(target);
            
            OnEnemyListUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public List<Transform> GetAllTargets()
    {
        return allTargets;
    }

    public List<EnemyWaveSO> GetWaveList()
    {
        return waveList;
    }

    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }


}
