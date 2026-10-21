using UnityEngine;
using System.Collections.Generic;

public class AreaDamage : MonoBehaviour
{
    private int damage = 1;
    private List<Transform> allTargetsInRadius = new List<Transform>();

    private float timer;
    private TargetDetection targetDetection;
    private bool hasDamaged;

    [SerializeField] private float delayedDamageTime = 0.16f;
    [SerializeField] private bool continueDamage = true;

    void Awake()
    {
        targetDetection = GetComponent<TargetDetection>();
    }

    // Runs every time the flame is turned on (SetActive(true)), not just the first time
    void OnEnable()
    {
        timer = delayedDamageTime;
        hasDamaged = false;
    }

    void Update()
    {
        if (hasDamaged || targetDetection == null)
            return;

        timer -= Time.deltaTime;
        if (timer > 0f)
            return;

        allTargetsInRadius = targetDetection.GetTargetsInRadius();
        Damage();

        if (continueDamage)
            timer += delayedDamageTime;
        else
            hasDamaged = true;
    }

    public void Damage()
    {
        foreach (Transform target in allTargetsInRadius)
        {
            if (target != null && target.TryGetComponent<Enemy>(out Enemy enemy))
                enemy.TakeDamage(damage);
        }
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetDelayedDamageTime(float time)
    {
        delayedDamageTime = time;
    }
}