using UnityEngine;

public class Item : MonoBehaviour
{
    public Vector3 itemPosition;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.color = GetRandomColor();
        }
    }

    private void Update()
    {
        var perspective = CameraComponent.focalLenth / (CameraComponent.focalLenth + itemPosition.z);
        transform.localScale = Vector3.one * perspective;
        transform.position = new Vector3(
            itemPosition.x * perspective,
            itemPosition.y * perspective,
            itemPosition.z);
    }

    private Color GetRandomColor()
    {
        var rRand = Random.Range(0f, 1f);
        var gRand = Random.Range(0f, 1f);
        var bRand = Random.Range(0f, 1f);
        return new Color(rRand, gRand, bRand);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
    }


}