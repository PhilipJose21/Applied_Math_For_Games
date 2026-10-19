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

    [Header("UI Panels")]
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject upgradeUI;

    [Header("Upgrade UI Elements")]
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI sellCostText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;

    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button cancelPlacementButton;

    [Header("Coin UI")]
    [SerializeField] private TextMeshProUGUI coinText;
    private GameManager gameManager;

    void Start()
    {
        
        gameManager = GameManager._instance;

        HealthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        HealthSystem.OnDead += HealthSystem_OnDead;
        GameManager.OnCoinChanged += GameManager_OnCoinChanged;
        GameManager.OnSelectedTowerChanged += GameManager_OnSelectedTowerChanged;
        GameManager.OnTowerUpgraded += GameManager_OnTowerUpgraded;
        EnemyManager.OnWaveEnded += EnemyManager_OnWaveEnded;

        healthBarFill.maxValue = healthSystem.GetMaxHealth();
        healthBarFill.value = healthSystem.GetHealth();

        winUI.SetActive(false);
        loseUI.SetActive(false);
        //pauseUI.SetActive(false);
        upgradeUI.SetActive(false);

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
        GameManager.OnSelectedTowerChanged -= GameManager_OnSelectedTowerChanged;
        GameManager.OnTowerUpgraded -= GameManager_OnTowerUpgraded;
        EnemyManager.OnWaveEnded -= EnemyManager_OnWaveEnded;
    }

    private void UpdateUpgradeUI(BaseTower tower)
    {
        var u = tower?.GetUpgrade();
        if (u == null)
        {
            return;
        }

        upgradeUI.SetActive(true);
        upgradeNameText.text = u?.upgradeName;
        // upgradeCostText.text = u != null ? u.upgradeCost.ToString() : "";
        // sellCostText.text = gameManager.GetSelectedTower()?.GetComponent<BaseTower>().GetSellValue().ToString();
        // upgradeDescriptionText.text = u != null ? $"Fire Rate: {(u.fireRateUPG.isToggled ? "+" : "")}{u.fireRateUPG.value}\n" +
        //     $"Range: {(u.rangeUPG.isToggled ? "+" : "")}{u.rangeUPG.value}\n" +
        //     $"Damage: {(u.damageUPG.isToggled ? "+" : "")}{u.damageUPG.value}\n" +
        //     $"Hidden Detection: {(u.hiddenDetectionUPG ? "Yes" : "No")}\n" +
        //     $"Pierce: {(u.pierceUPG ? "Yes" : "No")}" : "";
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
        loseUI.SetActive(true);
    }

    private void GameManager_OnSelectedTowerChanged(object sender, EventArgs e)
    {
        UpdateUpgradeUI(gameManager.GetSelectedTower()?.GetComponent<BaseTower>());
    }

    
    private void GameManager_OnTowerUpgraded(object sender, EventArgs e)
    {
        UpdateUpgradeUI(gameManager.GetSelectedTower()?.GetComponent<BaseTower>());
    }

    private void EnemyManager_OnWaveEnded(object sender, EventArgs e)
    {
        winUI.SetActive(true);
    }
}
