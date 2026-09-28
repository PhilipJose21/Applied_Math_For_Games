using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;
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
            if (distanceToPlayer < detectionRange)
            {
                Debug.Log("Player hit by projectile!");
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


}
