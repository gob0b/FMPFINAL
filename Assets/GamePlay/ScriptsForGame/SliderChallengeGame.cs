using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SliderChallengeGame : MonoBehaviour, IChallenge
{
    [Header("Game Settings")]
    public float timeLimit = 10f;
    public int requiredSwipes = 5;

    [Header("UI Elements")]
    public Slider slider;
    public TextMeshProUGUI countdownText;
    public Scrollbar timerBar;

    [Header("Audio Settings")]
    public AudioClip upSound;
    public AudioClip downSound;
    private AudioSource audioSource;

    private float timer;
    private bool gameActive = false;
    private int swipeCount = 0;
    private bool movingUp = true;

    private GameMode gameMode;

    void Awake()
    {
        gameMode = FindObjectOfType<GameMode>();
        audioSource = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        ResetGame();
        StartCoroutine(StartCountdown());
    }

    public void ResetGame()
    {
        timer = timeLimit;
        timerBar.size = 1f;
        swipeCount = 0;
        movingUp = true;
        slider.interactable = false;
        gameActive = false;
        countdownText.gameObject.SetActive(true);
        countdownText.text = "";
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

        slider.interactable = true;
        gameActive = true;
    }

    void Update()
    {
        if (!gameActive) return;

        timer -= Time.deltaTime;
        timerBar.size = Mathf.Clamp01(timer / timeLimit);

        CheckSliderMovement();

        if (swipeCount >= requiredSwipes)
        {
            gameActive = false;
            gameMode.OnChallengeComplete();
        }
        else if (timer <= 0)
        {
            gameActive = false;
            gameMode.OnChallengeFail();
        }
    }

    void CheckSliderMovement()
    {
        if (movingUp && slider.value >= slider.maxValue)
        {
            if (upSound) audioSource.PlayOneShot(upSound);
            movingUp = false;
        }
        else if (!movingUp && slider.value <= slider.minValue)
        {
            if (downSound) audioSource.PlayOneShot(downSound);
            movingUp = true;
            swipeCount++;
        }
    }
}

