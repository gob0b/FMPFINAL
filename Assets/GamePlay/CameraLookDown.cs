using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraLookDownWithWaypoints : MonoBehaviour
{
    [Header("Look Down Settings")]
    public float lookDownAngleInitial = 20f;
    public float lookDownAngleMore = 40f;
    public float lookUpAngle = -10f;
    public float lookSideAngle = 15f;
    public float rotationSpeed = 1f;
    public float waitBetweenSteps = 1.5f;

    [Header("Bobbing Settings")]
    public float bobbingSpeed = 2f;
    public float bobbingAmount = 0.05f;

    [Header("Move To Waypoints")]
    public List<Transform> waypoints;              // Waypoints to move through in order
    public List<float> moveDurations;              // Duration in seconds for moving to each waypoint (must match waypoints count)
    public float defaultMoveDuration = 2f;         // Fallback duration if moveDurations is missing/shorter

    [Header("Lock Target")]
    public Transform lockTarget;

    private Quaternion baseRotation;
    private Vector3 basePosition;
    private float bobbingTimer;

    private bool sequenceDone = false;

    void Start()
    {
        baseRotation = transform.localRotation;
        basePosition = transform.localPosition;

        StartCoroutine(RotationSequence());
    }

    void Update()
    {
        // Bobbing effect always active
        bobbingTimer += Time.deltaTime * bobbingSpeed;
        float newY = basePosition.y + Mathf.Sin(bobbingTimer) * bobbingAmount;
        Vector3 bobbedPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        transform.localPosition = bobbedPosition;

        // If locked and no waypoints left, keep locking onto target
        if (sequenceDone && lockTarget != null && (waypoints == null || waypoints.Count == 0))
        {
            LockToTarget();
        }
    }

    IEnumerator RotationSequence()
    {
        yield return RotateToEulerAngles(new Vector3(lookDownAngleInitial, 0, 0));
        yield return new WaitForSeconds(waitBetweenSteps);
        yield return RotateToEulerAngles(new Vector3(lookDownAngleMore, 0, 0));
        yield return new WaitForSeconds(waitBetweenSteps);
        yield return RotateToEulerAngles(new Vector3(lookDownAngleMore, -lookSideAngle, 0));
        yield return RotateToEulerAngles(new Vector3(lookDownAngleMore, lookSideAngle, 0));
        yield return RotateToEulerAngles(new Vector3(lookDownAngleMore, 0, 0));
        yield return new WaitForSeconds(waitBetweenSteps);
        yield return RotateToEulerAngles(new Vector3(lookUpAngle, 0, 0));
        yield return new WaitForSeconds(waitBetweenSteps);
        yield return RotateToEulerAngles(new Vector3(lookUpAngle, -lookSideAngle, 0));
        yield return RotateToEulerAngles(new Vector3(lookUpAngle, lookSideAngle, 0));
        yield return RotateToEulerAngles(new Vector3(lookUpAngle, 0, 0));
        yield return new WaitForSeconds(waitBetweenSteps);

        sequenceDone = true;

        if (waypoints != null && waypoints.Count > 0)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                float duration = (moveDurations != null && i < moveDurations.Count) ? moveDurations[i] : defaultMoveDuration;
                yield return MoveToWaypointAndLock(waypoints[i], duration);
            }
        }
    }

    IEnumerator RotateToEulerAngles(Vector3 eulerAngles)
    {
        Quaternion targetRotation = baseRotation * Quaternion.Euler(eulerAngles);
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        transform.localRotation = targetRotation;
    }

    IEnumerator MoveToWaypointAndLock(Transform waypoint, float duration)
{
    Vector3 startPos = transform.position;
    Quaternion fixedRotation = transform.rotation; // keep current rotation fixed

    float elapsed = 0f;
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // Move position toward waypoint
        transform.position = Vector3.Lerp(startPos, waypoint.position, t);

        // Keep rotation fixed
        transform.rotation = fixedRotation;

        yield return null;
    }

    // Ensure final position and rotation are exact
    transform.position = waypoint.position;
    transform.rotation = fixedRotation;
}


    private void LockToTarget()
    {
        float smoothSpeed = 5f * Time.deltaTime; // Adjust smoothness here
        transform.position = Vector3.Lerp(transform.position, lockTarget.position, smoothSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, lockTarget.rotation, smoothSpeed);
    }
}
