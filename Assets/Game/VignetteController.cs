using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour
{
    [Header("Post Processing Volume")]
    public Volume postProcessingVolume;  // Assign your Post-Processing Volume here

    [Header("Vignette Settings")]
    public float minIntensity = 0.2f;      // Minimum vignette intensity
    public float maxIntensity = 0.5f;      // Maximum vignette intensity
    public float speed = 2f;               // Speed of transition

    private Vignette vignette; // Reference to the vignette effect
    private float time;

    void Start()
    {
        // Get the Vignette component from the Post-Processing Volume
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out vignette))
        {
            time = 0f;
        }
        else
        {
            Debug.LogError("Vignette effect not found! Make sure your Volume has a Vignette override.");
        }
    }

    void Update()
    {
        if (vignette != null)
        {
            // Smoothly oscillate between min and max intensity
            time += Time.deltaTime * speed;
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(time, 1f));
            vignette.intensity.Override(intensity);
        }
    }
}



