using UnityEngine;

public class ClickObject : MonoBehaviour
{
    public float clickRadiusPixels = 50f;
    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);

            if (screenPosition.z > 0)
            {
                float distance = Vector2.Distance(Input.mousePosition, 
                                                new Vector2(screenPosition.x, screenPosition.y));
                if (distance <= clickRadiusPixels)
                {
                    OnClick();
                }
            }
        }
    }

    void OnClick()
    {
        Debug.Log("Object clicked: " + gameObject.name);
    }
}
