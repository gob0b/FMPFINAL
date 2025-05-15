using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float lookDownAngle = 30f;    // Angle to look down (degrees)
    public float stayDownTime = 2f;      // How long the camera stays looking down (seconds)
    public float lookUpSpeed = 1f;       // Speed of looking up back to the original position
    public float lookDownSpeed = 1f;     // Speed of looking down (adjustable)
    public float startDelay = 1f;        // Delay before the camera starts looking down (seconds)

    private float originalRotationX;     // Original X rotation of the camera
    private bool isLookingDown = false;  // Flag to check if the camera is looking down
    private bool isLookingUp = false;    // Flag to check if the camera is returning to look up
    private float timer = 0f;            // Timer to track stay down time
    private float delayTimer = 0f;       // Timer to handle the start delay
    private float currentRotationX;      // Current X rotation of the camera

    void Start()
    {
        originalRotationX = transform.rotation.eulerAngles.x;
        currentRotationX = originalRotationX;

        // Start the camera behavior automatically after the delay
        Invoke("StartLookingDown", startDelay);
    }

    void Update()
    {
        // Handle the start delay
        if (delayTimer < startDelay)
        {
            delayTimer += Time.deltaTime;
            return; // Wait until the delay is over
        }

        if (isLookingDown)
        {
            // Smoothly rotate the camera down
            currentRotationX = Mathf.Lerp(currentRotationX, lookDownAngle, Time.deltaTime * lookDownSpeed);

            transform.rotation = Quaternion.Euler(currentRotationX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            // Once the camera reaches the desired angle, stop looking down and start the timer for staying down
            if (Mathf.Abs(currentRotationX - lookDownAngle) < 0.1f)
            {
                isLookingDown = false;
                timer = 0f;
            }
        }
        else if (isLookingUp)
        {
            // Smoothly look back up to the original position
            float currentX = transform.rotation.eulerAngles.x;
            currentX = Mathf.Lerp(currentX, originalRotationX, Time.deltaTime * lookUpSpeed);
            transform.rotation = Quaternion.Euler(currentX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            // If we're close enough to the original position, stop looking up
            if (Mathf.Abs(currentX - originalRotationX) < 0.1f)
            {
                isLookingUp = false;
            }
        }

        // Handle staying down for the specified time
        if (!isLookingDown && !isLookingUp)
        {
            timer += Time.deltaTime;
            if (timer >= stayDownTime)
            {
                // Start looking up when time is over
                isLookingUp = true;
            }
        }
    }

    // This function will be called after the delay to start looking down
    void StartLookingDown()
    {
        isLookingDown = true;
        timer = 0f;
    }
}




