using UnityEngine;
using System.Collections;

public class VintageTVTurnOnFromCenter : MonoBehaviour
{
    public Material tvMaterial; // Assign the material with the shader
    public float startDelay = 2f; // Delay before flickering starts
    public float flickerDuration = 1f; // Duration for the flickering effect
    public float fullTurnOnDuration = 2f; // Time for the TV to fully turn on

    private void Start()
    {
        if (tvMaterial != null)
        {
            // Set initial shader values for progress
            tvMaterial.SetFloat("_Progress", 0f); // Start with TV off
            tvMaterial.SetFloat("_FlickerStrength", 0.2f); // Set flicker strength
        }

        // Start the turn-on effect with a delay
        StartCoroutine(TVTurnOnSequence());
    }

    private IEnumerator TVTurnOnSequence()
    {
        // Wait for the start delay before flickering starts
        yield return new WaitForSeconds(startDelay);

        // Start the flickering effect and gradually show the image
        float elapsedFlickerTime = 0f;
        while (elapsedFlickerTime < flickerDuration)
        {
            float progress = Mathf.Clamp01(elapsedFlickerTime / flickerDuration);
            tvMaterial.SetFloat("_Progress", progress); // Increase the progress to show image
            elapsedFlickerTime += Time.deltaTime;
            yield return null;
        }

        // Gradually make the TV turn on fully from the center
        float elapsedTurnOnTime = 0f;
        while (elapsedTurnOnTime < fullTurnOnDuration)
        {
            float progress = Mathf.Clamp01(elapsedTurnOnTime / fullTurnOnDuration);
            tvMaterial.SetFloat("_Progress", progress); // Fully reveal the image from the center
            elapsedTurnOnTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the TV is fully on at the end
        tvMaterial.SetFloat("_Progress", 1f);
    }
}


