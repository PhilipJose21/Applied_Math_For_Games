using UnityEngine;

public class Shotgun : BaseTower
{
    [SerializeField] private int numberOfBullets = 5;
    [SerializeField] private float spreadAngle = 15f;

    public override void Fire()
    {
        switch (towerLevel)
        {
            case 1:
                numberOfBullets = 6;
                break;
            case 3:
                numberOfBullets = 10;
                break;
            case 4:
                numberOfBullets = 15;
                break;
        }

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

        if (muzzleFlashRoutine != null) StopCoroutine(muzzleFlashRoutine);
                muzzleFlashRoutine = StartCoroutine(ShowMuzzleFlash());
        
        soundManager.PlaySFX(soundManager.towerGunfireClip);

        bullet.GetComponent<Projectile>().SetSpeed(bulletSpeed);
        bullet.GetComponent<Projectile>().SetLifetime(bulletLifetime);
        bullet.GetComponent<Projectile>().SetDamage(damage);
        bullet.GetComponent<Projectile>().SetHoming(false);
    }
}
