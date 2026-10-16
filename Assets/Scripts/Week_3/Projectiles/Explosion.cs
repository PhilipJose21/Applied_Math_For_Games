using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionDuration = 0.5f;
    [SerializeField] private float damage = 10f;

    private void Start()
    {
        Destroy(gameObject, explosionDuration);
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}
