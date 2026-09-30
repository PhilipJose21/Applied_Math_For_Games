using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private float lifetime;
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange = 1f;

    float timer;
    private void Start()
    {
        timer = lifetime;
    }

    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        detectPlayer();
        destroyProjectile();
    }

    void detectPlayer()
    {
        if (target != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer <= detectionRange)
            {
                GameManager._instance.GameOver();
                target.gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }

    void destroyProjectile()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetLifetime(float newLifetime)
    {
        lifetime = newLifetime;
        timer = lifetime;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }


}
