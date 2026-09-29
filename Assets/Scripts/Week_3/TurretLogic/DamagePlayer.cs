using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    private TargetDetection targetDetection;
    void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
    }

    void Update()
    {
        if (targetDetection.IsTargetDetected())
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
                GameManager._instance.GameOver();
                player.SetActive(false);
            }
        }
    }
}
