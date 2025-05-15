using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TimerAndVideoControl : MonoBehaviour
{
    public float timerDuration = 5f; // Time in seconds before action is triggered
    public VideoPlayer videoPlayer;  // Reference to the VideoPlayer
    public RawImage rawImage;  // Reference to the RawImage to display the video
    public RenderTexture videoTexture;  // The RenderTexture to show the video

    private float timer;

    void Start()
    {
        timer = 0f;  // Initialize timer
        rawImage.enabled = false;  // Disable image at start if you want to show video after timer

        // Ensure the VideoPlayer outputs to the RenderTexture
        videoPlayer.targetTexture = videoTexture;
    }

    void Update()
    {
        timer += Time.deltaTime;  // Increment timer

        if (timer >= timerDuration && !videoPlayer.isPlaying)
        {
            PlayVideo();  // Play the video when the timer reaches the set duration
        }
    }

    // Method to play video and enable the raw image
    void PlayVideo()
    {
        videoPlayer.Play();  // Start playing the video
        rawImage.enabled = true;  // Show the RawImage (with the video)
    }
}
