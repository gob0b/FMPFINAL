using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    // Public variables for adjusting the bobbing effect
    public float bobbingAmountVertical = 0.1f;  // Amount the camera bobs up and down
    public float bobbingAmountHorizontal = 0.1f;  // Amount the camera bobs left and right
    public float bobbingSpeedVertical = 1f;     // Speed of the vertical bobbing movement
    public float bobbingSpeedHorizontal = 1f;   // Speed of the horizontal bobbing movement

    private Vector3 originalPosition;    // The original position of the camera
    private float timerVertical = 0f;     // Timer for the vertical bobbing effect
    private float timerHorizontal = 0f;   // Timer for the horizontal bobbing effect

    void Start()
    {
        // Store the original position of the camera at the start
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Update the timer based on the time that has passed for both vertical and horizontal bobbing
        timerVertical += Time.deltaTime * bobbingSpeedVertical;
        timerHorizontal += Time.deltaTime * bobbingSpeedHorizontal;

        // Calculate the new positions based on sine waves for vertical and horizontal bobbing
        float newYPosition = originalPosition.y + Mathf.Sin(timerVertical) * bobbingAmountVertical;
        float newXPosition = originalPosition.x + Mathf.Sin(timerHorizontal) * bobbingAmountHorizontal;

        // Apply the new position to the camera, adjusting both X and Y while keeping the Z the same
        transform.localPosition = new Vector3(newXPosition, newYPosition, originalPosition.z);
    }
}

