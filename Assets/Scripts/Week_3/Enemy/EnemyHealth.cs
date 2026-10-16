using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Slider healthBarFill;
    [SerializeField] private Slider easedHealthBarFill;
    [Range(0.1f, 10f)][SerializeField] private float easeSpeed = 8f;
    Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        healthBarFill.maxValue = enemy.GetMaxHealth();
        healthBarFill.value = enemy.GetCurrentHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Mathf.Approximately(healthBarFill.value, easedHealthBarFill.value))
        {
            float easeStep = 1f - Mathf.Exp(-easeSpeed * Time.deltaTime);
            easedHealthBarFill.value = Mathf.Lerp(
                easedHealthBarFill.value,
                healthBarFill.value,
                easeStep);
        }
    }

    public void updateHealthValues(float currentHealth, float maxHealth)
    {
        healthBarFill.maxValue = maxHealth;
        healthBarFill.value = currentHealth;
    }
}
