using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    // A struct to store the text and its display timer
    [System.Serializable]
    public class TimedText
    {
        public string text;        // The actual text to display
        public float displayTime;  // How long the text stays on screen
        public float startDelay;   // Delay before the text appears
    }

    // List of timed texts that you can populate in the inspector
    public TimedText[] timedTexts;

    // Reference to the UI Text element where the text will be shown
    public Text displayText;

    private int currentTextIndex = 0; // Keep track of which text to display

    void Start()
    {
        // Start the coroutine to show texts one by one
        if (timedTexts.Length > 0 && displayText != null)
        {
            StartCoroutine(DisplayTextSequence());
        }
    }

    // Coroutine to display each text in sequence with a delay and timer
    private IEnumerator DisplayTextSequence()
    {
        foreach (TimedText timedText in timedTexts)
        {
            // Wait for the start delay before showing the text
            yield return new WaitForSeconds(timedText.startDelay);

            // Display the text
            displayText.text = timedText.text;

            // Wait for the specified display time before hiding the text
            yield return new WaitForSeconds(timedText.displayTime);

            // Hide the text
            displayText.text = "";
        }

        // Optional: You can reset or loop the sequence by calling StartCoroutine(DisplayTextSequence()); here if needed
    }
}

