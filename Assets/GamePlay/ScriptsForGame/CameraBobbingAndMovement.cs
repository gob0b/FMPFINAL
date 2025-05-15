using UnityEngine;
using System.Collections;

public class CameraBobbingAndMovement : MonoBehaviour
{
    // --- Bobbing Parameters ---
    [Header("Bobbing Settings")]
    public float bobbingSpeed = 0.1f;
    public float bobbingAmount = 0.2f;

    // --- Movement Settings ---
    [Header("Movement Settings")]
    public float moveAmount = 2f; // How much the camera moves in each direction
    public float rotationAmount = 10f; // How much the camera rotates
    public float moveSpeed = 1f; // Speed of movement and rotation
    public float startDelay = 1f; // Delay before starting the movement sequence
    public float audioStartDelay = 1f; // Delay before the audio starts playing

    // --- Duration Settings ---
    [Header("Movement Durations")]
    public float lookDownDuration = 1f;
    public float moveRightDuration = 1f;
    public float moveForwardDuration = 1f;
    public float moveLeftDuration = 1f;
    public float turnRightDuration = 1f;

    // --- Audio Settings ---
    [Header("Audio Settings")]
    public AudioClip audioClip; // The audio clip to play
    private AudioSource audioSource; // Reference to the AudioSource component

    // --- Private Variables ---
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Set up the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            StartCoroutine(CameraMovementSequence());
            StartCoroutine(PlayAudioWithDelay());
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is missing!");
        }
    }

    private void Update()
    {
        // Camera bobbing effect
        float bobbingY = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.localPosition = new Vector3(transform.localPosition.x, initialPosition.y + bobbingY, transform.localPosition.z);
    }

    private IEnumerator CameraMovementSequence()
    {
        // Initial delay before the movement starts
        yield return new WaitForSeconds(startDelay);

        // Movement Sequence:
        // 1. Look down for the specified duration
        yield return LookDown(lookDownDuration);

        // 2. Move right for the specified duration
        yield return MoveRight(moveRightDuration);

        // 3. Move forward for the specified duration
        yield return MoveForward(moveForwardDuration);

        // 4. Move left for the specified duration
        yield return MoveLeft(moveLeftDuration);

        // 5. Move forward again for the specified duration
        yield return MoveForward(moveForwardDuration);

        // 6. Turn right for the specified duration
        yield return TurnRight(turnRightDuration);
    }

    private IEnumerator PlayAudioWithDelay()
    {
        // Wait for the audio start delay before playing
        yield return new WaitForSeconds(audioStartDelay);
        audioSource.Play(); // Play the audio clip
    }

    private IEnumerator LookDown(float duration)
    {
        Vector3 targetRotation = new Vector3(-rotationAmount, 0, 0);
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), timeElapsed / duration);
            timeElapsed += Time.deltaTime * moveSpeed; // Adjust speed
            yield return null;
        }

        transform.rotation = Quaternion.Euler(targetRotation);
    }

    private IEnumerator MoveRight(float duration)
    {
        Vector3 targetPosition = initialPosition + transform.right * moveAmount;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime * moveSpeed; // Adjust speed
            yield return null;
        }

        transform.position = targetPosition;
    }

    private IEnumerator MoveLeft(float duration)
    {
        Vector3 targetPosition = initialPosition - transform.right * moveAmount;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime * moveSpeed; // Adjust speed
            yield return null;
        }

        transform.position = targetPosition;
    }

    private IEnumerator MoveForward(float duration)
    {
        Vector3 targetPosition = initialPosition + transform.forward * moveAmount;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime * moveSpeed; // Adjust speed
            yield return null;
        }

        transform.position = targetPosition;
    }

    private IEnumerator TurnRight(float duration)
    {
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, rotationAmount, 0);
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime * moveSpeed; // Adjust speed
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}

