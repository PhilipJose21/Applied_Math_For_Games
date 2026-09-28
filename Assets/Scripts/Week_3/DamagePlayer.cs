using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    private PlayerDetection playerDetection;
    void Start()
    {
        playerDetection = GetComponent<PlayerDetection>();
    }

    void Update()
    {
        if (playerDetection.IsPlayerDetected())
        {
            Damage();
        }
    }

    public void Damage()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Movement playerMovement = player.GetComponent<Movement>();
            if (playerMovement != null)
            {
                playerMovement.Die();
            }
        }
    }
}
