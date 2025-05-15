using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour
{
    [Header("UI Settings")]
    public Text displayText;
    public string[] texts;

    [Header("Timing Settings")]
    public float startDelay = 1f;
    public float typeSpeed = 0.05f;
    public float glitchDuration = 0.1f;
    public float deleteSpeed = 0.03f;
    public float stayDuration = 1.5f;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip typingSound;

    [Header("Glitch Settings")]
    public string glitchCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";

    private int currentIndex = 0;

    void Start()
    {
        if (texts.Length > 0 && displayText != null && audioSource != null && typingSound != null)
        {
            StartCoroutine(TypeLoop());
        }
        else
        {
            Debug.LogError("Assign all required fields: UI Text, AudioSource, and Typing Sound!");
        }
    }

    IEnumerator TypeLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            string fullText = texts[currentIndex];
            displayText.text = "";
            audioSource.Play();

            for (int i = 0; i < fullText.Length; i++)
            {
                char targetChar = fullText[i];

                for (float t = 0; t < glitchDuration; t += typeSpeed)
                {
                    char glitchChar = glitchCharacters[Random.Range(0, glitchCharacters.Length)];
                    displayText.text = fullText.Substring(0, i) + glitchChar;
                    yield return new WaitForSeconds(typeSpeed);
                }

                displayText.text = fullText.Substring(0, i + 1);
                yield return new WaitForSeconds(typeSpeed);
            }

            audioSource.Stop();
            yield return new WaitForSeconds(stayDuration);

            for (int i = fullText.Length; i >= 0; i--)
            {
                displayText.text = fullText.Substring(0, i);
                yield return new WaitForSeconds(deleteSpeed);
            }

            displayText.text = "";
            yield return new WaitForSeconds(0.5f);
            currentIndex = (currentIndex + 1) % texts.Length;
        }
    }
}
