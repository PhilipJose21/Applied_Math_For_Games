using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseTower : MonoBehaviour
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firePoint;

    [Header("Tower Stats: ")]
    [SerializeField] protected float level = 0;
    [SerializeField] protected int cost = 100;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected float bulletSpeed = 10f;
    [SerializeField] protected float bulletLifetime = 5f;
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected bool hasHiddenDetection = false;
    [SerializeField] protected bool hasPierce = false;

    [Header("Selection: ")]
    [Tooltip("World units. Keep under half the grid cell size so neighbours never overlap.")]
    [SerializeField] private float clickRadius = 0.9f;

    [Header("Tower Upgrades: ")]
    [SerializeField] protected towerUpgrades[] upgrades;

    [System.Serializable] public struct toggleFloat
    {
        public float value;
        public bool isToggled;
    }

    [System.Serializable] public class towerUpgrades
    {
        public toggleFloat fireRateUPG;
        public toggleFloat rangeUPG;
        public toggleFloat damageUPG;
        public bool hiddenDetectionUPG;
        public bool pierceUPG;
    }

    private float cooldown;
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

        if (Input.GetMouseButtonDown(0))   // once per click, not every frame it's held
            TrySelect();
    }

    void OnSelectedTowerChanged(object sender, EventArgs e) => UpdateRangeVisibility();

    void UpdateRangeVisibility()
    {
        targetDetection.SetShapeVisible(gameManager.GetSelectedTower() == gameObject);
    }

    void TrySelect()
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
            cooldown = fireRate;
        }
    }

    public virtual void Upgrade(int level)
    {
        switch (level)
        {
        }
    }

    public virtual int GetCost() => cost;
}