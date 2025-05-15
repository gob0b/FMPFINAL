using UnityEngine;

public class EmissionFader : MonoBehaviour
{
    [Header("Emission Settings")]
    public Color emissionColor = Color.white;
    [Range(0, 10)] public float startEmissionIntensity = 5f;
    [Range(0, 10)] public float flashEmissionIntensity = 8f;

    [Header("Timers")]
    public float fadeDuration = 5f;
    public float flashDuration = 0.2f;

    private Material mat;
    private float timer = 0f;
    private bool isFading = false;
    private bool hasFlashed = false;
    private Color originalEmission;
    private Coroutine flashRoutine;

    void Start()
    {
        // Clone material so we don't affect the original
        mat = GetComponent<Renderer>().material;
        originalEmission = emissionColor * Mathf.LinearToGammaSpace(startEmissionIntensity);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", originalEmission);
        isFading = true;
    }

    void Update()
    {
        if (isFading)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / fadeDuration);
            float currentIntensity = Mathf.Lerp(startEmissionIntensity, 0f, t);
            Color currentEmission = emissionColor * Mathf.LinearToGammaSpace(currentIntensity);
            mat.SetColor("_EmissionColor", currentEmission);

            if (t >= 1f && !hasFlashed)
            {
                isFading = false;
                hasFlashed = true;
                flashRoutine = StartCoroutine(FlashEmission());
            }
        }
    }

    private System.Collections.IEnumerator FlashEmission()
    {
        Color flashColor = emissionColor * Mathf.LinearToGammaSpace(flashEmissionIntensity);
        mat.SetColor("_EmissionColor", flashColor);
        yield return new WaitForSeconds(flashDuration);
        mat.SetColor("_EmissionColor", Color.black);
    }

    // Optional: Reset the emission to trigger again
    public void RestartEmission()
    {
        timer = 0f;
        hasFlashed = false;
        isFading = true;
        mat.SetColor("_EmissionColor", originalEmission);
    }
}

