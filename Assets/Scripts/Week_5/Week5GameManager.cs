using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class Week5GameManager : MonoBehaviour
{
    [SerializeField] private Transform missilePrefab;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private int difficultyScale = 10;
    [SerializeField] private int missilesPerSpawn = 1;
    private List<Transform> spawnPoints = new List<Transform>();

    public GameObject gameoverUI;

    private void Awake()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        foreach (Transform child in player)
        {
            spawnPoints.Add(child);
        }
        gameoverUI.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(SpawnMissiles());
        StartCoroutine(ScaleDifficulty());
    }

    public void GameOver()
    {
        gameoverUI.SetActive(true);
    }

    IEnumerator SpawnMissiles()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);
            for (int i = 0; i < missilesPerSpawn; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                Instantiate(missilePrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
        
    }

    IEnumerator ScaleDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyScale);
            spawnRate = Mathf.Max(0.5f, spawnRate - 0.1f);
            missilesPerSpawn++;
        }
    }

}
