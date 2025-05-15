using UnityEngine;

public class EmissionAndLightController : MonoBehaviour
{
    [Header("Emission Settings")]
    public Material targetMaterial; // Material to apply emission
    public float startDelay = 2f;   // Delay before emission starts
    public float emissionIntensity = 5f; // Maximum emission intensity
    public float emissionDuration = 1f; // How long the emission transition lasts

    [Header("Light Settings")]
    public Light targetLight;  // Light to turn on
    public float lightDelay = 0f; // Delay before light turns on

    private Color baseEmissionColor = Color.white; // Default emission color

    private void Start()
    {
        if (targetMaterial == null)
        {
            Debug.LogError("No material assigned to targetMaterial!");
            return;
        }

        if (targetLight == null)
        {
            Debug.LogError("No light assigned to targetLight!");
            return;
        }

        // Disable emission initially
        targetMaterial.EnableKeyword("_EMISSION");
        targetMaterial.SetColor("_EmissionColor", Color.black);

        // Turn off the light
        targetLight.enabled = false;

        // Start the emission and light activation process
        Invoke(nameof(StartEmission), startDelay);
        Invoke(nameof(EnableLight), startDelay + lightDelay);
    }

    private void StartEmission()
    {
        StartCoroutine(GradualEmission());
    }

    private System.Collections.IEnumerator GradualEmission()
    {
        float elapsedTime = 0f;
        while (elapsedTime < emissionDuration)
        {
            float lerpFactor = elapsedTime / emissionDuration;
            Color emissionColor = baseEmissionColor * Mathf.Lerp(0, emissionIntensity, lerpFactor);
            targetMaterial.SetColor("_EmissionColor", emissionColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final emission value is set
        targetMaterial.SetColor("_EmissionColor", baseEmissionColor * emissionIntensity);
    }

    private void EnableLight()
    {
        targetLight.enabled = true;
    }
}

