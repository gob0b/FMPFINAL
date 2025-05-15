using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DragScrollbarsGame : MonoBehaviour
{
    [Header("Game Settings")]
    public int numberOfScrollbars = 3; // Number of scrollbars
    public float timeLimit = 10f; // Time limit in seconds

    [Header("Scene Settings")]
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";

    [Header("UI Elements")]
    public Scrollbar[] scrollbars; // Assign scrollbars in inspector
    public Text countdownText; // UI text for countdown
    public Scrollbar timerBar; // UI Scrollbar for the timer

    private float timer;
    private bool gameActive = false;

    void Start()
    {
        timer = timeLimit;
        timerBar.size = 1f; // Ensure the timer bar starts full
        SetScrollbarsInteractable(false); // Disable scrollbars initially
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
        SetScrollbarsInteractable(true); // Enable scrollbars once the game starts
    }

    void Update()
    {
        if (gameActive)
        {
            timer -= Time.deltaTime;
            timerBar.size = Mathf.Clamp01(timer / timeLimit); // Ensure smooth countdown display

            if (AllScrollbarsDown())
            {
                SceneManager.LoadScene(winScene); // Instantly load win scene
            }
            else if (timer <= 0)
            {
                SceneManager.LoadScene(loseScene); // Lose if time runs out
            }
        }
    }

    bool AllScrollbarsDown()
    {
        foreach (Scrollbar sb in scrollbars)
        {
            if (sb.value > 0f) // Must be completely at the bottom
                return false;
        }
        return true;
    }

    void SetScrollbarsInteractable(bool state)
    {
        foreach (Scrollbar sb in scrollbars)
        {
            sb.interactable = state;
        }
    }
}

