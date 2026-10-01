using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform coinPrefab;
    [SerializeField] private Transform[] pathPoints;

    public float GetSpeed()
    {
        return speed;
    }

    public void Death()
    {
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

   
}
