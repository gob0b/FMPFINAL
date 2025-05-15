using UnityEngine;

public class CursorFollowLimited : MonoBehaviour
{
    [SerializeField] private float maxDistance = 0.5f; // Adjustable max distance for subtle movement
    [SerializeField] private float followSpeed = 5f; // Speed of movement
    private Vector3 origin; // The object's fixed position

    void Start()
    {
        origin = transform.position; // Store the fixed position as origin
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(origin).z; // Maintain depth
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);

        // Calculate the direction from origin to mouse
        Vector3 direction = worldMousePos - origin;

        // Limit the object's movement within maxDistance
        if (direction.magnitude > maxDistance)
        {
            direction = direction.normalized * maxDistance;
        }

        // Smoothly move the object towards the target position
        transform.position = Vector3.Lerp(transform.position, origin + direction, Time.deltaTime * followSpeed);
    }

    // Public method to adjust max distance at runtime
    public void SetMaxDistance(float newDistance)
    {
        maxDistance = Mathf.Max(0, newDistance); // Ensure non-negative value
    }
}

