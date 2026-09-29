using UnityEngine;
using System.Collections;


public class Movement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool hasDiagonalMovement = true;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            this.gameObject.AddComponent<CharacterController>();
            controller = GetComponent<CharacterController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (!hasDiagonalMovement) //prevents diagonal movement by setting the smaller axis to 0
        {
            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                vertical = 0f;
            }
            else
            {
                horizontal = 0f;
            }
        }
        
        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        controller.Move(direction * speed * Time.deltaTime);
    }
}
