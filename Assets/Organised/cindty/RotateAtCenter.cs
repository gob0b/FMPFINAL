using UnityEngine;

public class RotateAtCentre : MonoBehaviour
{
    [Header("Target Following")]
    public Transform target;
    public float followSpeed = 5f;

    [Header("Rotation Settings")]
    public Vector3 maxRotationSpeed = new Vector3(90f, 90f, 90f);
    public float rotationBuildUpTime = 3f;

    [Header("Emission Settings")]
    public Color emissionColor = Color.white;
    public float maxEmissionIntensity = 5f;
    public float emissionBuildUpTime = 3f;

    [Header("Disappear Settings")]
    public float timeUntilInvisible = 5f;

    [Header("Scale Settings")]
    public Vector3 firstTargetScale = new Vector3(1f, 1f, 1f);
    public float firstScaleUpTime = 2f;

    public float firstScaleHoldDuration = 1f; // NEW — how long to hold first scale before second scale starts

    public Vector3 secondTargetScale = new Vector3(2f, 2f, 2f);
    public float secondScaleUpTime = 2f;

    private Renderer objRenderer;
    private Material objMaterial;

    private float rotationTimer = 0f;
    private float emissionTimer = 0f;
    private float visibilityTimer = 0f;
    private float scaleTimer = 0f;
    private bool isInvisible = false;

    private bool firstScaleComplete = false;
    private bool secondScaleStarted = false;
    private bool secondScaleComplete = false;
    private float holdTimer = 0f;
    private float secondScaleTimer = 0f;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            objMaterial = objRenderer.material;
            objMaterial.EnableKeyword("_EMISSION");
        }

        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (isInvisible) return;

        float deltaTime = Time.deltaTime;

        // --- First Scale Up ---
        if (!firstScaleComplete)
        {
            scaleTimer += deltaTime;
            float scaleProgress = Mathf.Clamp01(scaleTimer / firstScaleUpTime);
            transform.localScale = Vector3.Lerp(Vector3.zero, firstTargetScale, scaleProgress);

            if (scaleProgress >= 1f)
            {
                firstScaleComplete = true;
                scaleTimer = 0f;
            }
        }
        // --- Hold at first scale ---
        else if (!secondScaleStarted)
        {
            holdTimer += deltaTime;
            if (holdTimer >= firstScaleHoldDuration)
            {
                secondScaleStarted = true;
            }
        }
        // --- Second Scale Up ---
        else if (!secondScaleComplete)
        {
            secondScaleTimer += deltaTime;
            float secondProgress = Mathf.Clamp01(secondScaleTimer / secondScaleUpTime);
            transform.localScale = Vector3.Lerp(firstTargetScale, secondTargetScale, secondProgress);

            if (secondProgress >= 1f)
            {
                secondScaleComplete = true;
            }
        }

        // --- Follow Target ---
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * deltaTime);
        }

        // --- Rotation ---
        rotationTimer += deltaTime;
        float rotationProgress = Mathf.Clamp01(rotationTimer / rotationBuildUpTime);
        Vector3 currentRotation = Vector3.Lerp(Vector3.zero, maxRotationSpeed, rotationProgress);
        transform.Rotate(currentRotation * deltaTime, Space.Self);

        // --- Emission ---
        if (objMaterial != null)
        {
            emissionTimer += deltaTime;
            float emissionProgress = Mathf.Clamp01(emissionTimer / emissionBuildUpTime);
            float currentEmission = Mathf.Lerp(0f, maxEmissionIntensity, emissionProgress);
            objMaterial.SetColor("_EmissionColor", emissionColor * currentEmission);
        }

        // --- Invisibility ---
        visibilityTimer += deltaTime;
        if (visibilityTimer >= timeUntilInvisible && objRenderer != null)
        {
            objRenderer.enabled = false;
            isInvisible = true;
        }
    }
}
