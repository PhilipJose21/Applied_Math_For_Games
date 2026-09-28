using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    [SerializeField] private GameObject flameParticles;
    PlayerDetection playerDetection;
    ParticleSystem ps;

    void Start()
    {
        playerDetection = GetComponent<PlayerDetection>();
        ps = flameParticles.GetComponentInChildren<ParticleSystem>(true);
        ApplyDetectionShape();
        flameParticles.SetActive(false);
    }

    void Update()
    {
        bool detected = playerDetection.IsPlayerDetected();
        flameParticles.SetActive(detected);

        if (detected)
        {
            ApplyDetectionShape();
        }
    }

    void ApplyDetectionShape()
    {
        var shape = ps.shape;
        shape.angle = playerDetection.GetDetectionAngle() / 2f;
        shape.radius = playerDetection.GetDetectionRange();

        var main = ps.main;

        float speed = 10f;
        main.startSpeed = speed;
    }
}