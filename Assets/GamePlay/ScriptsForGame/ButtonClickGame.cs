using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonClickGame : MonoBehaviour
{
    [Header("Game Settings")]
    public int requiredPresses = 10; // Number of presses needed
    public float timeLimit = 5f; // Time limit in seconds
    public float startDelay = 3f; // Countdown before the game starts

    [Header("Scene Settings")]
    public string winScene = "WinScene"; // Customizable win scene
    public string loseScene = "LoseScene"; // Customizable lose scene

    [Header("UI Elements")]
    public Scrollbar timerBar; // UI Scrollbar for the timer
    public Button pressButton; // UI Button to press
    public Text countdownText; // UI Text for countdown
    public Text pressCountText; // UI Text to display button presses

    private int currentPresses = 0;
    private float timer;
    private bool gameActive = false;

    void Start()
    {
        timer = timeLimit;
        pressButton.onClick.AddListener(PressButton);
        pressButton.interactable = false; // Disable button during countdown
        pressCountText.text = "Presses: 0 / " + requiredPresses;
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        float countdown = startDelay;
        while (countdown > 0)
        {
            countdownText.text = "" + Mathf.Ceil(countdown);
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        StartGame();
    }

    void StartGame()
    {
        gameActive = true;
        pressButton.interactable = true;
    }

    void Update()
    {
        if (gameActive)
        {
            timer -= Time.deltaTime;
            timerBar.size = timer / timeLimit; // Update scrollbar

            if (timer <= 0)
            {
                EndGame(false);
            }
        }
    }

    void PressButton()
    {
        if (!gameActive) return;

        currentPresses++;
        pressCountText.text = "Presses: " + currentPresses + " / " + requiredPresses;

        if (currentPresses >= requiredPresses)
        {
            EndGame(true);
        }
    }

    void EndGame(bool won)
    {
        gameActive = false;
        pressButton.interactable = false;
        string sceneName = won ? winScene : loseScene;
        SceneManager.LoadScene(sceneName);
    }
}

