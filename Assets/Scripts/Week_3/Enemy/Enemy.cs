using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    public int currentHealth;
    [SerializeField] private int enemyValue = 1;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float defense;
    [SerializeField] private bool isHidden;
    [SerializeField] private Transform coinPrefab;
    [SerializeField] private Transform defeatPrefab;
    [SerializeField] private Transform[] pathPoints;

    private float rewardMultiplier = 1f;   // grows by +50% each time wave scaling raises the enemy's stats
    private bool isDead;

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

    // Called once by EnemyManager right after the enemy is spawned, before Start() runs.
    public void ApplyWaveScaling(float healthMultiplier, float defenseMultiplier, float rewardMultiplier)
    {
        maxHealth = Mathf.Max(1, Mathf.RoundToInt(maxHealth * healthMultiplier));
        currentHealth = maxHealth;
        defense *= defenseMultiplier;
        this.rewardMultiplier = rewardMultiplier;
    }

    public void Death()
    {
        if (isDead) return;   // two hits in the same frame must not drop two coins
        isDead = true;

        Transform coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
        coin.GetComponent<RotateCoin>().SetCoinValue(GetEnemyValue());
        Transform defeat = Instantiate(defeatPrefab, transform.position, Quaternion.identity);
        defeat.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage, bool ignoreDefense = false)
    {
        if (isDead) return;

        // Defense subtracts from each hit (minimum 1). Change this line if you want a different formula.
        float reduction = ignoreDefense ? 0f : defense;
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage - reduction));

        currentHealth -= finalDamage;
        if (enemyHealth != null)
            enemyHealth.updateHealthValues(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Death();
    }

    public int GetCurrentHealth() => currentHealth;

    public int GetMaxHealth() => maxHealth;

    public float GetSpeed() => speed;

    public bool GetIsHidden() => isHidden;

    public float GetDefense() => defense;

    public int GetEnemyValue() => Mathf.Max(1, Mathf.RoundToInt(enemyValue * rewardMultiplier));

    public void SetEnemyValue(int newValue)
    {
        enemyValue = newValue;
    }

    public void SetDefense(float newDefense)
    {
        defense = newDefense;
    }
}