using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RocketBarrage : MonoBehaviour
{
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int rocketOffset = 45;
    [SerializeField] private int rocketCount = 4;
    private bool isFinished = false;

    // Update is called once per frame
    void Update()
    {
        if (!isFinished)
        {
            StartCoroutine(SpawnRockets());
        }
    }

    IEnumerator SpawnRockets()
    {
        for (int i = 0; i < rocketCount; i++)
        {
            isFinished = true;   
            Instantiate(rocketPrefab, spawnPoint.position, Quaternion.Euler(0f, i * rocketSpacing() + rocketOffset, 0f));
            yield return new WaitForSeconds(spawnInterval);
        }
        
        isFinished = false;
    }

    private int rocketSpacing()
    {
        return 360/rocketCount;
    }

    public void IncreaseRocketCount()
    {
        if (rocketCount < 8) // Limit the maximum number of rockets to 8
        {
            rocketCount++;
        }
        else
        {
            Debug.Log("Maximum rocket count reached.");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < rocketCount; i++)
        {
            Vector3 direction = Quaternion.Euler(0f, i * rocketSpacing() + rocketOffset, 0f) * Vector3.forward;
            Gizmos.DrawRay(spawnPoint.position, direction * 5f);
        }
    }
}
