using UnityEngine;
using System.Collections;

public class CRTInteraction : MonoBehaviour
{
    public GameObject tvScreen; // TV screen GameObject
    public GameObject buttonPanel; // The panel that contains the button (original panel)
    public GameObject[] panels; // Array of panels that will switch between each other
    public AudioClip buttonPressSound; // Audio to play when button is pressed
    public AudioSource audioSource; // Audio source to play the sound
    public Material tvMaterial; // Material for the TV screen to apply the shader

    private int currentPanelIndex = 0; // Current panel index (starts at 0)
    private bool isOn = false; // Track if the TV is on or off
    private float powerOnTime = 0f; // Track power-on time
    private float staticStrength = 0f; // Track the strength of static noise during transitions

    // Variables for controlling retraction
    public float retractSpeed = 0.2f; // Speed of the retraction
    private Vector3 originalPosition;
    private Vector3 retractedPosition;

    private void Start()
    {
        originalPosition = transform.position;
        retractedPosition = originalPosition + new Vector3(0, 0, -1); // Adjust Z-axis for retraction
        panels[currentPanelIndex].SetActive(true); // Make sure the first panel is active at the start
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press 'E' to interact with the TV
        {
            StartCoroutine(InteractWithTV());
        }
    }

    private IEnumerator InteractWithTV()
    {
        // Retract the GameObject when interacted with
        yield return StartCoroutine(RetractObject());

        // Play the button press sound
        if (buttonPressSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonPressSound);
        }

        // If the TV is off, turn it on using a shader
        if (!isOn)
        {
            isOn = true;
            StartCoroutine(TurnOnTV());
        }
        else
        {
            // If the TV is on, switch to the next panel with a static transition
            yield return StartCoroutine(StaticTransitionAndSwitchPanel());
        }

        // Extend the GameObject back to its original position
        yield return StartCoroutine(ExtendObjectBack());
    }

    private IEnumerator RetractObject()
    {
        float timeElapsed = 0f;
        while (timeElapsed < retractSpeed)
        {
            transform.position = Vector3.Lerp(originalPosition, retractedPosition, timeElapsed / retractSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = retractedPosition; // Ensure it reaches the retracted position
    }

    private IEnumerator ExtendObjectBack()
    {
        float timeElapsed = 0f;
        while (timeElapsed < retractSpeed)
        {
            transform.position = Vector3.Lerp(retractedPosition, originalPosition, timeElapsed / retractSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition; // Ensure it reaches the original position
    }

    private IEnumerator TurnOnTV()
    {
        float powerOnTime = 0f;
        Material tvMat = tvScreen.GetComponent<Renderer>().material;

        // Start turning on TV (using a horizontal split shader effect)
        while (powerOnTime < 1f)
        {
            powerOnTime += Time.deltaTime * 2f; // Speed of the power-on effect
            tvMat.SetFloat("_PowerOn", Mathf.Lerp(0f, 1f, powerOnTime));  // Gradually reveal the screen
            yield return null;
        }
    }

    private IEnumerator StaticTransitionAndSwitchPanel()
    {
        // Start the static noise transition effect
        float transitionTime = 0f;
        Material tvMat = tvScreen.GetComponent<Renderer>().material;

        // Trigger static effect
        while (transitionTime < 1f)
        {
            transitionTime += Time.deltaTime * 2f; // Speed of static transition
            tvMat.SetFloat("_NoiseStrength", Mathf.Lerp(0f, 1f, transitionTime));  // Increase static noise strength
            yield return null;
        }

        // Switch to the next panel
        panels[currentPanelIndex].SetActive(false);

        // Increment the panel index and ensure it loops back to the first panel
        currentPanelIndex = (currentPanelIndex + 1) % panels.Length;

        // Activate the new panel
        panels[currentPanelIndex].SetActive(true);

        // Wait a moment before transitioning out of static noise
        yield return new WaitForSeconds(0.5f);

        // Fade out the static noise
        transitionTime = 0f;
        while (transitionTime < 1f)
        {
            transitionTime += Time.deltaTime * 2f; // Speed of static transition
            tvMat.SetFloat("_NoiseStrength", Mathf.Lerp(1f, 0f, transitionTime));  // Decrease static noise strength
            yield return null;
        }

        // Ensure the static effect is off
        tvMat.SetFloat("_NoiseStrength", 0f);
    }
}


