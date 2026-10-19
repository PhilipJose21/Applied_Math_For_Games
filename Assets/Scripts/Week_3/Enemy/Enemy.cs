using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    public float currentHealth;
    [SerializeField] private float speed = 5f;

    [SerializeField] private int defense;
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

    public void Death()
    {
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        if (defense > 0){currentHealth -= damage * defense;}
        else{currentHealth -= damage;}
        enemyHealth.updateHealthValues(currentHealth, maxHealth);
        if (currentHealth <= 0f){Death();}
    }

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;

    public float GetSpeed() => speed;
   
}
