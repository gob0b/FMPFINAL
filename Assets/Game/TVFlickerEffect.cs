using UnityEngine;

public class TVFlickerEffect : MonoBehaviour
{
    public Light tvLight; // The light to apply the flickering effect to
    public float minFlickerIntensity = 0.5f; // Minimum intensity of the light
    public float maxFlickerIntensity = 1.5f; // Maximum intensity of the light
    public float flickerSpeed = 0.1f; // Speed of the flickering effect

    private float targetIntensity; // Target intensity value for the light
    private float timeElapsed;

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

        // Set the initial target intensity
        targetIntensity = tvLight.intensity;
    }

    void Update()
    {
        // Increment timeElapsed to control the flicker frequency
        timeElapsed += Time.deltaTime;

        // If the flicker speed interval has passed
        if (timeElapsed >= flickerSpeed)
        {
            // Change the target intensity randomly within the range
            targetIntensity = Random.Range(minFlickerIntensity, maxFlickerIntensity);

            // Reset the time counter
            timeElapsed = 0f;
        }

        // Apply the flickering effect by interpolating between the current and target intensity
        tvLight.intensity = Mathf.Lerp(tvLight.intensity, targetIntensity, Time.deltaTime * 10f);
    }
}

