using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PressGame : MonoBehaviour
{
    public Text countdownText;            // UI Text to show the countdown (for initial countdown)
    public Scrollbar timerScrollbar;      // Scrollbar for the timer (this will represent time)
    public Button clickButton;            // Button that the player needs to click
    public float countdownDuration = 3f;  // Countdown duration before the game starts
    public float clickTimerDuration = 10f; // Timer duration for clicking
    public int requiredClicks = 10;       // Number of times the player needs to click the button
    public string winScene = "WinScene";  // Scene to go to if player wins
    public string loseScene = "FailScene"; // Scene to go to if player loses

    private int currentClickCount = 0;    // Tracks how many times the player has clicked the button
    private bool gameEnded = false;       // Determines if the game has ended

    private void Start()
    {
        // Set the scrollbar value to 1 (full) at the start
        timerScrollbar.value = 1f;

        // Automatically start the countdown when the scene starts
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        float timeRemaining = countdownDuration;

        // Countdown before the game starts
        while (timeRemaining > 0)
        {
            countdownText.text = "Starting in: " + Mathf.Ceil(timeRemaining).ToString();
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);  // Short "GO!" display time
        countdownText.text = "";

        // Enable the button and start the timer for clicks
        clickButton.interactable = true;
        StartCoroutine(StartClickTimer());
    }

    private IEnumerator StartClickTimer()
    {
        float timeRemaining = clickTimerDuration;

        // Start the timer and update the scrollbar
        while (timeRemaining > 0 && currentClickCount < requiredClicks)
        {
            // Update the scrollbar value (as time decreases, the scrollbar's value decreases)
            timerScrollbar.value = timeRemaining / clickTimerDuration;

            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        // If the player has clicked the required number of times, win the game
        if (currentClickCount >= requiredClicks)
        {
            WinGame();
        }
        else
        {
            LoseGame();
        }
    }

    private void OnClick()
    {
        if (!gameEnded)
        {
            currentClickCount++;

            // If the required clicks are achieved, skip the remaining time and go straight to the win scene
            if (currentClickCount >= requiredClicks)
            {
                WinGame();
            }
        }
    }

    private void WinGame()
    {
        gameEnded = true;
        // Load the win scene immediately
        SceneManager.LoadScene(winScene);
    }

    private void LoseGame()
    {
        gameEnded = true;
        // Load the fail scene
        SceneManager.LoadScene(loseScene);
    }
}

