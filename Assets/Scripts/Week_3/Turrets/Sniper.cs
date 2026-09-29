using UnityEngine;

public class Sniper : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 5f;
    private float cooldown;
    private TargetDetection targetDetection;

    void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
        cooldown = fireRate;
    }

    void Update()
    {
        if (targetDetection.IsTargetDetected())
        {
            Fire();
        }
    }

    void Fire()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
        {
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(90f, 0f, 0f);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
            bullet.GetComponent<Projectile>().SetSpeed(bulletSpeed);
            bullet.GetComponent<Projectile>().SetLifetime(bulletLifetime);
            cooldown = fireRate;
        }
    }
}
