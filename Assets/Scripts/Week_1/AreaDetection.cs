using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AreaDetection : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform targetNoZone;
    [SerializeField] private Transform targetFinalZone;

    [Header("Detection Radius Settings")]
    [SerializeField] private float detectionRadius_Initial = 5f;
    [SerializeField] private float detectionRadius_Secondary = 2.5f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private bool wasInsideInitialArea = false;

    [Header("Shake Settings")]
    [SerializeField] private float shakeAmp = 0.15f;
    [SerializeField] private float shakeFreq = 25f;
    private Vector3 noZoneOriginalPosition;

    [Header("Material Settings")]
    [SerializeField] private Color initialAreaColor;
    [SerializeField] private Color finalAreaColor;
    private Renderer targetRenderer;
    private Coroutine colorRoutine;

    [Header("FinishZone Settings")]
    [SerializeField] private TextMeshProUGUI finishText;

    void Start()
    {
        float additionalScale = 1.75f; // Adjust this value to control the size of the detection area
        targetNoZone.localScale = new Vector3(detectionRadius_Initial * additionalScale, detectionRadius_Initial * additionalScale, detectionRadius_Initial * additionalScale);
        targetFinalZone.localScale = new Vector3(detectionRadius_Initial * additionalScale, detectionRadius_Initial * additionalScale, detectionRadius_Initial * additionalScale);
        targetRenderer = targetNoZone.GetComponent<Renderer>();
        targetRenderer.material.color = initialAreaColor;
        wasInsideInitialArea = false;
        noZoneOriginalPosition = targetNoZone.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckNoZone(targetNoZone);
        CheckFinalZone(targetFinalZone);
    }

    void CheckFinalZone(Transform transform)
    {
        float distance = Vector3.Distance(player.position, transform.position);
        finishText.enabled = (distance <= detectionRadius_Initial);
    }

    void CheckNoZone(Transform transform)
    {
        float distance = Vector3.Distance(player.position, noZoneOriginalPosition);
        if (distance <= detectionRadius_Secondary)
        {
            InsideSecondaryArea();
            return;
        }
        bool isInsideInitialArea = distance <= detectionRadius_Initial;
        if (isInsideInitialArea != wasInsideInitialArea)
        {
            wasInsideInitialArea = isInsideInitialArea;
            StartColorTransition(isInsideInitialArea);
        }
        if (isInsideInitialArea)
        {
            ApplyShake();
        }
        else
        {
            ResetShake();
        }
    }

    void ApplyShake()
    {
        float offsetX = (Mathf.PerlinNoise(Time.time * shakeFreq, 0f) - 0.5f) * 2f * shakeAmp;
        float offsetY = (Mathf.PerlinNoise(0f, Time.time * shakeFreq) - 0.5f) * 2f * shakeAmp;
        targetNoZone.position = noZoneOriginalPosition + new Vector3(offsetX, offsetY, 0f);
    }

    void ResetShake()
    {
        targetNoZone.position = noZoneOriginalPosition;
    }

    void StartColorTransition(bool isInside)
    {
        if (colorRoutine != null)
        {
            StopCoroutine(colorRoutine);
        }
        colorRoutine = StartCoroutine(ChangeColor(isInside));
    }

    void InsideSecondaryArea()
    {
        SceneManager.LoadScene("Week_1");
    }

    IEnumerator ChangeColor(bool isInside = false)
    {
        Color startColor = targetRenderer.material.color;
        Color targetColor = isInside ? finalAreaColor : initialAreaColor;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > duration && isInside)
            {
                InsideSecondaryArea();
            }
            float t = Mathf.Clamp01(elapsedTime / duration);
            targetRenderer.material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        targetRenderer.material.color = targetColor;
        colorRoutine = null;
    }
}