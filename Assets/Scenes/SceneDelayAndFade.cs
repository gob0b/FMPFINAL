using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneDelayAndFade : MonoBehaviour
{
    // References to the black screen image, delay, and fade durations
    public Image blackScreen;
    public float sceneDelay = 3f; // Time to wait before starting the scene
    public float fadeDuration = 2f; // Duration for the fade effect

    private void Start()
    {
        // Ensure the black screen is visible initially
        if (blackScreen != null)
        {
            blackScreen.color = new Color(0, 0, 0, 1); // Black screen fully visible
            StartCoroutine(DelayAndFade());
        }
        else
        {
            Debug.LogError("Black screen image not assigned!");
        }
    }

    private IEnumerator DelayAndFade()
    {
        // Pause all updates in the scene before starting (stop everything temporarily)
        Time.timeScale = 0f;

        // Wait for the specified delay time while time is paused
        yield return new WaitForSecondsRealtime(sceneDelay);

        // Resume the game (unfreeze time)
        Time.timeScale = 1f;

        // Start fading out the black screen
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // Calculate the alpha value based on time
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);

            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale
            yield return null;
        }

        // Ensure the black screen is completely invisible at the end
        blackScreen.color = new Color(0, 0, 0, 0);
    }
}

