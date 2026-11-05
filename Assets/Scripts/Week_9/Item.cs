using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Vector3 itemPosition;
    public float sizeMultiplier = 2.4f;
    [HideInInspector] public bool hasHit;

    private SpriteRenderer spriteRenderer;
    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale * sizeMultiplier;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public float HalfWidth
    {
        get { return (spriteRenderer != null && spriteRenderer.sprite != null) ? spriteRenderer.sprite.bounds.extents.x * baseScale.x : 0.5f; }
    }

    public float HalfHeight
    {
        get { return (spriteRenderer != null && spriteRenderer.sprite != null) ? spriteRenderer.sprite.bounds.extents.y * baseScale.y : 0.5f; }
    }

    private void Update()
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        var perspective = CameraComponent.GetPerspective(itemPosition.z);
        transform.localScale = baseScale * perspective;
        transform.position = CameraComponent.Project(itemPosition);

        if (spriteRenderer != null)
        {
            int maxItemSortingOrder = CameraComponent.sortingBase - 100;
            int depthOffset = Mathf.RoundToInt((1000f - itemPosition.z) * 0.1f);
            spriteRenderer.sortingOrder = maxItemSortingOrder + depthOffset;
        }
    }
}