using UnityEngine;

public class LightFlickerFlick : MonoBehaviour
{
    public Light lightSource; // The light source to flicker
    public float startDelay = 1f; // Delay before the flicker starts
    public int flashCount = 5; // Number of times the light will flash
    public float flashSpeed = 0.1f; // Time between flashes
    public float intensityMin = 0f; // Minimum light intensity (light off)
    public float intensityMax = 1f; // Maximum light intensity

    private bool isFlashing = false;
    private int flashesRemaining;

    void Start()
    {
        if (lightSource == null)
        {
            lightSource = GetComponent<Light>();
        }

        // Start with the light turned off
        lightSource.intensity = intensityMin;

        // Start the flickering after the delay
        Invoke("StartFlicker", startDelay);
    }

    void StartFlicker()
    {
        flashesRemaining = flashCount;
        isFlashing = true;
        StartCoroutine(FlickerLight());
    }

    System.Collections.IEnumerator FlickerLight()
    {
        while (flashesRemaining > 0)
        {
            // Turn the light on
            lightSource.intensity = intensityMax;
            yield return new WaitForSeconds(flashSpeed);

            // Turn the light off
            lightSource.intensity = intensityMin;
            yield return new WaitForSeconds(flashSpeed);

            flashesRemaining--;
        }

        // Optionally, turn the light back to its normal intensity after flashing
        lightSource.intensity = intensityMax;
    }
}


