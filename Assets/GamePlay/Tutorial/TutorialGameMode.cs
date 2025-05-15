using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGameMode : MonoBehaviour
{
    [Header("Game Panels")]
    public GameObject[] challengePanels;
    public GameObject failPanel;

    [Header("Level Timer Settings")]
    public float levelDuration = 10f;
    public Scrollbar timerBar;

    private GameObject currentPanel = null;
    private float timer = 0f;
    private bool timerRunning = false;

    void Start()
    {
        HideAllPanels();
        StartNextPanel();
    }

    void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            timerBar.size = timer / levelDuration;

            if (timer <= 0f)
            {
                FailLevel();
            }
        }
    }

    void HideAllPanels()
    {
        foreach (var panel in challengePanels)
        {
            panel.SetActive(false);
        }

        failPanel?.SetActive(false);
        timerBar.size = 1f;
    }

    void StartNextPanel()
    {
        HideAllPanels();

        // Pick a random panel to activate
        currentPanel = challengePanels[Random.Range(0, challengePanels.Length)];
        currentPanel.SetActive(true);

        // Optionally reset/setup the challenge
        currentPanel.SendMessage("StartLevel", SendMessageOptions.DontRequireReceiver);

        // Reset and start the timer
        timer = levelDuration;
        timerRunning = true;
    }

    public void CompleteLevel()
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
            currentPanel = null;
        }

        timerRunning = false;

        StartNextPanel(); // IMMEDIATE transition to the next panel
    }

    public void FailLevel()
    {
        timerRunning = false;

        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
            currentPanel = null;
        }

        failPanel?.SetActive(true);
    }
}
