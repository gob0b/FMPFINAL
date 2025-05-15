using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTimerFade : MonoBehaviour
{
    public float timerDuration = 5f; // Time before fade starts
    public float fadeDuration = 2f;  // Fade time
    public string nextSceneName;     // Name of the next scene to load
    public Image fadeImage;          // UI Image to fade (should be black and full screen)

    private float timer;
    private bool isFading = false;

    void Start()
    {
        timer = timerDuration;
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0); // Transparent
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StartCoroutine(FadeAndLoad());
            }
        }
    }

    System.Collections.IEnumerator FadeAndLoad()
    {
        isFading = true;
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}

