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
    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject playerStatsPanel;

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
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;

    [Header("Buttons")]
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private Button cancelPlacementButton;

    [Header("Coin UI")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI waveText;
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
        pauseUI.SetActive(false);
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

        var selectedTower = gameManager.GetSelectedTower();
        if (selectedTower != null)
        {
            UpdateUpgradeUI(selectedTower.GetComponent<BaseTower>());
        }
        else
        {
            upgradeUI.SetActive(false);
        }
        waveText.text = $"Wave: {gameManager.GetCurrentWave()}";

        if (gameManager.IsGamePaused())
        {
            pauseUI.SetActive(true);
            towerPanel.SetActive(false);
            upgradeUI.SetActive(false);
            playerStatsPanel.SetActive(false);
            pauseButton.SetActive(false);
        }
        else
        {
            pauseUI.SetActive(false);
            towerPanel.SetActive(true);
            playerStatsPanel.SetActive(true);
            pauseButton.SetActive(true);
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
            upgradeButton.interactable = false;
            upgradeNameText.text = "Max Level";
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

        SpecialUpgradeText(tower);
    }

    private void SpecialUpgradeText(BaseTower tower)
    {
        if (tower.GetComponent<Flamethrower>() != null)
        {
            if (tower.GetComponent<Flamethrower>().GetLevel() == 2)
            {
                SetUpgradeText(upgradeNameText2, true, "Increase Flame Range");
            }
            else if (tower.GetComponent<Flamethrower>().GetLevel() == 4)
            {
                SetUpgradeText(upgradeNameText2, true, "Temporarily removes defense");
            }
            else
            {
                SetUpgradeText(upgradeNameText2, false, string.Empty);
            }
        }

        if (tower.GetComponent<Burster>() != null)
        {
            if (tower.GetComponent<Burster>().GetLevel() == 2)
            {
                SetUpgradeText(upgradeNameText2, true, "Burst Amount: 4");
            }
            else if (tower.GetComponent<Burster>().GetLevel() == 3)
            {
                SetUpgradeText(upgradeNameText2, true, "Burst Amount: 8");
            }
            else if (tower.GetComponent<Burster>().GetLevel() == 4)
            {
                SetUpgradeText(upgradeNameText2, true, "Burst Amount: 12");
            }
            else
            {
                SetUpgradeText(upgradeNameText2, false, string.Empty);
            }
        }

        if (tower.GetComponent<Shotgun>() != null)
        {
            if (tower.GetComponent<Shotgun>().GetLevel() == 1)
            {
                SetUpgradeText(upgradeNameText2, true, "Bullet Amount: 6");
            }
            else if (tower.GetComponent<Shotgun>().GetLevel() == 3)
            {
                SetUpgradeText(upgradeNameText2, true, "Bullet Amount: 10");
            }
            else if (tower.GetComponent<Shotgun>().GetLevel() == 4)
            {
                SetUpgradeText(upgradeNameText2, true, "Bullet Amount: 15");
            }
            else
            {
                SetUpgradeText(upgradeNameText2, false, string.Empty);
            }
        }

        if (tower.GetComponent<Rocket>() != null)
        {
            
        }
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
        upgradeUI.SetActive(false);
        towerPanel.SetActive(false);
        playerStatsPanel.SetActive(false);
        pauseUI.SetActive(false);
    }
}
