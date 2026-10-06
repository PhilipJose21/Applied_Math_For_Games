using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionDuration = 0.5f;

    private void Start()
    {
        Destroy(gameObject, explosionDuration);
    }
}
