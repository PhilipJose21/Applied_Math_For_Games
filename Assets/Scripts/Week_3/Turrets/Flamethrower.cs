using UnityEngine;
using System.Collections.Generic;

public class Flamethrower : BaseTower
{
    [SerializeField] private GameObject flameParticles;

    [Header("Flame Upgrades: ")]
    [Tooltip("How many degrees the flame cone widens per upgrade level.")]
    [SerializeField] private float angleUpgradePerLevel = 5f;

    [Header("Defense Debuff: ")]
    [SerializeField] private int debuffLevel = 4;// minimum tower level to apply the debuff
    [SerializeField] private float debuffDuration = 5f;// seconds (refreshes while enemy stays in range)

    private const float FireRateDivisor = 5f;

    private AreaDamage flameDamage;
    private TargetDetection flameDetection;

    // Enemy -> defense it had before we zeroed it
    private readonly Dictionary<Enemy, int> originalDefense = new Dictionary<Enemy, int>();
    // Enemy -> seconds left on the debuff
    private readonly Dictionary<Enemy, float> debuffTimers = new Dictionary<Enemy, float>();
    private readonly List<Enemy> expired = new List<Enemy>();

    void Awake()
    {
        flameDamage = flameParticles.GetComponent<AreaDamage>();
        flameDetection = flameParticles.GetComponent<TargetDetection>();
    }

    public override void Start()
    {
        base.Start();
        ApplyFlameStats();
        flameParticles.SetActive(false);
    }

    public override void Update()
    {
        HandleSelectionInput();

        flameParticles.SetActive(targetDetection.IsTargetDetected());

        if (towerLevel >= debuffLevel)
            HandleDefenseDebuff();
    }

    private void ApplyFlameStats()
    {
        flameDamage.SetDamage(damage);
        flameDamage.SetDelayedDamageTime(fireRate / FireRateDivisor);
    }

    private void HandleDefenseDebuff()
    {
        //Apply / refresh the debuff on everything currently in range
        foreach (var target in targetDetection.GetAllTargetsInRadius())
        {
            if (target == null) continue;

            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy == null) continue;

            if (!originalDefense.ContainsKey(enemy))
            {
                originalDefense[enemy] = enemy.GetDefense(); //remember the real value once
                enemy.SetDefense(0);
            }
            debuffTimers[enemy] = debuffDuration;
        }

        //Tick timers and collect expired ones (never remove while enumerating)
        expired.Clear();
        foreach (var enemy in originalDefense.Keys)
        {
            if (enemy == null)// enemy died / was destroyed
            {
                expired.Add(enemy);
                continue;
            }

            debuffTimers[enemy] -= Time.deltaTime;
            if (debuffTimers[enemy] <= 0f)
            {
                enemy.SetDefense(originalDefense[enemy]);
                expired.Add(enemy);
            }
        }

        //Remove after the loop
        foreach (var enemy in expired)
        {
            originalDefense.Remove(enemy);
            debuffTimers.Remove(enemy);
        }
    }

    // If the tower is sold/disabled, give everyone their defense back
    void OnDisable()
    {
        foreach (var pair in originalDefense)
        {
            if (pair.Key != null)
                pair.Key.SetDefense(pair.Value);
        }
        originalDefense.Clear();
        debuffTimers.Clear();
    }

    public override void Upgrade(int level)
    {
        int levelBefore = towerLevel;
        base.Upgrade(level);// updates damage, fireRate, tower range, towerLevel
        if (towerLevel == levelBefore) return;// upgrade was rejected (max level etc.)

        towerUpgrades upgrade = upgrades[level - 1];

        flameDetection.setDetectionRange(flameDetection.GetDetectionRange() + upgrade.rangeUPG);
        flameDetection.setDetectionAngle(flameDetection.GetDetectionAngle() + angleUpgradePerLevel);

        ApplyFlameStats();// uses the TOTAL damage, not the upgrade delta
    }
}