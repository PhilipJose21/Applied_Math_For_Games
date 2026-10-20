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
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI fireRateText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI hiddenDetectionText;
    [SerializeField] private TextMeshProUGUI pierceText;
    [SerializeField] private TextMeshProUGUI upgradeNameText2;
    [SerializeField] private TextMeshProUGUI upgradeCostText3;

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
            upgradeUI.SetActive(false);
            SetUpgradeText(damageText, false, string.Empty);
            SetUpgradeText(fireRateText, false, string.Empty);
            SetUpgradeText(rangeText, false, string.Empty);
            SetUpgradeText(hiddenDetectionText, false, string.Empty);
            SetUpgradeText(pierceText, false, string.Empty);
            return;
        }

        upgradeUI.SetActive(true);
        upgradeNameText.text = u.upgradeName;
        upgradeCostText.text = u.upgradeCost.ToString();
        sellCostText.text = tower.GetSellValue().ToString();

        SetUpgradeText(damageText, u.damageUPG != 0, $"Damage: {u.damageUPG:+0;-0;0}");
        SetUpgradeText(fireRateText, u.fireRateUPG != 0, $"Fire Rate: {u.fireRateUPG:+0.##;-0.##;0}");
        SetUpgradeText(rangeText, u.rangeUPG != 0, $"Range: {u.rangeUPG:+0.##;-0.##;0}");
        SetUpgradeText(hiddenDetectionText, u.hiddenDetectionUPG, "Hidden Detection");
        SetUpgradeText(pierceText, u.pierceUPG, "Pierce");
    }

    private static void SetUpgradeText(TextMeshProUGUI text, bool isActive, string value)
    {
        if (text == null)
        {
            return;
        }

        text.gameObject.SetActive(isActive);
        text.text = value;
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
