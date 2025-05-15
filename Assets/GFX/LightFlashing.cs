using UnityEngine;

public class LightFlashing : MonoBehaviour
{
    // Public variables for adjusting the flashing effect
    public float onDuration = 1f;   // Duration for which the light stays on
    public float offDuration = 1f;  // Duration for which the light stays off
    public float startDelay = 2f;   // Delay before the light starts flashing (in seconds)

    private Light lightSource;      // Reference to the Light component
    private float timer = 0f;       // Timer to track the time for on/off durations
    private bool isLightOn = false; // Whether the light is currently on or off
    private bool hasStartedFlashing = false; // Whether the light has started the flashing cycle
    private bool isFlashing = false; // Whether the light is flashing or not
    private float startDelayTimer = 0f; // Timer to track the delay before starting the flashing

    void Start()
    {
        // Get the Light component attached to the GameObject
        lightSource = GetComponent<Light>();

        // Check if the light component is present
        if (lightSource == null)
        {
            Debug.LogError("No Light component found on this GameObject!");
        }

        // Ensure light starts off
        lightSource.enabled = false;
    }

    void Update()
    {
        // Ensure the light source exists before modifying it
        if (lightSource != null)
        {
            // Handle start delay logic (before flashing starts)
            if (!hasStartedFlashing)
            {
                startDelayTimer += Time.deltaTime;

                // Once the start delay is over, start the flashing cycle
                if (startDelayTimer >= startDelay)
                {
                    hasStartedFlashing = true;
                    isFlashing = true; // Start the flashing process
                    lightSource.enabled = true;  // Turn the light on initially
                    timer = 0f;  // Reset the timer
                }
            }

            // If the flashing cycle has started, handle the on/off durations
            if (isFlashing)
            {
                timer += Time.deltaTime;

                // If the light is on and the on duration is reached, turn it off
                if (isLightOn && timer >= onDuration)
                {
                    lightSource.enabled = false;  // Turn the light off
                    isLightOn = false;
                    timer = 0f;  // Reset the timer
                }
                // If the light is off and the off duration is reached, turn it on
                else if (!isLightOn && timer >= offDuration)
                {
                    lightSource.enabled = true;  // Turn the light on
                    isLightOn = true;
                    timer = 0f;  // Reset the timer
                }
            }
        }
    }

    // Optionally, you can add a method to toggle flashing on or off (for extra control)
    public void ToggleFlashing(bool shouldFlash)
    {
        isFlashing = shouldFlash;
        lightSource.enabled = shouldFlash;  // Turn the light on if flashing is enabled
    }
}

