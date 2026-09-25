using UnityEngine;

public class Attan : MonoBehaviour
{
    public Transform _enemy;
    [SerializeField] private float rotationSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemy == null) return;
        var dir = _enemy.position - transform.position;
        var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        Quaternion.Euler(0, angle, 0), rotationSpeed * Time.deltaTime);
    }
}
