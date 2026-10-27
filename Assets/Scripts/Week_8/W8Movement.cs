using UnityEngine;

public class W8Movement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _jumpSpeed = 8f;

    private AABBCollider _collider;
    private Vector2 _velocity;

    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        _collider = GetComponent<AABBCollider>();
    }

    private void Update()
    {
        _velocity.x = Input.GetAxisRaw("Horizontal") * _moveSpeed;

        if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
            _velocity.y = _jumpSpeed;

        _velocity.y += _gravity * Time.deltaTime;

        Move(_velocity * Time.deltaTime);
    }

    private void Move(Vector2 delta)
    {
        Vector2 pos = transform.position;
        Vector2 half = _collider.HalfSize;
        var manager = CollisionManager.Instance;

        if (delta.x != 0)
        {
            pos.x += delta.x;
            var hit = manager.GetOverlap(_collider, pos);
            if (hit != null)
            {
                pos.x = delta.x > 0 ? hit.min.x - half.x : hit.max.x + half.x;
                _velocity.x = 0;
            }
        }

        
        IsGrounded = false;
        pos.y += delta.y;
        var hitY = manager.GetOverlap(_collider, pos);
        if (hitY != null)
        {
            if (delta.y < 0)
            {
                pos.y = hitY.max.y + half.y;
                IsGrounded = true;
            }
            else
            {
                pos.y = hitY.min.y - half.y;
            }
            _velocity.y = 0;
        }

        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

}
