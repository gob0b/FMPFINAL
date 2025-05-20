using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraLookUpAndFade : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;
    public float maxLookUpAngle = 45f;

    [Header("Scene Settings")]
    public string sceneToLoad = "NextScene";

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 2f;

    private float currentAngle = 0f;
    private bool isRotating = false;
    private bool isFading = false;
    private float fadeTimer = 0f;

    public void TriggerLookUp()
    {
        if (!isRotating)
        {
            isRotating = true;
            fadeTimer = 0f;
        }
    }

    void Start()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        if (isRotating)
        {
            // Rotate
            if (currentAngle < maxLookUpAngle)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                float step = Mathf.Min(rotationStep, maxLookUpAngle - currentAngle);
                transform.Rotate(Vector3.left, step);
                currentAngle += step;
            }

            // Fade
            if (!isFading)
            {
                fadeTimer += Time.deltaTime;
                float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
                if (fadeImage != null)
                    fadeImage.color = new Color(0, 0, 0, alpha);
            }

            // Check if both finished
            if (currentAngle >= maxLookUpAngle && fadeTimer >= fadeDuration)
            {
                isFading = true;
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
