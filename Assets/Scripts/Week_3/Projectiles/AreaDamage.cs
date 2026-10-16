using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AreaDamage : MonoBehaviour
{
    private float damage = 1f;
    private List<Transform> allTargetsInRadius = new List<Transform>();
    private TargetDetection targetDetection;
    private bool hasDamaged;

    void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasDamaged)
        {
            return;
        }

        if (targetDetection == null)
        {
            Debug.Log("1");
            return;
        }

        allTargetsInRadius = targetDetection.GetTargetsInRadius();
        Damage();
        hasDamaged = true;
    }

    public void Damage()
    {
        foreach (Transform target in allTargetsInRadius)
        {
            if (target != null)
            {
                if (target.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                    Debug.Log("Damaged" + damage);
                }
            }
        }
    }
    

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}
