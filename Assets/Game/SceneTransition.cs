using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public string sceneToLoad; // The name of the scene to load
    public float fadeDuration = 1f; // Time taken for the fade effect
    public float delayBeforeSceneLoad = 1f; // Delay before the scene changes
    public float audioDelay = 0.5f; // Delay before the transition sound plays

    [Header("UI Elements")]
    public Image fadeImage; // UI image for fading effect

    [Header("Audio Settings")]
    public AudioSource transitionAudio; // Audio source for the transition sound
    public AudioSource keyPressAudio; // Audio source to play when 'A' is pressed

    private bool isTransitioning = false;

    void Update()
    {
        // Check if the 'A' key is pressed and if a transition is not already in progress
        if (Input.GetKeyDown(KeyCode.A) && !isTransitioning)
        {
            // Play immediate key press sound
            if (keyPressAudio != null)
            {
                keyPressAudio.Play();
            }

            StartCoroutine(Transition());
        }
    }

    IEnumerator Transition()
    {
        isTransitioning = true;

        // Wait before playing the transition sound
        yield return new WaitForSeconds(audioDelay);

        if (transitionAudio != null)
        {
            transitionAudio.Play();
        }

        // Fade in effect (black screen appears)
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Wait for the specified delay before changing the scene
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}

