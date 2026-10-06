using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [SerializeField] private bool isExplosive = false;
    [SerializeField] private GameObject explosion; 
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private float lifeTime = 5f;

    private float speed;
    private float timer;
    private bool hasExploded;

    private void Awake()
    {
        timer = lifeTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        if (hasExploded) return;
        if (target == null)
        {
            transform.position += transform.up * speed * Time.deltaTime;
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        transform.position += toTarget.normalized * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) <= detectionRange)
            Hit();
    }

    private void Hit()
    {
        if (isExplosive)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        else
        {
            GameManager._instance.GetEnemyManager().RemoveTarget(target);
            if (target.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.Death();
            }
        }
        Destroy(gameObject);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetLifetime(float newLifetime)
    {
        timer = newLifetime;
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}