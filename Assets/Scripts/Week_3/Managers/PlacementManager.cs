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
    [SerializeField] private Renderer placementArea;

    [Header("Placement Math")]
    [SerializeField] private float groundY = 0f;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private Vector2Int gridMin = new(-10, -10);
    [SerializeField] private Vector2Int gridMax = new(10, 10);

    private Camera cam;
    private GameObject preview;
    private Renderer[] previewRenderers;
    private readonly HashSet<Vector2Int> occupied = new();

    private Vector3 mousePosition;   // snapped world position
    private bool hasGroundPoint;
    private bool canPlace;

    void Awake() => _instance = this;

    void Start()
    {
        cam = Camera.main;
        if (selectedTower != null) CreateTowerPreview();
    }

    void Update()
    {
        if (preview == null) return;

        // Cursor is over a UI element (tower buttons, etc.): hide the ghost, don't place
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

    // --- Math helpers (no physics) ---

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

        // Make the preview inert so turret logic
        foreach (var mb in preview.GetComponentsInChildren<MonoBehaviour>())
            mb.enabled = false;
        foreach (var col in preview.GetComponentsInChildren<Collider>())
            col.enabled = false;

        previewRenderers = preview.GetComponentsInChildren<Renderer>();
    }

    bool CanPlaceTower()
    {
        return InBounds(mousePosition) && !occupied.Contains(ToCell(mousePosition));
    }

    bool InBounds(Vector3 p)
    {
        Bounds b = placementArea.bounds;
        return p.x >= b.min.x && p.x <= b.max.x &&
            p.z >= b.min.z && p.z <= b.max.z;
    }

    void PlaceTower()
    {
        Instantiate(selectedTower, mousePosition, Quaternion.identity);
        occupied.Add(ToCell(mousePosition));

        Destroy(preview);
        preview = null;
        selectedTower = null;

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

    public void FreeCell(Vector3 worldPos) => occupied.Remove(ToCell(worldPos));

    
    Vector3 Snap(Vector3 p) => new(
        Mathf.Round(p.x / cellSize) * cellSize,
        groundY,
        Mathf.Round(p.z / cellSize) * cellSize);

    Vector2Int ToCell(Vector3 p) => new(
        Mathf.RoundToInt(p.x / cellSize),
        Mathf.RoundToInt(p.z / cellSize));
}