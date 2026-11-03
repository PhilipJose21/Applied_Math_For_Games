using UnityEngine;

public class AABBCollider : MonoBehaviour
{
    [Tooltip("Size at scale 1. A default Unity cube is 1x1.")]
    [SerializeField] private Vector2 _size = Vector2.one;
    [Tooltip("Center offset in local space (for off-center pivots).")]
    [SerializeField] private Vector2 _offset = Vector2.zero;

    public Vector2 Center => transform.TransformPoint(_offset);

    public Vector2 HalfSize
    {
        get
        {
            Vector3 s = transform.lossyScale;
            Vector2 half = new Vector2(_size.x * Mathf.Abs(s.x),
                                       _size.y * Mathf.Abs(s.y)) * 0.5f;

            // Enclosing AABB of a rectangle rotated around Z
            float rad = transform.eulerAngles.z * Mathf.Deg2Rad;
            float c = Mathf.Abs(Mathf.Cos(rad));
            float sn = Mathf.Abs(Mathf.Sin(rad));
            return new Vector2(c * half.x + sn * half.y,
                               sn * half.x + c * half.y);
        }
    }

    public Vector2 min => Center - HalfSize;
    public Vector2 max => Center + HalfSize;

    private void OnEnable()
    {
        if (CollisionManager.Instance != null)
            CollisionManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        if (CollisionManager.Instance != null)
            CollisionManager.Instance.Unregister(this);
    }

    private void Update()
    {
        float z = transform.position.z;
        Vector3 a = new Vector3(min.x, min.y, z);
        Vector3 b = new Vector3(max.x, min.y, z);
        Vector3 c = new Vector3(max.x, max.y, z);
        Vector3 d = new Vector3(min.x, max.y, z);
        Debug.DrawLine(a, b, Color.red);
        Debug.DrawLine(b, c, Color.red);
        Debug.DrawLine(c, d, Color.red);
        Debug.DrawLine(d, a, Color.red);
    }

    public void OnCollision(AABBCollider other)
    {
        Debug.Log($"{name} collided with {other.name}");
    }
}