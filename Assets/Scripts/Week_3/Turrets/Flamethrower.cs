using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    [SerializeField] private GameObject flameParticles;
    TargetDetection targetDetection;

    void Start()
    {
        targetDetection = GetComponent<TargetDetection>();
        flameParticles.SetActive(false);
    }

    void Update()
    {
        if (targetDetection.IsTargetDetected())
        {
            flameParticles.SetActive(true);
        }
        else
        {
            flameParticles.SetActive(false);
        }
    }
}
