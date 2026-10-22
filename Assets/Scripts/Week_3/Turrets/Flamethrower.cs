using UnityEngine;
using System.Collections.Generic;

public class Flamethrower : BaseTower
{
    [SerializeField] private GameObject flameParticles;

    [Header("Flame Upgrades: ")]
    [Tooltip("How many degrees the flame cone widens per upgrade level.")]
    [SerializeField] private float angleUpgradePerLevel = 5f;

    [Header("Defense Debuff: ")]
    [SerializeField] private int debuffLevel = 4;
    [SerializeField] private float debuffDuration = 5f;

    private const float FireRateDivisor = 5f;

    private AreaDamage flameDamage;
    private TargetDetection flameDetection;

    private readonly Dictionary<Enemy, float> originalDefense = new Dictionary<Enemy, float>();
    private readonly Dictionary<Enemy, float> debuffTimers = new Dictionary<Enemy, float>();
    private readonly List<Enemy> expired = new List<Enemy>();

    private AudioSource flameAudioSource;

    private bool wasFiring;

    void Awake()
    {
        flameDamage = flameParticles.GetComponent<AreaDamage>();
        flameDetection = flameParticles.GetComponent<TargetDetection>();

        if (flameAudioSource == null)
            flameAudioSource = gameObject.AddComponent<AudioSource>();
        flameAudioSource.playOnAwake = false;
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

        bool isFiring = targetDetection.IsTargetDetected();
        flameParticles.SetActive(isFiring);
        UpdateFlameAudio(isFiring);

        if (towerLevel >= debuffLevel)
            HandleDefenseDebuff();
    }

    private void UpdateFlameAudio(bool isFiring)
    {
        if (isFiring && !wasFiring)
        {
            flameAudioSource.loop = false;
            flameAudioSource.clip = soundManager.flameThrowerStartClip;
            flameAudioSource.Play();
        }
        else if (isFiring && !flameAudioSource.isPlaying)
        {
            flameAudioSource.loop = true;
            flameAudioSource.clip = soundManager.flameThrowerLoopClip;
            flameAudioSource.Play();
        }
        else if (!isFiring && wasFiring)
        {
            flameAudioSource.Stop();
        }

        wasFiring = isFiring;
    }

    private void ApplyFlameStats()
    {
        flameDamage.SetDamage(damage);
        flameDamage.SetDelayedDamageTime(fireRate / FireRateDivisor);
    }

    private void HandleDefenseDebuff()
    {
        foreach (var target in targetDetection.GetAllTargetsInRadius())
        {
            if (target == null) continue;

            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy == null) continue;

            if (!originalDefense.ContainsKey(enemy))
            {
                originalDefense[enemy] = enemy.GetDefense();
                enemy.SetDefense(0);
            }
            debuffTimers[enemy] = debuffDuration;
        }

        expired.Clear();
        foreach (var enemy in originalDefense.Keys)
        {
            if (enemy == null)
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

        // 3) Remove after the loop
        foreach (var enemy in expired)
        {
            originalDefense.Remove(enemy);
            debuffTimers.Remove(enemy);
        }
    }

    void OnDisable()
    {
        if (flameAudioSource != null) flameAudioSource.Stop();
        wasFiring = false;
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
        base.Upgrade(level);
        if (towerLevel == levelBefore) return;

        towerUpgrades upgrade = upgrades[level - 1];

        flameDetection.setDetectionRange(flameDetection.GetDetectionRange() + upgrade.rangeUPG);
        flameDetection.setDetectionAngle(flameDetection.GetDetectionAngle() + angleUpgradePerLevel);

        ApplyFlameStats();
    }
}