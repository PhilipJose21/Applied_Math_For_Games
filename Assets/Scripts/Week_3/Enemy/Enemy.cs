using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    public float currentHealth;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform coinPrefab;
    [SerializeField] private Transform[] pathPoints;

    EnemyHealth enemyHealth;
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth component not found on the enemy.");
            return;
        }
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
        enemyHealth.updateHealthValues(currentHealth, maxHealth);
        if (currentHealth <= 0f)
        {
            Death();
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
   
}
