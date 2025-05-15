using System.Collections;
using UnityEngine;

public class LightVisibilityController : MonoBehaviour
{
    [Header("Light Groups")]
    public Light[] lightsVisibleAtStart;        // These start ON, then turn OFF
    public Light[] lightsInvisibleAtStart;      // These start OFF, turn ON later, flicker, then turn OFF

    [Header("Light Intensities")]
    public float invisibleLightsMaxIntensity = 1f; // Max intensity for invisible lights when they turn on

    [Header("Emission Settings")]
    public GameObject[] emissionObjects;        // Objects with emission
    public float emissionOffTime = 8f;          // When to turn off emission (independent of lights)

    [Header("Particle Effects")]
    public ParticleSystem[] sparkParticles;     // Particle effects triggered when lights turn off

    [Header("Timing")]
    public float visibleLightsOffTime = 5f;     // When to turn off lightsVisibleAtStart
    public float invisibleLightsOnTime = 7f;    // When to turn on and flicker lightsInvisibleAtStart
    public float invisibleLightsOffTime = 12f;  // When to turn off lightsInvisibleAtStart

    [Header("Flicker Settings")]
    public float flickerDuration = 3f;          // Duration of flickering before staying ON
    public float flickerInterval = 0.1f;        // How fast lights flicker
    public float flickerIntensity = 1f;         // Max flicker intensity

    private void Start()
    {
        // Ensure visible lights and emission objects are on at start
        ToggleLights(lightsVisibleAtStart, true);
        ToggleLights(lightsInvisibleAtStart, false);
        SetEmission(emissionObjects, true);

        // Ensure invisible lights start at 0 intensity
        foreach (Light light in lightsInvisibleAtStart)
        {
            if (light != null)
                light.intensity = 0f;
        }

        // Schedule transitions
        StartCoroutine(HandleVisibleLightsOff());
        StartCoroutine(HandleInvisibleLightsOn());
        StartCoroutine(HandleInvisibleLightsOff());
        StartCoroutine(HandleEmissionOff());  // New coroutine to control emission separately
    }

    // --- Instantly turn off visible lights, disable emission, and trigger sparks ---
    private IEnumerator HandleVisibleLightsOff()
    {
        yield return new WaitForSeconds(visibleLightsOffTime);
        ToggleLights(lightsVisibleAtStart, false);
        TriggerParticleEffects();
    }

    // --- Instantly turn on invisible lights, then flicker them ---
    private IEnumerator HandleInvisibleLightsOn()
    {
        yield return new WaitForSeconds(invisibleLightsOnTime);
        ToggleLights(lightsInvisibleAtStart, true, invisibleLightsMaxIntensity);
        StartCoroutine(FlickerLights(lightsInvisibleAtStart));
    }

    // --- Instantly turn off invisible lights ---
    private IEnumerator HandleInvisibleLightsOff()
    {
        yield return new WaitForSeconds(invisibleLightsOffTime);
        ToggleLights(lightsInvisibleAtStart, false);
    }

    // --- NEW: Turn off emission at a specific time ---
    private IEnumerator HandleEmissionOff()
    {
        yield return new WaitForSeconds(emissionOffTime);
        SetEmission(emissionObjects, false);
    }

    // --- Flicker effect for lights ---
    private IEnumerator FlickerLights(Light[] lights)
    {
        float timer = 0f;
        while (timer < flickerDuration)
        {
            foreach (Light light in lights)
            {
                if (light != null)
                {
                    light.enabled = Random.value > 0.5f;
                    light.intensity = Random.Range(0.5f, flickerIntensity);
                }
            }
            timer += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        // Ensure lights stay on after flickering
        ToggleLights(lights, true, invisibleLightsMaxIntensity);
    }

    // --- Instantly turn lights on or off ---
    private void ToggleLights(Light[] lights, bool state, float intensity = -1f)
    {
        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.enabled = state;

                // Only change intensity if a specific value is given
                if (intensity >= 0f)
                    light.intensity = intensity;
            }
        }
    }

    // --- Enable or disable emission ---
    private void SetEmission(GameObject[] objects, bool enable)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null && renderer.material.HasProperty("_EmissionColor"))
                {
                    renderer.material.EnableKeyword("_EMISSION");
                    Color emissionColor = enable ? Color.white * 1f : Color.black;
                    renderer.material.SetColor("_EmissionColor", emissionColor);
                }
            }
        }
    }

    // --- Trigger spark particles when visible lights turn off ---
    private void TriggerParticleEffects()
    {
        foreach (ParticleSystem ps in sparkParticles)
        {
            if (ps != null)
            {
                ps.Play();
            }
        }
    }
}
