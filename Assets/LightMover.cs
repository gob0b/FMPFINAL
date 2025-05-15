using UnityEngine;

public class LightMover : MonoBehaviour
{
    // Public variables to adjust the speed and direction
    public float speed = 5f;   // Speed of the light's movement
    public Vector3 direction = Vector3.forward;  // Direction of the movement, default is forward

    // Update is called once per frame
    void Update()
    {
        // Move the light in the specified direction at the specified speed
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
