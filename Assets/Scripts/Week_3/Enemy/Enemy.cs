using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform[] pathPoints;

    public float GetSpeed()
    {
        return speed;
    }

   
}
