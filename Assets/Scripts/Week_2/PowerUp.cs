using UnityEngine;

enum PowerUpType
{
    AddRocket,
}

public class PowerUp : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private PowerUpType powerUpType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(this.transform.position, playerTransform.position);
        if (distance <= 1f)
        {
            if (powerUpType == PowerUpType.AddRocket)
            {
                RocketBarrage rocketBarrage = playerTransform.GetComponent<RocketBarrage>();
                if (rocketBarrage != null)
                {
                    rocketBarrage.IncreaseRocketCount();
                }
            }
            Destroy(this.gameObject);
        }
    }
}
