using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenBlinkEffectWithCountdown : MonoBehaviour
{
    public Image blinkImage;            // The UI Image used for the blink effect (must be a full-screen black Image)
    public float blinkSpeed = 1f;       // Speed at which the screen fades in and out (seconds)
    public int totalBlinks = 5;         // Total number of blinks before stopping
    public bool stopAfterBlinks = true; // Option to stop the effect after the specified number of blinks
    private int blinkCount = 0;         // Keeps track of how many times the screen has blinked

    public Text countdownText;          // UI Text component to display countdown
    public float countdownDuration = 5f; // Duration of the countdown before the effect starts
    public float clickTimer = 10f;      // Time available to click the button after countdown finishes

    private bool hasStartedBlinking = false; // To ensure the blinking starts after the countdown

    private void Start()
    {
        // Ensure the blink image starts as invisible
        if (blinkImage != null)
        {
            blinkImage.color = new Color(0, 0, 0, 0);  // Fully transparent
        }

        // Start the countdown
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        float timeRemaining = countdownDuration;

        // Display countdown
        while (timeRemaining > 0)
        {
            countdownText.text = "Starting in: " + Mathf.Ceil(timeRemaining).ToString();
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        // Countdown finished, start clicking timer and then blink effect
        countdownText.text = "GO!";

        // Give a short "GO!" display time
        yield return new WaitForSeconds(1f);

        countdownText.text = "";

        // Start the blink effect after countdown
        hasStartedBlinking = true;
        StartCoroutine(ClickTimer());
        StartCoroutine(BlinkEffect());
    }

    private IEnumerator ClickTimer()
    {
        float timeRemaining = clickTimer;

        while (timeRemaining > 0 && !hasStartedBlinking)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        // You can add logic here to check if the user clicked a button or performed an action
        // and provide feedback based on whether it was within the time frame.
        // Example: If you have a button, you could check for the button press and give feedback.
    }

    private IEnumerator BlinkEffect()
    {
        while (blinkCount < totalBlinks || !stopAfterBlinks)
        {
            // Fade to black (screen goes black)
            yield return FadeToAlpha(1f);

            // Wait briefly to simulate blink duration
            yield return new WaitForSeconds(0.1f);

            // Fade back to clear (screen goes back to normal)
            yield return FadeToAlpha(0f);

            // Increment the blink count
            blinkCount++;

            // Stop blinking if the total blinks have been reached and stopAfterBlinks is true
            if (blinkCount >= totalBlinks && stopAfterBlinks)
            {
                break;
            }

            // Wait for the specified blink interval before the next blink
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    private IEnumerator FadeToAlpha(float targetAlpha)
    {
        float currentAlpha = blinkImage.color.a;
        float elapsedTime = 0f;

        // Gradually change the alpha to simulate fade in/out effect
        while (elapsedTime < blinkSpeed)
        {
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, elapsedTime / blinkSpeed);
            blinkImage.color = new Color(0, 0, 0, newAlpha); // Set the new alpha value
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final alpha is exactly the target alpha
        blinkImage.color = new Color(0, 0, 0, targetAlpha);
    }
}
