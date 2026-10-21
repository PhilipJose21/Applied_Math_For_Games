using System.Collections;
using UnityEngine;

public class Burster : BaseTower
{
    [SerializeField] private float burstDelay = 0.1f;   // seconds between bullets in a burst
    [SerializeField] private int bulletCount = 3;

    private bool isBursting;



    public override void Fire()
    {
        if (isBursting) return; 

        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
            StartCoroutine(FireBurst());
    }

    private IEnumerator FireBurst()
    {
        isBursting = true;

        switch(towerLevel)
        {
            case 1:
                bulletCount = 3;
                break;
            case 2:
                bulletCount = 4;
                break;
            case 3:
                bulletCount = 8;
                break;
            default:
                bulletCount = 12;
                break;
        }


        for (int i = 0; i < bulletCount; i++)
        {
            Transform target = targetDetection.GetTarget();
            if (target == null) break;

            SpawnBullet(target);

            if (i < bulletCount - 1)
                yield return new WaitForSeconds(burstDelay);
        }

        cooldown = fireRate;
        isBursting = false;
    }

    private void SpawnBullet(Transform target)
    {
        Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(90f, 0f, 0f);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        var b = bullet.GetComponent<Projectile>();
        b.SetSpeed(bulletSpeed);
        b.SetLifetime(bulletLifetime);
        b.SetTarget(target);
        b.SetDamage(damage);
        b.SetPierce(hasPierce);

        Debug.Log("Bullet Created");
    }

    // Coroutines stop when the object is disabled, so make sure the flag doesn't stay stuck on
    void OnDisable()
    {
        isBursting = false;
    }
}