using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject[] panels;
    public float transitionDuration = 1f;
    public ButtonAnimator[] buttonAnimators;
    public GameObject canvas;
    public Light sceneLight;
    public Light panelLight;
    public GameObject objectToAnimate;
    public string animationTrigger = "PlayAnimation";

    [Header("Static Effect Settings")]
    public Image staticImage;
    public Material staticMaterial;
    public float staticFadeInDuration = 0.5f;  // Adjustable fade-in duration
    public float staticFadeOutDuration = 0.5f; // Adjustable fade-out duration
    public float staticVisibleDuration = 0.5f; // Adjustable duration for how long static image is visible
    public AudioClip staticAudioClip; // Audio clip for the static effect

    [Header("Audio Settings")]
    public AudioClip buttonPressAudioClip; // AudioClip for button press
    public AudioClip lightOnAudioClip;     // AudioClip for light turning on
    public AudioClip lightOffAudioClip;    // AudioClip for light turning off
    public AudioClip pressEAudioClip;      // AudioClip for pressing E

    [Header("Emission Settings")]
    public Renderer emissionRenderer;
    public Color emissionOnColor = Color.white;
    public Color emissionOffColor = Color.black;

    [Header("Panel Light & Emission Trigger")]
    public int triggerPanelIndex = 0; // Index of the panel that triggers the light and emission
    public float flickerIntensityRange = 0.2f; // Range for flickering intensity (0 to 1)
    public float flickerSpeed = 0.1f; // Speed of the flicker effect

    private int currentPanelIndex = 0;
    private bool hasPressedE = false;

    private AudioSource audioSource;

    void Start()
    {
        // Set initial state for canvas, lights, and emission
        canvas.SetActive(false);  // Canvas is hidden initially
        sceneLight.enabled = false; // Scene light is off initially
        panelLight.enabled = false; // Panel light is off initially
        SetEmission(false); // Emission is off initially

        // Ensure only the first panel is visible at the start
        InitializePanels();

        // Set static image invisible at start
        if (staticImage != null)
        {
            staticImage.enabled = false;
        }

        // Create an AudioSource for handling the sound effects
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Press Space to trigger button animation and panel cycling
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Play the static effect
            StartCoroutine(PlayStaticTransition());

            PlayButtonAnimation();
            StartCoroutine(CyclePanels());
        }

        // Press E to toggle canvas, light, and emission
        if (Input.GetKeyDown(KeyCode.E) && !hasPressedE)
        {
            ToggleCanvasAndLight();
            PlayObjectAnimation();
            hasPressedE = true;
            PlayAudioClip(pressEAudioClip);  // Play the E press sound
        }
    }

    private void InitializePanels()
    {
        // Hide all panels initially, and show only the first one
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false); // Hide all panels
        }
        panels[0].SetActive(true); // Show the first panel
    }

    private void PlayButtonAnimation()
    {
        if (buttonAnimators.Length > currentPanelIndex && buttonAnimators[currentPanelIndex] != null)
        {
            buttonAnimators[currentPanelIndex].Buttonin();
        }
        PlayAudioClip(buttonPressAudioClip); // Play the button press sound
    }

    private IEnumerator CyclePanels()
    {
        // Make sure the static image is visible during transition
        if (staticImage != null)
        {
            staticImage.enabled = true;
            yield return FadeImage(staticImage, 0f, 1f, staticFadeInDuration); // Use adjustable fade-in duration
        }

        panels[currentPanelIndex].SetActive(false);

        // Cycle to the next panel
        currentPanelIndex = (currentPanelIndex + 1) % panels.Length;
        panels[currentPanelIndex].SetActive(true);

        // Handle light and emission when switching to the new panel
        HandleLightAndEmission();

        // Fade out the static image after transitioning to the new panel
        if (staticImage != null)
        {
            yield return new WaitForSeconds(staticVisibleDuration); // Wait for the adjustable visible duration
            yield return FadeImage(staticImage, 1f, 0f, staticFadeOutDuration); // Use adjustable fade-out duration
            staticImage.enabled = false;
        }
    }

    private void HandleLightAndEmission()
    {
        bool isTriggerPanel = currentPanelIndex == triggerPanelIndex;

        // Handle turning off light and emission before switching panels
        if (panelLight.enabled)
        {
            // Check if the lightOnAudio is playing and stop it if it is
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // Stop the "on" audio
                PlayAudioClip(lightOffAudioClip); // Play the "off" audio
            }
        }

        // Enable panel light and emission for the triggered panel
        panelLight.enabled = isTriggerPanel;
        SetEmission(isTriggerPanel);

        // Play the "on" audio if the panel is the trigger panel
        if (isTriggerPanel)
        {
            PlayAudioClip(lightOnAudioClip); // Play the "on" audio
        }
    }

    private void SetEmission(bool isOn)
    {
        if (emissionRenderer != null)
        {
            Material mat = emissionRenderer.material;
            mat.SetColor("_EmissionColor", isOn ? emissionOnColor : emissionOffColor);
            if (isOn)
            {
                mat.EnableKeyword("_EMISSION");
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    private IEnumerator FlickerLight()
    {
        float time = 0f;
        while (panelLight.enabled)
        {
            // Create random flicker intensity
            float flickerIntensity = 1f - Random.Range(0f, flickerIntensityRange);
            panelLight.intensity = flickerIntensity;

            // Wait for a short time before changing intensity again
            time += Time.deltaTime;
            if (time >= flickerSpeed)
            {
                time = 0f;
            }
            yield return null;
        }

        // Ensure the light returns to normal intensity when flickering stops
        panelLight.intensity = 1f;
    }

    private IEnumerator FadeImage(Image img, float startAlpha, float endAlpha, float duration)
    {
        float timeElapsed = 0f;
        Color startColor = img.color;
        startColor.a = startAlpha;
        img.color = startColor;

        Color endColor = img.color;
        endColor.a = endAlpha;

        while (timeElapsed < duration)
        {
            img.color = Color.Lerp(startColor, endColor, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        img.color = endColor;
    }

    private void ToggleCanvasAndLight()
    {
        // Toggle canvas visibility and turn on the light and emission
        canvas.SetActive(true);  // Show the canvas
        sceneLight.enabled = true;  // Turn on the scene light
        panelLight.enabled = true;  // Turn on the panel light
        SetEmission(true);  // Turn on the emission

        // Ensure only the first panel is visible when the canvas is toggled on
        InitializePanels();

        // Play the light on audio when E is pressed
        PlayAudioClip(lightOnAudioClip); // Play the light "on" sound
    }

    private void PlayObjectAnimation()
    {
        if (objectToAnimate != null)
        {
            Animator animator = objectToAnimate.GetComponent<Animator>();
            animator?.SetTrigger(animationTrigger);
        }
    }

    private IEnumerator PlayStaticTransition()
    {
        // Show the static image when Space is pressed
        if (staticImage != null)
        {
            staticImage.enabled = true;

            // Play static effect audio
            if (staticAudioClip != null)
            {
                audioSource.PlayOneShot(staticAudioClip);
            }

            yield return FadeImage(staticImage, 0f, 1f, staticFadeInDuration);  // Fade in static effect
            yield return new WaitForSeconds(staticVisibleDuration); // Wait for the adjustable visible duration
            yield return FadeImage(staticImage, 1f, 0f, staticFadeOutDuration);  // Fade out static effect
            staticImage.enabled = false;

            // Stop static audio after the static image is hidden
            audioSource.Stop();
        }
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip); // Play the audio clip
        }
    }
}



































