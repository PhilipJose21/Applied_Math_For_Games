using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager _instance;
    public static event EventHandler OnTowerPlaced;

    [SerializeField] private GameObject selectedTower;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [Tooltip("Towers can only be placed fully inside this renderer's area (the Plane).")]
    [SerializeField] private Renderer placementArea;

    [Header("Blocked Areas")]
    [SerializeField] private Renderer[] blockedAreas;
    [SerializeField] private float towerHalfSize = 0.5f;

    [Header("Placement Math")]
    [SerializeField] private float groundY = 0f;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private Vector2Int gridMin = new(-10, -10);
    [SerializeField] private Vector2Int gridMax = new(10, 10);

    private const float Epsilon = 0.001f;

    private Camera cam;
    private GameObject preview;
    private Renderer[] previewRenderers;
    private readonly List<Transform> placedTowers = new();

    private Vector3 mousePosition;
    private bool hasGroundPoint;
    private bool canPlace;
    SoundManager soundManager;

    void Awake() => _instance = this;

    void Start()
    {
        cam = Camera.main;

        if (blockedAreas == null || blockedAreas.Length == 0)
            FindPathAreas();

        foreach (var tower in FindObjectsByType<BaseTower>(FindObjectsSortMode.None))
            placedTowers.Add(tower.transform);

        if (selectedTower != null) CreateTowerPreview();
        soundManager = SoundManager._instance;
    }

    void Update()
    {
        if (preview == null) return;

        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (overUI)
        {
            preview.SetActive(false);
            return;
        }

        hasGroundPoint = TryGetGroundPoint(out Vector3 hit);
        preview.SetActive(hasGroundPoint);
        if (!hasGroundPoint) return;

        mousePosition = Snap(hit);
        preview.transform.position = mousePosition;

        canPlace = CanPlaceTower();
        SetPreviewMaterial(canPlace ? validMaterial : invalidMaterial);

        if (canPlace && Input.GetMouseButtonDown(0))
            checkCost();
        else if (Input.GetMouseButtonDown(1))
        {
            Destroy(preview);
            preview = null;
            selectedTower = null;
        }
    }

    bool TryGetGroundPoint(out Vector3 point)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float denom = ray.direction.y;

        if (Mathf.Abs(denom) < 1e-5f) { point = default; return false; }

        float t = (groundY - ray.origin.y) / denom;
        if (t < 0f) { point = default; return false; }

        point = ray.origin + ray.direction * t;
        return true;
    }

    void CreateTowerPreview()
    {
        if (preview != null) Destroy(preview);

        preview = Instantiate(selectedTower);

        foreach (var mb in preview.GetComponentsInChildren<MonoBehaviour>())
            mb.enabled = false;
        foreach (var col in preview.GetComponentsInChildren<Collider>())
            col.enabled = false;

        previewRenderers = preview.GetComponentsInChildren<Renderer>();
    }

    bool CanPlaceTower()
    {
        if (!FootprintInsidePlane(mousePosition)) return false;

        if (blockedAreas != null)
        {
            foreach (var area in blockedAreas)
            {
                if (area != null && FootprintOverlapsBounds(mousePosition, area.bounds))
                    return false;
            }
        }

        placedTowers.RemoveAll(t => t == null);
        float minSeparation = towerHalfSize * 2f - Epsilon;
        foreach (var t in placedTowers)
        {
            Vector3 d = t.position - mousePosition;
            if (Mathf.Abs(d.x) < minSeparation && Mathf.Abs(d.z) < minSeparation)
                return false;
        }

        return true;
    }

    bool FootprintInsidePlane(Vector3 p)
    {
        Bounds b = placementArea.bounds;
        return p.x - towerHalfSize >= b.min.x - Epsilon && p.x + towerHalfSize <= b.max.x + Epsilon &&
               p.z - towerHalfSize >= b.min.z - Epsilon && p.z + towerHalfSize <= b.max.z + Epsilon;
    }

    bool FootprintOverlapsBounds(Vector3 p, Bounds b)
    {
        return p.x + towerHalfSize > b.min.x + Epsilon && p.x - towerHalfSize < b.max.x - Epsilon &&
               p.z + towerHalfSize > b.min.z + Epsilon && p.z - towerHalfSize < b.max.z - Epsilon;
    }

    void FindPathAreas()
    {
        var found = new List<Renderer>();
        foreach (var r in FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            if (r.gameObject.name.StartsWith("Path"))
                found.Add(r);
        }
        blockedAreas = found.ToArray();
        Debug.Log($"PlacementManager: blocking placement on {blockedAreas.Length} path pieces.");
    }

    void PlaceTower()
    {
        GameObject placed = Instantiate(selectedTower, mousePosition, Quaternion.identity);
        placedTowers.Add(placed.transform);

        Destroy(preview);
        preview = null;
        selectedTower = null;
        soundManager.PlaySFX(soundManager.towerPlaceClip);
        OnTowerPlaced?.Invoke(this, EventArgs.Empty);
    }

    void checkCost()
    {
        if (selectedTower != null)
        {
            BaseTower tower = selectedTower.GetComponent<BaseTower>();
            if (tower != null)
            {
                int cost = tower.GetCost();
                if (GameManager._instance.GetCoinCount() >= cost)
                {
                    GameManager._instance.RemoveCoin(cost);
                    PlaceTower();
                }
                else
                {
                    Debug.Log("Not enough resources to place the tower.");
                }
            }
        }
    }

    void SetPreviewMaterial(Material mat)
    {
        foreach (var r in previewRenderers) r.sharedMaterial = mat;
    }

    public void SetSelectedTower(GameObject towerPrefab)
    {
        selectedTower = towerPrefab;
        CreateTowerPreview();
    }

    public void FreeCell(Vector3 worldPos)
    {
        placedTowers.RemoveAll(t => t == null ||
            (Mathf.Abs(t.position.x - worldPos.x) < Epsilon && Mathf.Abs(t.position.z - worldPos.z) < Epsilon));
    }

    Vector3 Snap(Vector3 p) => new(
        Mathf.Round(p.x / cellSize) * cellSize,
        groundY,
        Mathf.Round(p.z / cellSize) * cellSize);
}