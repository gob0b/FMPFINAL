using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class TypingChallengeGame : MonoBehaviour
{
    [Header("Game Settings")]
    public float timeLimit = 10f; // Time limit in seconds
    public List<string> requiredWords = new List<string>(); // Words to be typed in order
    public List<Sprite> wordImages = new List<Sprite>(); // Images corresponding to words

    [Header("Scene Settings")]
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";

    [Header("UI Elements")]
    public TMP_InputField inputField; // Input field for player typing
    public Image wordDisplay; // Displays the current word as an image
    public Scrollbar timerBar; // Timer displayed as a scrollbar

    [Header("Audio Settings")]
    public AudioSource correctSound;
    public AudioSource incorrectSound;

    private float timer;
    private bool gameActive = false;
    private int currentWordIndex = 0;

    void Start()
    {
        timer = timeLimit;
        timerBar.size = 1f;
        inputField.onEndEdit.AddListener(CheckWord);
        UpdateWordDisplay();
        StartCoroutine(StartCountdown());
        inputField.ActivateInputField(); // Keep input field active
    }

    IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(1f);
        gameActive = true;
    }

    void Update()
    {
        if (gameActive)
        {
            timer -= Time.deltaTime;
            timerBar.size = Mathf.Clamp01(timer / timeLimit);

            if (timer <= 0)
            {
                SceneManager.LoadScene(loseScene);
            }
        }
    }

    void CheckWord(string input)
    {
        if (!gameActive) return;

        if (input.ToLower() == requiredWords[currentWordIndex].ToLower())
        {
            if (correctSound) correctSound.Play();

            currentWordIndex++;
            inputField.text = "";

            if (currentWordIndex >= requiredWords.Count)
            {
                SceneManager.LoadScene(winScene); // Win when all words are correct
            }
            else
            {
                UpdateWordDisplay(); // Move to the next word
            }
        }
        else
        {
            if (incorrectSound) incorrectSound.Play();
            inputField.text = ""; // Clear input and retry the current word
        }

        inputField.ActivateInputField(); // Ensure the input field stays active
    }

    void UpdateWordDisplay()
    {
        if (currentWordIndex < wordImages.Count)
        {
            wordDisplay.sprite = wordImages[currentWordIndex];
        }
    }
}


