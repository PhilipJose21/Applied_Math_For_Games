using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AreaDamage : MonoBehaviour
{
    private List<Transform> allTargetsInRadius = new List<Transform>();
    private HashSet<Enemy> pendingDeaths = new HashSet<Enemy>();
    private TargetDetection targetDetection;
    [SerializeField] private float deathDelay = 0.5f;

    void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        allTargetsInRadius = targetDetection.GetTargetsInRadius();
        foreach (Transform target in allTargetsInRadius)
        {
            if (target != null)
            {
                if (target.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    if (pendingDeaths.Add(enemy))
                    {
                        StartCoroutine(DelayedDeath(enemy));
                    }
                }
            }
        }
    }

    private IEnumerator DelayedDeath(Enemy enemy)
    {
        yield return new WaitForSeconds(deathDelay);

        if (enemy != null)
        {
            enemy.Death();
        }

        pendingDeaths.Remove(enemy);
    }
}
