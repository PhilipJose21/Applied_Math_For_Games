using UnityEngine;

public class AABBCollider : MonoBehaviour
{
    [SerializeField] private float _length;
    [SerializeField] private float _width;

    public Vector2 max {get; private set;}
    public Vector2 min {get; private set;}
    public Vector2 HalfSize => new Vector2(_width, _length) * 0.5f;

    private void OnEnable()
    {
        UpdateBounds();
        if (CollisionManager.Instance != null)
            CollisionManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        if (CollisionManager.Instance != null)
        {
            CollisionManager.Instance.Unregister(this);
        }
    }

    void Update()
    {
        UpdateBounds();
        var x = this.transform.position.x + _width * .5f;
        var y = this.transform.position.y + _length * .5f;

        var negx = this.transform.position.x - _width * .5f;
        var negy = this.transform.position.y - _length * .5f;

        max = new Vector2(x, y);
        min = new Vector2(negx, negy);

        Debug.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0), Color.red);
        Debug.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0), Color.red);
        Debug.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0), Color.red);
        Debug.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0), Color.red);
    }

    private void UpdateBounds()
    {
        Vector2 p = transform.position;
        max = p + HalfSize;
        min = p - HalfSize;
    }

    public void OnCollision(AABBCollider other)
    {
        Debug.Log($"{this.name} collided with {other.name}");
    }
}
