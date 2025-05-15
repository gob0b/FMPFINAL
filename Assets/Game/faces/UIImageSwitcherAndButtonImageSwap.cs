using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIImageSwitcherAndButtonImageSwap : MonoBehaviour
{
    [Header("Image Switcher Settings")]
    public Image displayImage;         // The UI Image component to switch
    public Sprite[] images;            // Array of images to cycle through
    public float displayTime = 2f;     // Time each image is shown before switching
    private int currentIndex = 0;      // Current image index

    [Header("Button Image Swap Settings")]
    public Button button;                // The Button to control
    public Image buttonImage;            // The Image component of the button
    public Sprite swappedImage;          // The image to swap with the button's image
    public Sprite originalImage;         // The original image of the button (store this so it can revert back)
    public int totalSwaps = 3;           // Number of times the image will swap before transitioning
    public float imageDisplayTime = 1f;  // How long the swapped image stays visible (seconds)
    public string sceneToLoad;           // The scene to load after the effect
    public float delayBeforeSceneLoad = 2f; // Delay before transitioning to the next scene (seconds)

    public AudioClip[] soundClips;       // Array of sound clips to play
    private int swapCount = 0;           // Counter to track how many times the image has swapped
    private AudioSource audioSource;     // AudioSource to play the sound clips

    private void Start()
    {
        // Ensure the button's original image is set
        if (buttonImage != null)
        {
            originalImage = buttonImage.sprite;
        }

        // Set up the button's onClick event
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }

        // Get the AudioSource component to play sounds
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not attached
        }

        // Start image switcher if there are images to switch
        if (images.Length > 0 && displayImage != null)
        {
            StartCoroutine(SwitchImages());
        }
        else
        {
            Debug.LogError("Assign an Image component and add sprites to the array for image switching!");
        }
    }

    // Coroutine to switch images in a loop
    IEnumerator SwitchImages()
    {
        while (true) // Loop indefinitely
        {
            displayImage.sprite = images[currentIndex]; // Change the image
            yield return new WaitForSeconds(displayTime); // Wait before switching

            currentIndex = (currentIndex + 1) % images.Length; // Cycle through images
        }
    }

    // Button click handler for swapping images and playing sounds
    private void OnButtonClick()
    {
        // Only swap the image and play sound if the swap count is less than the total number of swaps
        if (swapCount < totalSwaps)
        {
            // Start the coroutine to swap the image and play the sound
            StartCoroutine(SwapImageAndPlaySound());
        }
        else
        {
            // If swaps are done, start the scene transition after the specified delay
            StartCoroutine(TransitionToScene());
        }
    }

    // Coroutine to swap the image and play sound on button click
    private IEnumerator SwapImageAndPlaySound()
    {
        // Swap the image with the new one
        if (buttonImage != null && swappedImage != null)
        {
            buttonImage.sprite = swappedImage;
        }

        // Play the corresponding sound for the current swap
        if (soundClips != null && soundClips.Length > swapCount)
        {
            audioSource.PlayOneShot(soundClips[swapCount]); // Play the sound clip for this swap
        }

        // Wait for the specified time with the swapped image
        yield return new WaitForSeconds(imageDisplayTime);

        // Revert the image back to the original
        if (buttonImage != null && originalImage != null)
        {
            buttonImage.sprite = originalImage;
        }

        // Increase the swap count
        swapCount++;

        // Wait before the next swap if there are more swaps to go
        if (swapCount < totalSwaps)
        {
            yield return new WaitForSeconds(imageDisplayTime);
        }
    }

    // Coroutine to transition to a new scene after button swaps are complete
    private IEnumerator TransitionToScene()
    {
        // Wait for the specified delay before transitioning to the next scene
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        // Load the next scene
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
