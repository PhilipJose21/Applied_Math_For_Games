using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private float lifetime;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 1f;

    float timer;
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
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
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= detectionRange)
            {
                GameManager._instance.GameOver();
                playerTransform.gameObject.SetActive(false);
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


}
