using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonImageSwap : MonoBehaviour
{
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
    }

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
