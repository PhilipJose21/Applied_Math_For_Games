using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    [SerializeField] private GameObject flameParticles;
    PlayerDetection playerDetection;

    void Start()
    {
        playerDetection = GetComponent<PlayerDetection>();
        flameParticles.SetActive(false);
    }

    void Update()
    {
        if (playerDetection.IsPlayerDetected())
        {
            flameParticles.SetActive(true);
        }
        else
        {
            flameParticles.SetActive(false);
        }
    }
}
