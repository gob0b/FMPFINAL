using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMovement : MonoBehaviour
{
    public Transform targetObject; // The GameObject to look at after camera finishes looking down.
    public List<Transform> waypoints; // List of waypoints the camera will move to.
    public float startDelay = 2f; // Delay before the camera starts looking down.
    public float lookDownDuration = 2f; // Duration for looking down.
    public float lookUpSpeed = 1f; // Speed of looking up.
    public float moveSpeed = 5f; // Speed of moving between waypoints.
    public AudioClip waypointAudio; // Audio to play when moving between waypoints.
    public AudioClip lookDownAudio; // Audio to play when looking down.
    private AudioSource audioSource;

    private bool isLookingDown = true;
    private bool isLookingUp = false;
    private bool isMovingToWaypoints = false;
    private int currentWaypointIndex = 0;
    private bool isBobbing = true; // Camera bobbing always on by default

    private Vector3 initialPosition;
    private Vector3 originalPosition;

    // Public variables to adjust camera bobbing speed and amount
    [Header("Camera Bobbing Settings")]
    public float bobbingSpeed = 10f; // Speed of bobbing (adjustable in the Inspector)
    public float bobbingAmount = 0.05f; // Amount of bobbing (adjustable in the Inspector)

    // Public variable to adjust the amount of camera look down
    [Header("Camera Look Down Settings")]
    public float lookDownAngle = -60f; // Angle to look down (adjustable in the Inspector)
    public float lookDownSpeed = 1f; // Speed of looking down (adjustable in the Inspector)

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        initialPosition = transform.position;
        originalPosition = transform.position;
        StartCoroutine(StartDelayAndLookDownUp());
    }

    void Update()
    {
        if (isLookingUp && !isMovingToWaypoints)
        {
            LookUp();
        }

        if (isMovingToWaypoints)
        {
            MoveToWaypoints();
        }

        if (isBobbing)
        {
            ApplyCameraBobbing();
        }
    }

    private IEnumerator StartDelayAndLookDownUp()
    {
        // Wait for the start delay
        yield return new WaitForSeconds(startDelay);

        // Start looking down to the desired angle
        Quaternion targetRotation = Quaternion.Euler(lookDownAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        while (isLookingDown && Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            // Smoothly transition to the look-down angle, using lookDownSpeed
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookDownSpeed);
            yield return null;
        }

        // Play the "looking down" audio when the camera starts looking down
        if (lookDownAudio != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(lookDownAudio);
        }

        // Wait for the specified duration while looking down
        yield return new WaitForSeconds(lookDownDuration);

        // Stop the "looking down" audio when the camera looks up
        if (audioSource.isPlaying && lookDownAudio != null)
        {
            audioSource.Stop();
        }

        // Start looking up
        isLookingDown = false;
        isLookingUp = true;
    }

    private void LookUp()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookUpSpeed);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            isLookingUp = false;
            LockOnToTarget();
        }
    }

    private void LockOnToTarget()
    {
        if (targetObject != null)
        {
            // Start following the target and moving to waypoints
            isMovingToWaypoints = true;
        }
    }

    private void MoveToWaypoints()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            // Move the camera to the current waypoint
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Play the waypoint audio when the camera starts moving to the next waypoint
            if (audioSource != null && waypointAudio != null && Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                audioSource.PlayOneShot(waypointAudio);
            }

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // Once the camera reaches the last waypoint, stop the audio
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            isMovingToWaypoints = false;
        }

        // Always look at the target object
        if (targetObject != null)
        {
            transform.LookAt(targetObject);
        }
    }

    private void ApplyCameraBobbing()
    {
        // Apply bobbing only when moving
        if (isMovingToWaypoints)
        {
            float time = Time.time * bobbingSpeed;
            float bobbingYOffset = Mathf.Sin(time) * bobbingAmount;
            transform.position = new Vector3(transform.position.x, originalPosition.y + bobbingYOffset, transform.position.z);
        }
    }
}


