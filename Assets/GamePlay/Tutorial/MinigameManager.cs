using UnityEngine;
using TMPro;

public class MinigameManager : MonoBehaviour
{
    [Header("Minigame Canvases")]
    public GameObject[] minigameCanvases; // Assign your 4 canvases here

    [Header("Timer Settings")]
    public float countdownDuration = 5f;
    public TextMeshProUGUI timerText;

    [Header("Game State")]
    public bool gameCompleted = false; // Set this true from your minigame scripts when finished

    private float currentTimer;
    private GameObject currentCanvas;

    private enum GameState { Waiting, Playing, Cooldown }
    private GameState currentState = GameState.Waiting;

    void Start()
    {
        StartCountdown();
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Waiting:
            case GameState.Cooldown:
                RunCountdown();
                break;

            case GameState.Playing:
                if (gameCompleted)
                {
                    EndCurrentGame();
                }
                break;
        }
    }

    void RunCountdown()
    {
        currentTimer -= Time.deltaTime;
        UpdateTimerDisplay();

        if (currentTimer <= 0)
        {
            if (currentState == GameState.Waiting)
            {
                LaunchRandomMinigame();
            }
            else if (currentState == GameState.Cooldown)
            {
                StartCountdown(); // Restart waiting timer
            }
        }
    }

    void StartCountdown()
    {
        currentTimer = countdownDuration;
        currentState = GameState.Waiting;
        UpdateTimerDisplay();
    }

    void LaunchRandomMinigame()
    {
        int index = Random.Range(0, minigameCanvases.Length);
        currentCanvas = minigameCanvases[index];
        currentCanvas.SetActive(true);
        currentState = GameState.Playing;
        timerText.text = ""; // Hide timer during gameplay
    }

    void EndCurrentGame()
    {
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false);
            currentCanvas = null;
        }

        gameCompleted = false;
        currentTimer = countdownDuration;
        currentState = GameState.Cooldown;
        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        timerText.text = Mathf.Ceil(currentTimer).ToString("0");
    }
}
