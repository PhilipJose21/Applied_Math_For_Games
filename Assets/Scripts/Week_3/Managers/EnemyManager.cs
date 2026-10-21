using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyManager : MonoBehaviour
{
    public static EventHandler OnEnemyListUpdated;
    public static EventHandler OnWaveEnded;
    public static EnemyManager _instance;

    [SerializeField] private float spawnRate = 1f;

    [Header("Enemy Scaling")]
    [Tooltip("Health goes up after every Nth completed wave (10, 20, 30...).")]
    [SerializeField] private int healthIncreaseEveryXWaves = 10;
    [SerializeField, Range(0f, 1f)] private float healthIncreasePercent = 0.10f;
    [Tooltip("Defense goes up after every Nth completed wave, except waves that also raise health.")]
    [SerializeField] private int defenseIncreaseEveryXWaves = 5;
    [SerializeField, Range(0f, 1f)] private float defenseIncreasePercent = 0.05f;
    [Tooltip("Enemy cash reward goes up by this much every time health or defense is raised.")]
    [SerializeField, Range(0f, 2f)] private float rewardIncreasePercent = 0.5f;

    [Header("Wave Completion Reward")]
    [SerializeField] private int baseWaveReward = 200;
    [Tooltip("Added to the base reward after every full block of waves.")]
    [SerializeField] private int rewardIncreasePerBlock = 100;
    [SerializeField] private int wavesPerRewardBlock = 5;

    [Header("Enemy List")]
    [SerializeField] private List<EnemyWaveSO> waveList = new List<EnemyWaveSO>();
    [SerializeField] private List<Transform> allTargets = new List<Transform>();

    private List<Transform> spawnEnemyList = new List<Transform>();
    private Transform spawnPoint;
    private int currentWaveIndex = 0;
    private int currentWaveNumber = 0;   // 1-based number of the wave that is running (0 = none started yet)
    private int spawnEnemyIndex = 0;
    private float cooldown;
    private bool waveStarted = false;    // true while enemies are still being spawned
    private bool waveInProgress = false; // true from StartWave until the last enemy of the wave is dead
    private bool gameWon = false;

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

    void Update()
    {
        SpawnEnemy();
        CheckTargetList();
        CheckWaveCompleted();

        if (!gameWon && currentWaveIndex >= waveList.Count && allTargets.Count == 0)
        {
            gameWon = true;
            Debug.Log("All waves completed");
            OnWaveEnded?.Invoke(this, EventArgs.Empty);
        }
    }

    public void StartWave()
    {
        if (waveInProgress) return;

        if (currentWaveIndex >= waveList.Count)
        {
            waveStarted = false;
            return;
        }

        if (allTargets.Count > 0)
        {
            Debug.Log("Cannot start wave, enemies still present");
            return;
        }

        spawnEnemyList = waveList[currentWaveIndex].enemyPrefabs;
        spawnEnemyIndex = 0;
        currentWaveNumber = currentWaveIndex + 1;
        waveStarted = true;
        waveInProgress = true;
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

        Transform spawned = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Scale the new enemy for the wave it spawns in (before its Start() runs)
        if (spawned.TryGetComponent<Enemy>(out Enemy enemy))
        {
            GetScaling(currentWaveNumber - 1, out float healthMult, out float defenseMult, out float rewardMult);
            enemy.ApplyWaveScaling(healthMult, defenseMult, rewardMult);
        }

        RefreshTargets();
        spawnEnemyIndex++;
        cooldown = spawnRate;

        if (spawnEnemyIndex >= spawnEnemyList.Count)
        {
            waveStarted = false;
            currentWaveIndex++;
        }
    }

    // wavesCompleted = how many waves were finished before this one (wave 11 -> 10)
    void GetScaling(int wavesCompleted, out float healthMult, out float defenseMult, out float rewardMult)
    {
        int healthInterval = Mathf.Max(1, healthIncreaseEveryXWaves);
        int defenseInterval = Mathf.Max(1, defenseIncreaseEveryXWaves);

        int healthSteps = wavesCompleted / healthInterval;

        // Defense also steps every Nth wave, but skips any wave where health already stepped
        int defenseSteps = 0;
        for (int wave = defenseInterval; wave <= wavesCompleted; wave += defenseInterval)
        {
            if (wave % healthInterval != 0)
                defenseSteps++;
        }

        healthMult = Mathf.Pow(1f + healthIncreasePercent, healthSteps);
        defenseMult = Mathf.Pow(1f + defenseIncreasePercent, defenseSteps);
        rewardMult = Mathf.Pow(1f + rewardIncreasePercent, healthSteps + defenseSteps);
    }

    int GetWaveReward(int waveNumber)
    {
        int blockSize = Mathf.Max(1, wavesPerRewardBlock);
        int block = (waveNumber - 1) / blockSize;          // 0 for waves 1-5, 1 for 6-10, ...
        int wavesCompletedInBlock = (waveNumber - 1) % blockSize + 1;   // 1..5
        return (baseWaveReward + rewardIncreasePerBlock * block) * wavesCompletedInBlock;
    }

    void CheckWaveCompleted()
    {
        if (!waveInProgress || waveStarted || allTargets.Count > 0) return;

        waveInProgress = false;

        int reward = GetWaveReward(currentWaveNumber);
        GameManager._instance.AddCoin(reward);
        Debug.Log($"Wave {currentWaveNumber} complete: +${reward}");
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

    // 1-based number of the wave that is running (or the last one that ran)
    public int GetCurrentWaveNumber()
    {
        return currentWaveNumber;
    }
}