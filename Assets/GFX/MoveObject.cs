using UnityEngine;

public class MoveObject : MonoBehaviour
{
    // Public variables to adjust in the inspector
    public float moveDistance = 5f;  // How far the object moves on the Z-axis
    public float moveSpeed = 2f;     // Speed at which the object moves
    public float stayTime = 2f;      // Time to stay at the end position before retracting
    public float startDelay = 1f;    // Delay before the object starts moving (in seconds)

    private Vector3 initialPosition; // The original position of the object
    private Vector3 targetPosition;  // The position where the object will move to
    private bool isMoving = false;   // Whether the object is moving or not
    private bool isRetracting = false; // Whether the object is retracting back
    private float stayTimer = 0f;    // Timer to track how long the object stays at the target position
    private float startDelayTimer = 0f; // Timer to handle the initial delay

    void Start()
    {
        // Store the initial position of the object
        initialPosition = transform.position;
        targetPosition = initialPosition + Vector3.forward * moveDistance; // Calculate target position on the Z-axis
    }

    void Update()
    {
        // If the delay has not passed, handle the delay countdown
        if (startDelayTimer < startDelay)
        {
            startDelayTimer += Time.deltaTime; // Count the delay time
            return; // Do nothing else until the delay time has passed
        }

        // If the object is not moving or retracting, start moving it forward on the Z-axis
        if (!isMoving && !isRetracting)
        {
            isMoving = true;  // Start moving the object
        }

        // Handle movement forward (along Z-axis)
        if (isMoving)
        {
            MoveObjectForward();
        }
        // Handle staying at the target position
        else if (!isRetracting && stayTimer < stayTime)
        {
            stayTimer += Time.deltaTime;  // Count time while the object stays at the target position
        }
        // Handle retracting the object back to its initial position
        else if (isRetracting)
        {
            MoveObjectBack();
        }
    }

    // Function to move the object forward along the Z-axis
    private void MoveObjectForward()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // When the object reaches the target position, stop moving and start the stay time
        if (transform.position == targetPosition)
        {
            isMoving = false;  // Stop moving
            stayTimer = 0f;    // Reset the stay timer
        }
    }

    // Function to move the object back to the initial position
    private void MoveObjectBack()
    {
        transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);

        // When the object reaches the initial position, stop retracting and reset the state
        if (transform.position == initialPosition)
        {
            isRetracting = false;  // Stop retracting
            isMoving = false;      // Stop any further movement
            stayTimer = 0f;        // Reset stay timer
        }
    }

    // Function to restart the movement cycle (forward -> stay -> retract)
    public void RestartMovement()
    {
        isMoving = true;
        isRetracting = false;
        stayTimer = 0f;
    }
}


