using UnityEngine;
using UnityEngine.Events;


public class RotateTurret : MonoBehaviour
{
    public enum RotationType
    {
        None,
        AutoRotate,
        LookAtTarget,
        LookAtTargetWithDelay
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
        else if (rotationType == RotationType.LookAtTargetWithDelay)
        {
            LookAtTargetWithDelay(player);
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

    void LookAtTargetWithDelay(Transform target)
    {
        if (target == null) return;//if there is no target, return

        //calculate the direction to the target and the angle to rotate towards it
        var dir = target.position - transform.position;
        var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        var speed = rotationSpeed * Time.deltaTime;
       
        //rotates turret
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, angle, 0),speed
        );
    }

    void LookAtTarget(Transform target)
    {
        if (target == null) return;//if there is no target, return

        //calculate the direction to the target and the angle to rotate towards it
        var dir = target.position - transform.position;
        var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        //rotates turret
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        Quaternion.Euler(0, angle, 0), 
        rotationSpeed * Time.deltaTime);
    }
}
