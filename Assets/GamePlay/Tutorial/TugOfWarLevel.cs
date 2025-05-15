using UnityEngine;
using UnityEngine.UI;

public class TugOfWarLevel : MonoBehaviour
{
    public Slider tugSlider;
    public Button mashButton;
    public float sliderSpeed = 0.25f;        // speed it moves left automatically
    public float mashPushPower = 0.05f;      // how much it moves right per mash
    public AudioSource mashAudio;
    public TutorialGameMode gameMode;

    private bool isActive = false;

    void OnEnable()
    {
        StartLevel(); // Auto start when panel becomes visible
    }

    public void StartLevel()
    {
        isActive = true;
        tugSlider.value = 0.5f; // start in the middle

        mashButton.onClick.RemoveAllListeners();
        mashButton.onClick.AddListener(OnMashButtonPressed);
    }

    void Update()
    {
        if (!isActive) return;

        // Automatically drift to the left
        tugSlider.value -= sliderSpeed * Time.deltaTime;

        // Check if player wins (Slider is 50% or more to the right)
        if (tugSlider.value >= 1f)
        {
            isActive = false;
            gameMode.CompleteLevel(); // Complete level immediately on right win
        }
        else if (tugSlider.value <= 0f)
        {
            isActive = false;
            gameMode.FailLevel();
        }

        // If slider value reaches 50% or more to the right, immediately complete the level
        if (tugSlider.value >= 0.5f)
        {
            isActive = false;
            gameMode.CompleteLevel(); // Trigger level completion immediately
        }
    }

    void OnMashButtonPressed()
    {
        if (!isActive) return;

        tugSlider.value += mashPushPower;
        tugSlider.value = Mathf.Clamp01(tugSlider.value); // Keep it between 0 and 1

        if (mashAudio != null)
        {
            mashAudio.Play();
        }
    }
}
