using UnityEngine;
using System.Collections.Generic;

public class AreaDamage : MonoBehaviour
{
    private int damage = 1;
    private List<Transform> allTargetsInRadius = new List<Transform>();

    private float timer;
    private TargetDetection targetDetection;

    [SerializeField] private float delayedDamageTime = 0.16f;

    [Tooltip("Temporary: prints what the flame is doing so we can see where it breaks.")]
    [SerializeField] private bool debugLogs = true;

    void Awake()
    {
        targetDetection = GetComponent<TargetDetection>();
        if (targetDetection == null)
            Debug.LogError("AreaDamage: no TargetDetection on " + name, this);
    }

    void OnEnable()
    {
        timer = delayedDamageTime;
    }

    void Update()
    {
        if (targetDetection == null) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer += Mathf.Max(delayedDamageTime, 0.01f);   // always keeps ticking while the flame is on

        allTargetsInRadius = targetDetection.GetTargetsInRadius();
        if (debugLogs)
            Debug.Log($"AreaDamage tick: {(allTargetsInRadius == null ? -1 : allTargetsInRadius.Count)} targets, damage {damage}", this);

        Damage();
    }

    public void Damage()
    {
        if (allTargetsInRadius == null) return;

        foreach (Transform target in allTargetsInRadius)
        {
            if (target == null) continue;

            Enemy enemy = target.GetComponentInParent<Enemy>();   // works even if the target is a child collider
            if (enemy == null)
            {
                if (debugLogs) Debug.LogWarning("AreaDamage: no Enemy component on " + target.name, target);
                continue;
            }
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