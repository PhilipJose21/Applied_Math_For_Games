using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    [SerializeField] private GameObject flameParticles;
    [SerializeField] private float damage = 1f;
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
            flameParticles.GetComponent<AreaDamage>().SetDamage(damage);
        }
        else
        {
            flameParticles.SetActive(false);
        }
    }
}
