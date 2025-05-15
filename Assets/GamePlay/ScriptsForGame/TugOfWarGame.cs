using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TugOfWarGame : MonoBehaviour, IChallenge
{
    [Header("Game Settings")]
    public float timeLimit = 10f;
    public float opponentSpeed = 0.01f;

    [Header("UI Elements")]
    public Slider slider;
    public TextMeshProUGUI countdownText;
    public Scrollbar timerBar;

    [Header("Audio Settings")]
    public AudioClip spacebarPressSound;
    private AudioSource audioSource;

    private float timer;
    private float middlePoint;
    private bool gameActive = false;

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
        gameActive = false;
        slider.value = (slider.maxValue + slider.minValue) / 2;
        middlePoint = slider.value;
        slider.interactable = false;
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
        gameActive = true;
    }

    void Update()
    {
        if (!gameActive) return;

        timer -= Time.deltaTime;
        timerBar.size = Mathf.Clamp01(timer / timeLimit);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            slider.value += 0.1f;
            if (spacebarPressSound) audioSource.PlayOneShot(spacebarPressSound);
        }

        slider.value -= opponentSpeed * Time.deltaTime;

        if (slider.value >= slider.maxValue)
        {
            gameActive = false;
            gameMode.OnChallengeComplete();
        }
        else if (timer <= 0)
        {
            gameActive = false;
            if (slider.value >= middlePoint)
            {
                gameMode.OnChallengeComplete();
            }
            else
            {
                gameMode.OnChallengeFail();
            }
        }
    }
}
