using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    public float currentHealth;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform coinPrefab;
    [SerializeField] private Transform[] pathPoints;


    void Start()
    {
        currentHealth = maxHealth;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void Death()
    {
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            Death();
        }
    }

   
}
