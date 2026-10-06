using UnityEngine;

public class MissileWeek5 : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float hitDistance = .5f;
    private Transform target;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    [System.Obsolete]
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f || target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        if (direction.sqrMagnitude <= hitDistance * hitDistance)
        {
            Debug.Log("HIT");
            target.GetComponent<PlaneMovement>().TakeDamage(1);
            Destroy(gameObject);
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
