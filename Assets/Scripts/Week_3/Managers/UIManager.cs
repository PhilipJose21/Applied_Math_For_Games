using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    [Header("Health Bar")]
    [SerializeField] private Slider healthBarFill;
    [SerializeField] private Slider easedHealthBarFill;
    [Range(0.1f, 10f)][SerializeField] private float easeSpeed = 8f;
    [SerializeField] private HealthSystem healthSystem;

    [Header("Coin UI")]
    [SerializeField] private TextMeshProUGUI coinText;
    private GameManager gameManager;

    void Start()
    {
        
        gameManager = GameManager._instance;

        HealthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        HealthSystem.OnDead += HealthSystem_OnDead;
        GameManager.OnCoinChanged += GameManager_OnCoinChanged;

        healthBarFill.maxValue = healthSystem.GetMaxHealth();
        healthBarFill.value = healthSystem.GetHealth();

        UpdateCoinUI();
    }

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

    private void OnDestroy()
    {
        HealthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        HealthSystem.OnDead -= HealthSystem_OnDead;
        GameManager.OnCoinChanged -= GameManager_OnCoinChanged;
    }

    private void UpdateCoinUI()
    {
        coinText.text = gameManager.GetCoinCount().ToString();
    }

    private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        healthBarFill.value = HealthSystem._instance.GetHealth();
    }

    private void GameManager_OnCoinChanged(object sender, EventArgs e)
    {
        UpdateCoinUI();
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}
