using UnityEngine;

public class ParticleShape : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    [SerializeField] private float particleSpeed = 10f;   // world units per second
    [SerializeField] private float particleSize = 1f;     // world units

    PlayerDetection playerDetection;
    ParticleSystem ps;

    void Start()
    {
        playerDetection = GetComponent<PlayerDetection>();
        ps = particles.GetComponentInChildren<ParticleSystem>(true);

        // Emitter must sit exactly on the turret so its axes match the detection area
        ResetTransform(particles.transform);
        ResetTransform(ps.transform);

        ApplyDetectionShape();
    }

    void Update()
    {
        // Keeps the flame in sync if range/angle change while the game runs
        if (particles.activeSelf)
        {
            ApplyDetectionShape();
        }
    }

    void ResetTransform(Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    void ApplyDetectionShape()
    {
        float angle = playerDetection.GetDetectionAngle();
        // Detection range is in the turret's local space, so convert to world units
        float worldRange = playerDetection.GetDetectionRange() * transform.lossyScale.z;

        var main = ps.main;
        main.scalingMode = ParticleSystemScalingMode.Shape;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.startSpeed = particleSpeed;
        main.startLifetime = worldRange / particleSpeed;   // distance = speed * lifetime
        main.startSize = particleSize;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.01f;
        shape.radiusThickness = 1f;
        shape.arc = angle;
        shape.arcMode = ParticleSystemShapeMultiModeValue.Random;
        shape.position = Vector3.zero;

        // Lay the circle flat on XZ and center the arc on the turret's forward (+Z)
        shape.rotation = new Vector3(90f, 0f, 90f - angle / 2f);
    }
}