using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
public class LightFlickerTension : MonoBehaviour
{
    [Header("Flicker Intensity Settings")]
    public float minFlickerDelay = 0.1f;
    public float maxFlickerDelay = 0.5f;

    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float maxFlickerIntensity = 2.5f;

    [Header("Tension Control")]
    [Range(0f, 1f)]
    public float tensionLevel = 0f;

    [Header("Light Lifetime")]
    public float lightLifetime = 10f;
    private float lightTimer = 0f;
    private bool lightIsDead = false;

    [Header("Shutdown Sound")]
    public AudioClip shutdownSound;

    [Header("Secondary Sound (Optional)")]
    public AudioClip secondarySound;
    public float secondarySoundDelay = 0.5f; // Time to wait before playing the second sound

    [Header("Emissive Materials (Optional)")]
    public Renderer[] emissiveObjects;

    private Light lightSource;
    private AudioSource audioSource;
    private float nextFlickerTime;

    private bool secondarySoundPlayed = false; // Track if secondary sound has been played

    void Start()
    {
        lightSource = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
        ScheduleNextFlicker();
    }

    void Update()
    {
        if (lightIsDead) return;

        lightTimer += Time.deltaTime;

        if (lightTimer >= lightLifetime)
        {
            ShutDownLight();
            return;
        }

        if (Time.time >= nextFlickerTime)
        {
            Flicker();
            ScheduleNextFlicker();
        }
    }

    void Flicker()
    {
        float flickerMin = Mathf.Lerp(minIntensity, maxIntensity, 1 - tensionLevel);
        float flickerMax = Mathf.Lerp(maxIntensity, maxFlickerIntensity, tensionLevel);

        lightSource.intensity = Random.Range(flickerMin, flickerMax);
    }

    void ScheduleNextFlicker()
    {
        float delay = Mathf.Lerp(maxFlickerDelay, minFlickerDelay, tensionLevel);
        nextFlickerTime = Time.time + Random.Range(delay * 0.5f, delay * 1.5f);
    }

    void ShutDownLight()
    {
        lightSource.enabled = false;
        lightIsDead = true;

        // Play shutdown sound
        if (shutdownSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shutdownSound);
        }

        // Schedule secondary sound if it exists
        if (secondarySound != null && !secondarySoundPlayed)
        {
            secondarySoundPlayed = true; // Make sure it only plays once
            Invoke(nameof(PlaySecondarySound), secondarySoundDelay);
        }

        // Turn off emission on assigned objects
        if (emissiveObjects != null)
        {
            foreach (Renderer rend in emissiveObjects)
            {
                foreach (Material mat in rend.materials)
                {
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.DisableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }
    }

    void PlaySecondarySound()
    {
        if (audioSource != null && secondarySound != null)
        {
            audioSource.PlayOneShot(secondarySound);
        }
    }
}
