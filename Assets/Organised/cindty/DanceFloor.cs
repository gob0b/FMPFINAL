using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DanceFloor : MonoBehaviour
{
    [Header("Emission Settings")]
    public Color emissionColor = Color.white;
    public float maxEmissionIntensity = 2f;
    public float fadeSpeed = 1f;
    public float startDelay = 1f;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;
    private float currentIntensity = 0f;
    private bool isIncreasing = true;
    private bool isAnimating = false;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        rend.GetPropertyBlock(propBlock);

        SetEmission(0f);
        Invoke(nameof(StartEmissionCycle), startDelay);
    }

    void StartEmissionCycle()
    {
        isAnimating = true;
        isIncreasing = true;
    }

    private void Update()
    {
        if (!isAnimating) return;

        float step = fadeSpeed * Time.deltaTime;

        if (isIncreasing)
        {
            currentIntensity += step;
            if (currentIntensity >= maxEmissionIntensity)
            {
                currentIntensity = maxEmissionIntensity;
                isIncreasing = false;
            }
        }
        else
        {
            currentIntensity -= step;
            if (currentIntensity <= 0f)
            {
                currentIntensity = 0f;
                isAnimating = false;
                Invoke(nameof(StartEmissionCycle), startDelay); // Wait before starting again
            }
        }

        SetEmission(currentIntensity);
    }

    void SetEmission(float intensity)
    {
        Color finalColor = emissionColor * intensity;
        propBlock.SetColor("_EmissionColor", finalColor);
        rend.SetPropertyBlock(propBlock);

        if (intensity > 0f)
            rend.material.EnableKeyword("_EMISSION");
        else
            rend.material.DisableKeyword("_EMISSION");
    }
}

