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
        var position = transform.position;
        if (target == null)
        {
            transform.position += transform.up * speed * Time.deltaTime;
            return;
        }
        transform.position = position + (target.position - transform.position).normalized * speed * Time.deltaTime;
        detectTarget();
        destroyProjectile();
    }

    void detectTarget()
    {
        if (target != null)
        {
            // check if the target is within the detection range
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer <= detectionRange)
            {
                EnemyManager enemyManager = GameManager._instance.GetEnemyManager();
                enemyManager.RemoveTarget(target);
                if (target.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.Death();
                }
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
