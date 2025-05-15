using UnityEngine;
using UnityEngine.UI;

public class VisibilityControl : MonoBehaviour
{
    // Public variables to control the behavior from Unity Inspector
    public Image image;                  // The UI Image to control visibility
    public AudioSource audioSource;      // The AudioSource to play sound
    public AudioClip soundClip;          // The sound clip to play
    public float delayBeforeVisible = 2f;   // Time before the image becomes visible (seconds)
    public float timeVisible = 3f;          // Time the image stays visible (seconds)
    public float fadeSpeed = 1f;            // Speed of fading in and out (can be adjusted for quicker/slower transitions)

    // Private variables for controlling the state
    private bool isVisible = false;
    private float timer = 0f;

    private void Start()
    {
        // Initially hide the image
        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        }

        // Set the audio clip
        if (audioSource != null && soundClip != null)
        {
            audioSource.clip = soundClip;
        }

        timer = 0f;
    }

    private void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        if (timer >= delayBeforeVisible && timer <= delayBeforeVisible + timeVisible)
        {
            // Make the image visible
            SetImageVisibility(true);

            // If sound is not playing, start it
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (timer > delayBeforeVisible + timeVisible)
        {
            // Make the image invisible
            SetImageVisibility(false);

            // Stop the sound if it's playing
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // Reset the timer for the next cycle
            timer = 0f;
        }
    }

    // Method to set image visibility with a fade effect
    private void SetImageVisibility(bool visible)
    {
        if (image != null)
        {
            float targetAlpha = visible ? 1f : 0f;
            float currentAlpha = image.color.a;

            // Lerp alpha value to create fade-in/fade-out effect
            float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
        }
    }
}
