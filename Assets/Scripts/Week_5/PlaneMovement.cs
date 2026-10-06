using UnityEngine;

public class PlaneMovement : MonoBehaviour
{
    public float speed = 10;
    public float rotationSpeed = 100;
    private Transform transform;

    public int health = 5;
    void Start()
    {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RotatePlane(-1);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            RotatePlane(1);
        }
    }

    private void RotatePlane(float rotationInput)
    {
        transform.Rotate(0f, rotationInput * rotationSpeed * Time.deltaTime, 0f);
    }

    [System.Obsolete]
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Week5GameManager gameManager = FindObjectOfType<Week5GameManager>();
            gameManager.GameOver();
        }
    }
}
