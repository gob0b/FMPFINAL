using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSequenceSwitcher : MonoBehaviour
{
    [Header("Image Switcher Settings")]
    public Image[] images;            // Array of images to cycle through
    public float displayTime = 2f;    // Time each image is shown before switching
    private int currentIndex = 0;     // Current image index

    private void Start()
    {
        if (images.Length > 0)
        {
            // Set all images invisible at the start
            foreach (Image img in images)
            {
                img.gameObject.SetActive(false);
            }

            // Start the image sequence with the first image visible
            images[currentIndex].gameObject.SetActive(true);
            StartCoroutine(SwitchImages());
        }
        else
        {
            Debug.LogError("Assign images in the array!");
        }
    }

    // Coroutine to cycle through images
    IEnumerator SwitchImages()
    {
        while (true) // Loop indefinitely
        {
            // Wait for the specified display time
            yield return new WaitForSeconds(displayTime);

            // Hide the current image
            images[currentIndex].gameObject.SetActive(false);

            // Move to the next image in the array
            currentIndex = (currentIndex + 1) % images.Length;

            // Show the next image
            images[currentIndex].gameObject.SetActive(true);
        }
    }
}


