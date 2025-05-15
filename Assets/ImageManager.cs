using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{
    // A struct to store the image and its display timer
    [System.Serializable]
    public class TimedImage
    {
        public Sprite image;          // The image to display
        public float displayTime;     // How long the image stays on screen
        public float startDelay;      // Delay before the image appears
    }

    // List of timed images that you can populate in the inspector
    public TimedImage[] timedImages;

    // Reference to the UI Image component where the images will be shown
    public Image displayImage;

    void Start()
    {
        // Start the coroutine to show images one by one
        if (timedImages.Length > 0 && displayImage != null)
        {
            StartCoroutine(DisplayImageSequence());
        }
    }

    // Coroutine to display each image in sequence with a delay and timer
    private IEnumerator DisplayImageSequence()
    {
        foreach (TimedImage timedImage in timedImages)
        {
            // Wait for the start delay before showing the image
            yield return new WaitForSeconds(timedImage.startDelay);

            // Display the image (new image will be placed on top of the previous one)
            displayImage.sprite = timedImage.image;
            displayImage.enabled = true; // Enable the Image component to make the image visible

            // Wait for the specified display time before showing the next image
            yield return new WaitForSeconds(timedImage.displayTime);
        }

        // Optionally, you can restart the sequence if needed
        // StartCoroutine(DisplayImageSequence()); // Uncomment if you want it to loop
    }
}


