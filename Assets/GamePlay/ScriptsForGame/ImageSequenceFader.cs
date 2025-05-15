using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSequenceFader : MonoBehaviour
{
    public Image[] images; // Set the images to display
    public int repeatCount = 3; // Number of times the sequence repeats
    public float displayTime = 2f; // Time each image stays visible
    public float transitionTime = 1f; // Time it takes to fade between images
    public float startDelay = 0f; // Delay before sequence starts
    public float sequenceDelay = 2f; // Delay before the next sequence starts
    public float lastImageReturnDelay = 3f; // Time before last image re-enters the sequence

    private void Start()
    {
        foreach (Image img in images)
        {
            Color tempColor = img.color;
            tempColor.a = 0;
            img.color = tempColor; // Ensure all images start fully transparent
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(startDelay);

        for (int cycle = 0; cycle < repeatCount; cycle++)
        {
            for (int i = 0; i < images.Length; i++)
            {
                yield return StartCoroutine(FadeImage(images[i], 0f, 1f, transitionTime));
                yield return new WaitForSeconds(displayTime);

                yield return StartCoroutine(FadeImage(images[i], 1f, 0f, transitionTime));

                if (i == images.Length - 1) // If it's the last image, wait before restarting sequence
                {
                    yield return new WaitForSeconds(lastImageReturnDelay);
                }
            }

            yield return new WaitForSeconds(sequenceDelay); // Delay before the next sequence starts
        }
    }

    private IEnumerator FadeImage(Image img, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color tempColor = img.color;

        while (elapsedTime < duration)
        {
            tempColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            img.color = tempColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tempColor.a = endAlpha;
        img.color = tempColor;
    }
}



