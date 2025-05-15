using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIImageSwitcher : MonoBehaviour
{
    [Header("Image Settings")]
    public Image displayImage;         // The UI Image component to switch
    public Sprite[] images;            // Array of images to cycle through
    public float displayTime = 2f;     // Time each image is shown before switching
    private int currentIndex = 0;      // Current image index

    void Start()
    {
        if (images.Length > 0 && displayImage != null)
        {
            StartCoroutine(SwitchImages());
        }
        else
        {
            Debug.LogError("Assign an Image component and add sprites to the array!");
        }
    }

    IEnumerator SwitchImages()
    {
        while (true) // Loop indefinitely
        {
            displayImage.sprite = images[currentIndex]; // Change the image
            yield return new WaitForSeconds(displayTime); // Wait before switching

            currentIndex = (currentIndex + 1) % images.Length; // Cycle through images
        }
    }
}

