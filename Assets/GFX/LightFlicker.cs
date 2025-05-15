using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    // Public variables for adjusting the flickering effect
    public float flickerSpeed = 0.1f;  // Speed of the flickering effect (how fast it changes)
    public float flickerAmount = 0.5f; // The amount of flickering (how much it flickers between 0 and max intensity)
    public float pauseDuration = 1f;   // Duration for which the light will pause (stay off) between flickers

    private Light lightSource;  // Reference to the Light component
    private float originalIntensity;  // Store the original intensity of the light
    private float timer = 0f;   // Timer to keep track of time for flickering and pausing
    private bool isFlickering = true;  // Whether the light is currently flickering or paused

    void Start()
    {
        // Get the Light component attached to the GameObject
        lightSource = GetComponent<Light>();

        // Store the original intensity at the start
        if (lightSource != null)
        {
            originalIntensity = lightSource.intensity;
        }
        else
        {
            Debug.LogError("No Light component found on this GameObject!");
        }
    }

    void Update()
    {
        // Ensure the light source exists before modifying it
        if (lightSource != null)
        {
            // If the light is flickering, apply the flicker effect
            if (isFlickering)
            {
                // Calculate a flickering effect using Mathf.PerlinNoise for randomness
                float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * flickerAmount;
                lightSource.intensity = originalIntensity - flicker;
            }
            else
            {
                // If the light is not flickering, make the intensity 0 (light off)
                lightSource.intensity = 0f;
            }

            // Update the timer
            timer += Time.deltaTime;

            // If the timer exceeds the pause duration, toggle the flickering state
            if (timer >= pauseDuration)
            {
                // Reset the timer
                timer = 0f;

                // Toggle between flickering and pausing
                isFlickering = !isFlickering;
            }
        }
    }
}

