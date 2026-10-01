using Unity.VisualScripting;
using UnityEngine;

public class RotateCoin : MonoBehaviour
{
    float delay = 0.5f;
    [SerializeField] private RectTransform targetUI;
    [SerializeField] private float speed = 1.75f;
    [SerializeField] private float distanceThreshold = 1.5f;
    private Camera mainCamera;
    private float cooldown;
    void Start()
    {
        targetUI = GameObject.FindGameObjectWithTag("CoinUI").transform as RectTransform;
        mainCamera = Camera.main;
        cooldown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 100f * Time.deltaTime);
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
        {
            GoToUI();
        }
    }

    void GoToUI()
    {
        Vector3 uiScreenPos = targetUI.position;
        uiScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(uiScreenPos);

        transform.position = Vector3.Lerp(transform.position, worldPos, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, worldPos) <= distanceThreshold)
        {
            GameManager._instance.AddCoin(1);
            Destroy(gameObject);
        }
    }
}
