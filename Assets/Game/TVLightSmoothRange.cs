using UnityEngine;

public class TVLightSmoothRange : MonoBehaviour
{
    public Light tvLight; // The Light component to modify
    public float minRange = 3f; // Minimum range of the light
    public float maxRange = 7f; // Maximum range of the light
    public float transitionSpeed = 2f; // Speed of the range transition

    private float targetRange; // The target range we are transitioning toward
    private bool increasing = true; // Track whether we're increasing or decreasing the range

    void Start()
    {
        // Ensure the light component is assigned
        if (tvLight == null)
        {
            tvLight = GetComponent<Light>();
        }

        if (tvLight == null)
        {
            Debug.LogError("No Light component found on this object.");
            return;
        }

        // Start at the minimum range
        targetRange = minRange;
    }

    void Update()
    {
        // Smoothly transition the light range
        tvLight.range = Mathf.MoveTowards(tvLight.range, targetRange, transitionSpeed * Time.deltaTime);

        // If we've reached the target range, choose a new target
        if (Mathf.Approximately(tvLight.range, targetRange))
        {
            targetRange = increasing ? maxRange : minRange;
            increasing = !increasing; // Switch direction
        }
    }
}

