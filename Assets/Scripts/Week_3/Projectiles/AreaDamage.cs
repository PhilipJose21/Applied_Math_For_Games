using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AreaDamage : MonoBehaviour
{
    private float damage = 1f;
    private List<Transform> allTargetsInRadius = new List<Transform>();
    
    private float timer;
    private TargetDetection targetDetection;
    private bool hasDamaged;

    [SerializeField] private float delayedDamageTime = 0.1f;
    [SerializeField] private bool continueDamage = false;

    void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
        timer = delayedDamageTime;
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
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            allTargetsInRadius = targetDetection.GetTargetsInRadius();
            Damage();
            if (continueDamage)
            {
                timer = delayedDamageTime;
            }
            else
            {
                hasDamaged = true;
            }
        }
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
                    Debug.Log("damage: " + damage); 
                }
            }
        }
    }
    

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}
