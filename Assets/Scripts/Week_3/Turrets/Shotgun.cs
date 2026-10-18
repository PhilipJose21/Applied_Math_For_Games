using UnityEngine;

public class Shotgun : BaseTower
{
    [SerializeField] private int numberOfBullets = 5;
    [SerializeField] private float spreadAngle = 15f;
    private float cooldown;

    public override void Fire()
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
        bullet.GetComponent<Projectile>().SetDamage(damage);
        bullet.GetComponent<Projectile>().SetHoming(false);
    }

    public override void Upgrade(int level)
    {
        switch (level)
        {
            
        }
    }
}
