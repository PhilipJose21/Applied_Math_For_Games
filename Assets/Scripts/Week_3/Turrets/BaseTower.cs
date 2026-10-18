using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firePoint;

    [Header("Tower Stats: ")]
    [SerializeField] protected float level = 0;
    [SerializeField] protected int cost = 100;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected float bulletSpeed = 10f;
    [SerializeField] protected float bulletLifetime = 5f;
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected bool hasHiddenDetection = false;
    [SerializeField] protected bool hasPierce = false;

    [Header("Tower Upgrades: ")]
    [SerializeField] protected towerUpgrades[] upgrades;

    [System.Serializable] public struct toggleFloat
    {
        public float value;
        public bool isToggled;
    }

    [System.Serializable] public class towerUpgrades
    {
        public toggleFloat fireRateUPG;
        public toggleFloat rangeUPG;
        public toggleFloat damageUPG;
        public bool hiddenDetectionUPG;
        public bool pierceUPG;
    }

    private float cooldown;
    protected TargetDetection targetDetection;

    public virtual void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
        cooldown = fireRate;
    }

    public virtual void Update()
    {
        if (targetDetection.IsTargetDetected())
        {
            Fire();
        }
    }

    public virtual void Fire()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
        {
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(90f, 0f, 0f);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
            bullet.GetComponent<Projectile>().SetSpeed(bulletSpeed);
            bullet.GetComponent<Projectile>().SetLifetime(bulletLifetime);
            bullet.GetComponent<Projectile>().SetTarget(targetDetection.GetTarget());
            bullet.GetComponent<Projectile>().SetDamage(damage);
            cooldown = fireRate;
        }
    }

    public virtual void Upgrade(int level)
    {
        switch (level)
        {
            
                
        }
    }

    public virtual int GetCost()
    {
        return cost;
    }
}
