using UnityEngine;
using System.Collections.Generic;

public class GameMode : MonoBehaviour
{
    public GameObject[] challengePanels; // Assign in Inspector
    public GameObject failPanel;
    private int currentIndex = -1;
    private GameObject currentPanel;

    void Start()
    {
        foreach (GameObject panel in challengePanels)
        {
            panel.SetActive(false);
        }
        failPanel.SetActive(false);
        NextPanel();
    }

    public void NextPanel()
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
            ResetPanel(currentPanel);
        }

        if (challengePanels.Length == 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, challengePanels.Length);
        } while (newIndex == currentIndex);

        currentIndex = newIndex;
        currentPanel = challengePanels[currentIndex];
        currentPanel.SetActive(true);

        // Start mini-game logic
        IChallenge challenge = currentPanel.GetComponent<IChallenge>();
        if (challenge != null)
        {
            challenge.StartGame();
        }
    }

    public void OnChallengeComplete()
    {
        NextPanel(); // move to next
    }

    public void OnChallengeFail()
    {
        foreach (GameObject panel in challengePanels)
        {
            panel.SetActive(false);
        }
        failPanel.SetActive(true);
    }

    private void ResetPanel(GameObject panel)
    {
        IChallenge challenge = panel.GetComponent<IChallenge>();
        if (challenge != null)
        {
            challenge.ResetGame();
        }
    }
}
