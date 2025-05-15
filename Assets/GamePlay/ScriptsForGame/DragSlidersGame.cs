using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DragSlidersGame : MonoBehaviour
{
    [Header("Game Settings")]
    public int numberOfSliders = 3; // Number of sliders
    public float timeLimit = 10f; // Time limit in seconds

    [Header("Scene Settings")]
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";

    [Header("UI Elements")]
    public Slider[] sliders; // Assign sliders in inspector
    public Text countdownText; // UI text for countdown
    public Scrollbar timerBar; // UI Scrollbar for the timer

    [Header("Audio Settings")]
    public AudioClip sliderSound; // Sound to play when slider reaches max
    private AudioSource audioSource;

    private float timer;
    private bool gameActive = false;
    private bool[] sliderSoundPlayed;

    void Start()
    {
        timer = timeLimit;
        timerBar.size = 1f; // Ensure the timer bar starts full
        SetSlidersInteractable(false); // Disable sliders initially
        audioSource = GetComponent<AudioSource>();
        sliderSoundPlayed = new bool[sliders.Length]; // Track which sliders have played sound
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = "Starting in: " + countdown;
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        StartGame();
    }

    void StartGame()
    {
        gameActive = true;
        SetSlidersInteractable(true); // Enable sliders once the game starts
    }

    void Update()
    {
        if (gameActive)
        {
            timer -= Time.deltaTime;
            timerBar.size = Mathf.Clamp01(timer / timeLimit); // Ensure smooth countdown display

            CheckSliders();

            if (AllSlidersMax())
            {
                SceneManager.LoadScene(winScene); // Instantly load win scene
            }
            else if (timer <= 0)
            {
                SceneManager.LoadScene(loseScene); // Lose if time runs out
            }
        }
    }

    void CheckSliders()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i].value >= sliders[i].maxValue && !sliderSoundPlayed[i])
            {
                audioSource.PlayOneShot(sliderSound);
                sliderSoundPlayed[i] = true;
            }
        }
    }

    bool AllSlidersMax()
    {
        foreach (Slider slider in sliders)
        {
            if (slider.value < slider.maxValue) // Must be completely at max value
                return false;
        }
        return true;
    }

    void SetSlidersInteractable(bool state)
    {
        foreach (Slider slider in sliders)
        {
            slider.interactable = state;
        }
    }
}