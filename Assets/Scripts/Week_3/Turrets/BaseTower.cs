using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseTower : MonoBehaviour
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firePoint;

    [Header("Tower Stats: ")]
    [SerializeField] protected int towerLevel = 0;
    [SerializeField] protected int cost = 100;
    [SerializeField] protected int sellValue;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected float bulletSpeed = 10f;
    [SerializeField] protected float bulletLifetime = 5f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected bool hasHiddenDetection = false;
    [SerializeField] protected bool hasPierce = false;

    [Header("Selection: ")]
    [Tooltip("World units. Keep under half the grid cell size so neighbours never overlap.")]
    [SerializeField] private float clickRadius = 0.9f;

    [Header("Tower Upgrades: ")]
    [SerializeField] protected towerUpgrades[] upgrades;



    [System.Serializable] public class towerUpgrades
    {
        public string upgradeName;
        public int upgradeCost;
        public float fireRateUPG;
        public float rangeUPG;
        public int damageUPG;
        public bool hiddenDetectionUPG;
        public bool pierceUPG;
    }

    protected float cooldown;
    protected TargetDetection targetDetection;
    private Camera mainCamera;
    private GameManager gameManager;

    public virtual void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
        cooldown = fireRate;
        mainCamera = Camera.main;
        gameManager = GameManager._instance;

        GameManager.OnSelectedTowerChanged += OnSelectedTowerChanged;
        UpdateSellValue();
        UpdateRangeVisibility();
    }

    void OnDestroy()
    {
        GameManager.OnSelectedTowerChanged -= OnSelectedTowerChanged;
    }

    public virtual void Update()
    {
        if (targetDetection.IsTargetDetected())
            Fire();

        HandleSelectionInput();
    }

    protected void HandleSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
            TrySelect();
    }

    void OnSelectedTowerChanged(object sender, EventArgs e) => UpdateRangeVisibility();

    void UpdateRangeVisibility()
    {
        targetDetection.SetShapeVisible(gameManager.GetSelectedTower() == gameObject);
    }

    public virtual void TrySelect()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (!TryGetClickPoint(out Vector3 point)) return;

        Vector3 offset = point - transform.position;
        offset.y = 0f;
        if (offset.sqrMagnitude > clickRadius * clickRadius) return;

        bool alreadySelected = gameManager.GetSelectedTower() == gameObject;
        gameManager.SetSelectedTower(alreadySelected ? null : gameObject);
    }

    bool TryGetClickPoint(out Vector3 point)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float t))
        {
            point = ray.GetPoint(t);
            return true;
        }
        point = default;
        return false;
    }

    public void UpdateSellValue()
    {
        sellValue = Mathf.RoundToInt(cost * 0.5f);
        for (int i = 0; i < towerLevel; i++)
        {
            sellValue += Mathf.RoundToInt(upgrades[i].upgradeCost * 0.5f);
        }
    }

    public virtual void Fire()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
        {
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(90f, 0f, 0f);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

            var b = bullet.GetComponent<Projectile>();
            b.SetSpeed(bulletSpeed);
            b.SetLifetime(bulletLifetime);
            b.SetTarget(targetDetection.GetTarget());
            b.SetDamage(damage);
            b.SetPierce(hasPierce);
            cooldown = fireRate;
        }
    }

    public virtual void Upgrade(int level)
    {
        if (level <= towerLevel || level > upgrades.Length)
        {
            Debug.Log("Max Level Reached");
            return;
        }

        towerUpgrades upgrade = upgrades[level - 1];
        damage += upgrade.damageUPG;
        fireRate -= upgrade.fireRateUPG / 10f;
        targetDetection.setDetectionRange(targetDetection.GetDetectionRange() + upgrade.rangeUPG);
        hasHiddenDetection |= upgrade.hiddenDetectionUPG;
        hasPierce |= upgrade.pierceUPG;
        towerLevel = level;
        
        cooldown = fireRate;
        UpdateSellValue();
    }

    public virtual int GetUpgradeCost()
    {
        if (towerLevel < upgrades.Length)
            return upgrades[towerLevel].upgradeCost;
        else
            return 0;
    }

    public virtual int GetSellValue() => sellValue;

    public virtual int GetCost() => cost;

    public virtual int GetLevel() => towerLevel;

    public virtual bool GetIsHidden() => hasHiddenDetection;

    public virtual towerUpgrades GetUpgrade()
    {
        if (towerLevel < upgrades.Length)
            return upgrades[towerLevel];
        else
            return null;
    }
}