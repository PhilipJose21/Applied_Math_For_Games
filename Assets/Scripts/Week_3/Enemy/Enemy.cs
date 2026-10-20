using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    public int currentHealth;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int defense;
    [SerializeField] private bool isHidden;
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

    public void TakeDamage(int damage, bool ignoreDefense = false)
    {
        if (defense > 0 && !ignoreDefense){currentHealth -= damage * defense;}
        if (defense <= 0 || ignoreDefense){currentHealth -= damage;}
        else{currentHealth -= damage;}
        enemyHealth.updateHealthValues(currentHealth, maxHealth);
        if (currentHealth <= 0f){Death();}
    }

    public int GetCurrentHealth() => currentHealth;

    public int GetMaxHealth() => maxHealth;

    public float GetSpeed() => speed;

    public bool GetIsHidden() => isHidden;
   
}
