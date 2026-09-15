using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RocketBarrage : MonoBehaviour
{
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int rocketOffset = 45;
    [SerializeField] private int maxRockets = 8;
    [SerializeField] private int rocketCount = 4;
    private bool isFinished = false;

    // Update is called once per frame
    void Update()
    {
        if (!isFinished)
        {
            StartCoroutine(SpawnRockets());
        }
        if (rocketCount > maxRockets)
        {
            rocketCount = maxRockets;
        }
    }

    IEnumerator SpawnRockets()
    {
        for (int i = 0; i < rocketCount; i++)
        {
            isFinished = true;   
            Instantiate(rocketPrefab, spawnPoint.position, rocketAngleCalc(i));
            yield return new WaitForSeconds(spawnInterval);
        }
        
        isFinished = false;
    }

    private Quaternion rocketAngleCalc(int i) //Calculates the angle for each rocket based on the number of rockets and the offset
    {
        return Quaternion.Euler(0f, i * rocketSpacing() + rocketOffset, 0f);
    }

    private int rocketSpacing() //Calculates the spacing between each rocket
    {
        return 360/rocketCount;
    }

    public void IncreaseRocketCount()
    {
        if (rocketCount < maxRockets) // Limit the maximum number of rockets to maxRockets
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
            Vector3 direction = rocketAngleCalc(i) * Vector3.forward;
            Gizmos.DrawRay(spawnPoint.position, direction * 5f);
        }
    }
}
