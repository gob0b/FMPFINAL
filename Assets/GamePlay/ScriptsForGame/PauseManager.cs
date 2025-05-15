using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public CanvasGroup pauseCanvasGroup;
    public float fadeDuration = 0.5f;

    private bool isPaused = false;
    private bool isFading = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isFading)
        {
            if (isPaused)
                StartCoroutine(FadeOutAndResume());
            else
                StartCoroutine(FadeInAndPause());
        }
    }

    IEnumerator FadeInAndPause()
    {
        isFading = true;

        Time.timeScale = 0f; // Pause time
        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            pauseCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        pauseCanvasGroup.alpha = 1;
        pauseCanvasGroup.interactable = true;
        pauseCanvasGroup.blocksRaycasts = true;
        isPaused = true;
        isFading = false;
    }

    IEnumerator FadeOutAndResume()
    {
        isFading = true;

        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            pauseCanvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        pauseCanvasGroup.alpha = 0;
        Time.timeScale = 1f; // Resume time
        isPaused = false;
        isFading = false;
    }
}
