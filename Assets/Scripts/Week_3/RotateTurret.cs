using UnityEngine;
using UnityEngine.Events;


public class RotateTurret : MonoBehaviour
{
    public enum RotationType
    {
        None,
        AutoRotate,
        LookAtTarget,
        LookAtTargetWithOffset
    }


    private PlayerDetection playerDetection;
    [SerializeField] private RotationType rotationType = RotationType.AutoRotate;
    [SerializeField] private float rotationSpeed = 5f;
    
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerDetection = GetComponent<PlayerDetection>();
    }

    void Update()
    {
        if (playerDetection.IsPlayerDetected())
        {
            checkRotateType();
        }
    }

    public void checkRotateType()
    {
        if (rotationType == RotationType.LookAtTarget)
        {
            LookAtTarget(player);
        }
        else if (rotationType == RotationType.LookAtTargetWithOffset)
        {
            LookAtTargetWithOffset(player);
        }
        else if (rotationType == RotationType.AutoRotate)
        {
            AutoRotate();
        }
        else if (rotationType == RotationType.None)
        {
            return;
        }
    }

    void AutoRotate()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void LookAtTargetWithOffset(Transform target)
    {
        if (target == null) return;
        var dir = target.position - transform.position;
        var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        // Add an offset of 45 degrees to the rotation
        angle += 45f;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, angle, 0),
            rotationSpeed * Time.deltaTime
        );
    }

    void LookAtTarget(Transform target)
    {
        if (target == null) return;
        var dir = target.position - transform.position;
        var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        Quaternion.Euler(0, angle, 0), 
        rotationSpeed * Time.deltaTime);
    }
}
