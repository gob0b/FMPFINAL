using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PulseAndVanish : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseAmount = 0.05f;
    public float pulseSpeed = 2f;

    [Header("Scale Up Settings")]
    public float scaleUpTarget = 2f;
    public float scaleUpDuration = 2f;

    [Header("Visibility Settings")]
    public float visibilityDuration = 5f;

    [Header("Emission Settings")]
    public float emissionFadeInDuration = 2f;
    public Color emissionColor = Color.white;

    [Header("Global Script Pause")]
    public float globalPauseDuration = 2f;

    private Vector3 initialScale;
    private float elapsedTime;
    private Material mat;
    private Renderer rend;

    private bool started = false;
    private bool finished = false;

    private void Start()
    {
        initialScale = transform.localScale;
        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            mat = rend.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.black);
        }

        StartCoroutine(PauseAllOtherScripts());
    }

    private IEnumerator PauseAllOtherScripts()
    {
        // Find all scripts in the scene except this one
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        List<MonoBehaviour> scriptsToPause = new List<MonoBehaviour>();

        foreach (var script in allScripts)
        {
            if (script != this && script.enabled)
            {
                script.enabled = false;
                scriptsToPause.Add(script);
            }
        }

        // Wait, then resume
        yield return new WaitForSeconds(globalPauseDuration);

        foreach (var script in scriptsToPause)
        {
            if (script != null)
                script.enabled = true;
        }

        started = true;
    }

    private void Update()
    {
        if (!started || finished)
            return;

        elapsedTime += Time.deltaTime;

        // Pulse
        float pulse = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = initialScale * pulse;

        // Scale Up
        float scaleProgress = Mathf.Clamp01(elapsedTime / scaleUpDuration);
        float targetScale = Mathf.Lerp(1f, scaleUpTarget, scaleProgress);
        transform.localScale *= targetScale;

        // Emission fade in
        if (mat != null && emissionFadeInDuration > 0)
        {
            float emissionProgress = Mathf.Clamp01(elapsedTime / emissionFadeInDuration);
            mat.SetColor("_EmissionColor", emissionColor * emissionProgress);
        }

        // Hide after time
        if (elapsedTime >= visibilityDuration)
        {
            finished = true;
            gameObject.SetActive(false);
        }
    }
}

