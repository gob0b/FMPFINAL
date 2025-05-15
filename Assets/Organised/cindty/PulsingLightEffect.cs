using UnityEngine;
using System.Collections;

public class PulsingLightEffect : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight; // Light to animate
    public float maxOuterAngle = 60f; // Final outer angle
    public float retractionAmount = 10f; // How much it dips before stabilizing
    public float animationSpeed = 1f; // How fast the animation plays

    [Header("Timing")]
    public float startDelay = 1f;
    public float fadeDuration = 1f;
    public float holdDuration = 1f;
    public float retractionDuration = 0.5f;
    public float finalHoldDuration = 0.5f;

    private void Start()
    {
        if (targetLight == null)
        {
            Debug.LogError("No light assigned!");
            return;
        }

        StartCoroutine(AnimateLight());
    }

    private IEnumerator AnimateLight()
    {
        targetLight.spotAngle = 0f;

        yield return new WaitForSeconds(startDelay);

        // Step 1: Fade in to max
        yield return AnimateAngle(0f, maxOuterAngle, fadeDuration);

        // Step 2: Hold max
        yield return new WaitForSeconds(holdDuration);

        // Step 3: Retract a bit
        float retractedAngle = Mathf.Clamp(maxOuterAngle - retractionAmount, 0, maxOuterAngle);
        yield return AnimateAngle(maxOuterAngle, retractedAngle, retractionDuration);

        // Step 4: Expand back to max
        yield return AnimateAngle(retractedAngle, maxOuterAngle, retractionDuration);

        // Step 5: Hold again
        yield return new WaitForSeconds(finalHoldDuration);

        // Step 6: Fade out
        yield return AnimateAngle(maxOuterAngle, 0f, fadeDuration);
    }

    private IEnumerator AnimateAngle(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            targetLight.spotAngle = Mathf.Lerp(from, to, t);
            elapsed += Time.deltaTime * animationSpeed;
            yield return null;
        }

        targetLight.spotAngle = to;
    }
}

