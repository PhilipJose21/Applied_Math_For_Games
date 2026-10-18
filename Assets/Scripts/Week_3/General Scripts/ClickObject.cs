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
        
    }

    
}
