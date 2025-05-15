using UnityEngine;
using System.Collections;

public class VintageTVEffect : MonoBehaviour
{
    public Material tvMaterial; // Assign the TV screen material with the shader
    public float startDelay = 2f; // Delay before turning on
    public float effectDuration = 1.5f; // How long the effect lasts
    public float flickerDuration = 1f; // Extra flicker time after turning on

    private void Start()
    {
        // Ensure shader starts in "off" state
        if (tvMaterial != null)
        {
            tvMaterial.SetFloat("_Progress", 0f);
            tvMaterial.SetFloat("_FlickerStrength", 0.2f);
        }

        // Start the effect after a delay
        StartCoroutine(DelayedTurnOn());
    }

    private IEnumerator DelayedTurnOn()
    {
        yield return new WaitForSeconds(startDelay); // Wait before turning on
        StartCoroutine(TVTurnOn());
    }

    private IEnumerator TVTurnOn()
    {
        float elapsed = 0f;

        while (elapsed < effectDuration)
        {
            float progress = elapsed / effectDuration;
            tvMaterial.SetFloat("_Progress", progress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tvMaterial.SetFloat("_Progress", 1f); // Fully on

        // Add flickering after turning on
        elapsed = 0f;
        while (elapsed < flickerDuration)
        {
            float flickerStrength = Random.Range(0f, 0.2f);
            tvMaterial.SetFloat("_FlickerStrength", flickerStrength);
            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        tvMaterial.SetFloat("_FlickerStrength", 0f); // Turn off flicker
    }
}

