using UnityEngine;

public class SceneTimerCameraController : MonoBehaviour
{
    [Header("Delays")]
    public float startDelay = 2f;                // Delay before everything starts

    [Header("Timer Settings")]
    public float timerDuration = 5f;             // Time before triggering animation

    [Header("Camera Movement")]
    public Transform cameraToMove;               // The camera (or GameObject) to move
    public float cameraMoveSpeed = 1f;           // Speed of movement
    public Transform cameraStopWaypoint;         // Waypoint where camera should move to

    [Header("Animation Settings")]
    public Animator animationTrigger;            // Animator to trigger when timer ends

    private float delayTimer;
    private float timer;
    private bool animationTriggered = false;
    private bool started = false;

    void Start()
    {
        delayTimer = startDelay;
        timer = timerDuration;

        if (cameraToMove == null)
        {
            Debug.LogWarning("No camera assigned to move in SceneTimerCameraController.");
        }
    }

    void Update()
    {
        // Wait until start delay is over
        if (!started)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                started = true;
            }
            return;
        }

        // Move the camera if assigned and not at waypoint
        if (cameraToMove != null && cameraStopWaypoint != null && Vector3.Distance(cameraToMove.position, cameraStopWaypoint.position) > 0.1f)
        {
            cameraToMove.position = Vector3.MoveTowards(cameraToMove.position, cameraStopWaypoint.position, cameraMoveSpeed * Time.deltaTime);
        }

        // Countdown timer and trigger animation
        if (!animationTriggered)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                animationTriggered = true;

                if (animationTrigger != null)
                {
                    animationTrigger.SetTrigger("Timer"); // Trigger animation
                }
            }
        }
    }
}
