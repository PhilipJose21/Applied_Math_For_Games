using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    public static HealthSystem _instance;

    public static event EventHandler OnHealthChanged; 
    public static event EventHandler OnDead;

    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        _instance = this;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthNormalized()
    {
        if (maxHealth <= 0) return 0f;
        return (float)currentHealth / maxHealth;
    }
}
