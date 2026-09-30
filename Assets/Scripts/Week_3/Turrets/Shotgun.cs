using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int numberOfBullets = 5;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifetime = .15f;
    private float cooldown;
    TargetDetection targetDetection;

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
            for (int i = 0; i < numberOfBullets; i++)
            {
                CreateBullet(new Vector3(0f, Random.Range(-spreadAngle, spreadAngle), 0f));
            }
            cooldown = fireRate;
        }
    }

    void CreateBullet(Vector3 offsetRotation)
    {
        Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(90f, offsetRotation.y, 0f);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        bullet.GetComponent<Projectile>().SetSpeed(bulletSpeed);
        bullet.GetComponent<Projectile>().SetLifetime(bulletLifetime);
        bullet.GetComponent<Projectile>().SetTarget(targetDetection.GetTarget());
    }
}
