using UnityEngine;

public class Sniper : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;
    private float cooldown;
    PlayerDetection playerDetection;

    void Start()
    {
        playerDetection = GetComponent<PlayerDetection>();
        cooldown = fireRate;
    }

    void Update()
    {
        if (playerDetection.IsPlayerDetected())
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
            Instantiate(bulletPrefab, firePoint.position, bulletRotation);
            cooldown = fireRate;
        }
    }
}
