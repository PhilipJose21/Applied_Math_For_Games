using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private bool isExplosive = false;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private float lifeTime = 5f;

    private float damage;
    private float speed;
    private float timer;
    private bool hasExploded;
    private bool homing = true;   // false = fly straight and hit whatever it passes near

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

        // Straight-flying projectile (shotgun pellets)
        if (!homing)
        {
            transform.position += transform.up * speed * Time.deltaTime;
            CheckStraightHit();
            return;
        }

        if (!homing)
        {
            Vector3 prev = transform.position;
            transform.position += transform.up * speed * Time.deltaTime;
            CheckStraightHit(prev, transform.position);
            return;
        }

        // Original homing behavior
        if (target == null)
        {
            transform.position += transform.up * speed * Time.deltaTime;
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        transform.position += toTarget.normalized * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) <= detectionRange)
            Hit(target);
    }

    private void CheckStraightHit()
    {
        if (EnemyManager._instance == null) return;

        var enemies = EnemyManager._instance.GetAllTargets();
        float rangeSqr = detectionRange * detectionRange;

        for (int i = 0; i < enemies.Count; i++)
        {
            Transform e = enemies[i];
            if (e == null) continue;

            Vector3 d = e.position - transform.position;
            d.y = 0f; // ignore height differences
            if (d.sqrMagnitude <= rangeSqr)
            {
                Hit(e);
                return;
            }
        }
    }

    private void Hit(Transform victim)
    {
        if (isExplosive)
        {
            GameObject Explosion = Instantiate(explosion, transform.position, Quaternion.identity);
            Explosion.GetComponent<AreaDamage>().SetDamage(damage);
        }
        else if (victim != null && victim.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    private void CheckStraightHit(Vector3 from, Vector3 to)
    {
        if (EnemyManager._instance == null) return;

        var enemies = EnemyManager._instance.GetAllTargets();
        float rangeSqr = detectionRange * detectionRange;

        Vector3 seg = to - from; seg.y = 0f;
        float segLenSqr = seg.sqrMagnitude;

        for (int i = 0; i < enemies.Count; i++)
        {
            Transform e = enemies[i];
            if (e == null) continue;

            Vector3 toEnemy = e.position - from; toEnemy.y = 0f;

            // Closest point on the travelled segment to the enemy
            float t = segLenSqr > 0f ? Mathf.Clamp01(Vector3.Dot(toEnemy, seg) / segLenSqr) : 0f;
            Vector3 closest = seg * t;

            if ((toEnemy - closest).sqrMagnitude <= rangeSqr)
            {
                Hit(e);
                return;
            }
        }
    }

    public void SetSpeed(float newSpeed) => speed = newSpeed;
    public void SetLifetime(float newLifetime) => timer = newLifetime;
    public void SetTarget(Transform newTarget) => target = newTarget;
    public void SetDamage(float damage) => this.damage = damage;
    public void SetHoming(bool value) => homing = value;
}