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
        Debug.Log("Player has been damaged!");
    }
}
