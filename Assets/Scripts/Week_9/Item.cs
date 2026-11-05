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
        if(spriteRenderer != null)
        {
            spriteRenderer.color = GetRandomColor();
        }
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
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-itemPosition.z);
        }
    }

    private Color GetRandomColor()
    {
        var rRand = Random.Range(0f, 1f);
        var gRand = Random.Range(0f, 1f);
        var bRand = Random.Range(0f, 1f);
        return new Color(rRand, gRand, bRand);
    }
}