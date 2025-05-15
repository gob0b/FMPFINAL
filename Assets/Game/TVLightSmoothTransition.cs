using UnityEngine;

public class TVLightSmoothTransition : MonoBehaviour
{
    public Light tvLight; // The light to apply the transition to
    public float minIntensity = 0.5f; // Minimum intensity of the light
    public float maxIntensity = 1.5f; // Maximum intensity of the light
    public float transitionSpeed = 0.5f; // Speed at which the light transitions

    private float targetIntensity; // The target intensity we are transitioning towards
    private bool increasing = true; // Flag to track whether we're increasing or decreasing intensity

    void Start()
    {
        // Ensure the light is assigned
        if (tvLight == null)
        {
            tvLight = GetComponent<Light>();
        }

        if (tvLight == null)
        {
            Debug.LogError("No Light component found on this object.");
        }

        // Set initial target intensity to the minimum
        targetIntensity = minIntensity;
    }

    void Update()
    {
        // Smoothly transition the light intensity
        tvLight.intensity = Mathf.MoveTowards(tvLight.intensity, targetIntensity, transitionSpeed * Time.deltaTime);

        // If we've reached the target intensity, choose a new target to transition towards
        if (Mathf.Approximately(tvLight.intensity, targetIntensity))
        {
            // Switch target intensity (either move to max or min)
            if (increasing)
            {
                targetIntensity = maxIntensity; // Go to max intensity
            }
            else
            {
                targetIntensity = minIntensity; // Go to min intensity
            }

            // Switch the direction of transition
            increasing = !increasing;
        }
    }
}

