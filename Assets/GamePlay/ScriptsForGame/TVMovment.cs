using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TVMovment : MonoBehaviour
{
    public GameObject[] panels; // List of panels to cycle through
    public Image transitionImage; // Transition image for the effect
    public float[] panelDurations; // Duration for each panel
    public float transitionDuration = 0.5f; // Duration of transition effect

    private int currentPanelIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        if (panels.Length > 0 && panelDurations.Length == panels.Length)
        {
            // Deactivate all panels except the first one
            for (int i = 1; i < panels.Length; i++)
            {
                panels[i].SetActive(false);
            }
            transitionImage.gameObject.SetActive(false); // Ensure the transition image is hidden initially
            StartCoroutine(CyclePanels());
        }
        else
        {
            Debug.LogError("Ensure panels and panelDurations arrays have the same length and are not empty.");
        }
    }

    IEnumerator CyclePanels()
    {
        while (true)
        {
            yield return new WaitForSeconds(panelDurations[currentPanelIndex]); // Wait for the current panel duration
            StartCoroutine(TransitionToNextPanel()); // Transition to the next panel
        }
    }

    IEnumerator TransitionToNextPanel()
    {
        if (isTransitioning) yield break; // Prevent double transitions
        isTransitioning = true;

        // Show and fade in transition image
        transitionImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        transitionImage.color = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 0f);

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            transitionImage.color = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, Mathf.Lerp(0, 1, elapsedTime / transitionDuration));
            yield return null;
        }

        // Switch to the next panel instantly (no fade)
        panels[currentPanelIndex].SetActive(false); // Hide the current panel
        currentPanelIndex = (currentPanelIndex + 1) % panels.Length; // Move to the next panel
        panels[currentPanelIndex].SetActive(true); // Show the new panel

        // Fade out the transition image
        elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            transitionImage.color = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, Mathf.Lerp(1, 0, elapsedTime / transitionDuration));
            yield return null;
        }

        // Hide the transition image after the fade-out
        transitionImage.gameObject.SetActive(false);

        isTransitioning = false;
    }
}

