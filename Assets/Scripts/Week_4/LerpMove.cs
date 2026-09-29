using UnityEngine;

public class LerpMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float timeToReachTarget;
    [SerializeField] private Transform controlPoint;

    private float totalTime;
    private Vector3 startPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        totalTime += Time.deltaTime;

        var lerpedTime = Mathf.Clamp01(totalTime / timeToReachTarget);

        // Lerp the position of the object from the start position to the target position based on the lerped time
        // transform.position = startPosition + (target.position - startPosition) * lerpedTime; //moves gradually to target
        //transform.position = startPosition + (target.position - startPosition) * (lerpedTime * lerpedTime); //Using lerp to slowly move the object to the target position then speeds up as it moves closer to the target position
        transform.position = QuadraticFast(startPosition, controlPoint.position, target.position, lerpedTime); //Using a quadratic function to move the object to the target position
    
    
    }

    void Reset()
    {
        transform.position = startPosition;
        totalTime = 0f;
    }

    private Vector3 QuadraticFast(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }
}
