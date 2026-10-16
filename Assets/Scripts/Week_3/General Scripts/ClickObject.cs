using UnityEngine;

public class ClickObject : MonoBehaviour
{
    public float clickRadiusPixels = 50f;
    private Camera mainCamera;
    private GameManager gameManager;

    void Start()
    {
        mainCamera = Camera.main;
        gameManager = GameManager._instance;
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
        gameManager.SetSelectedTower(gameObject);
    }
}
