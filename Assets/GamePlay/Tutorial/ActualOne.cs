using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SequencePanel
{
    public GameObject panelObject;
    public Text panelText;
    public List<KeyCode> keySequence;
    public float timeLimit;
}

public class ActualOne : MonoBehaviour
{
    public List<SequencePanel> level1Panels;
    public List<SequencePanel> level2Panels;
    public List<SequencePanel> level3Panels;

    public Scrollbar challengeTimerBar;
    public AudioSource correctKeyAudio;
    public AudioSource wrongKeyAudio;
    public GameObject failPanel;
    public Button restartButton;
    public Button loadSceneButton;
    public string sceneToLoad;

    private List<SequencePanel> activePanels = new List<SequencePanel>();
    private int completedPanelCount = 0;
    private float timer;
    private SequencePanel currentPanel;

    void Start()
    {
        failPanel.SetActive(false);
        challengeTimerBar.gameObject.SetActive(false);

        restartButton.onClick.AddListener(RestartChallenge);
        loadSceneButton.onClick.AddListener(() => SceneManager.LoadScene(sceneToLoad));

        // Start with Level 1 panels only
        activePanels.AddRange(level1Panels);
        DisableAllPanels();
        StartCoroutine(StartNextPanel());
    }

    void DisableAllPanels()
    {
        foreach (var panel in level1Panels)
        {
            panel.panelObject.SetActive(false);
            panel.panelText.gameObject.SetActive(false);
        }
        foreach (var panel in level2Panels)
        {
            panel.panelObject.SetActive(false);
            panel.panelText.gameObject.SetActive(false);
        }
        foreach (var panel in level3Panels)
        {
            panel.panelObject.SetActive(false);
            panel.panelText.gameObject.SetActive(false);
        }
    }

    IEnumerator StartNextPanel()
    {
        if (completedPanelCount == 4 && !activePanels.Contains(level2Panels[0]))
            activePanels.AddRange(level2Panels);
        if (completedPanelCount == 8 && !activePanels.Contains(level3Panels[0]))
            activePanels.AddRange(level3Panels);

        currentPanel = activePanels[Random.Range(0, activePanels.Count)];
        currentPanel.panelObject.SetActive(true);
        currentPanel.panelText.gameObject.SetActive(true);

        yield return StartCoroutine(SequenceChallenge(currentPanel));
    }

    IEnumerator SequenceChallenge(SequencePanel panel)
    {
        List<KeyCode> sequence = panel.keySequence;
        timer = panel.timeLimit;
        int currentIndex = 0;

        challengeTimerBar.gameObject.SetActive(true);
        challengeTimerBar.size = 1f;

        while (timer > 0)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(sequence[currentIndex]))
                {
                    correctKeyAudio?.Play();
                    currentIndex++;
                    if (currentIndex >= sequence.Count)
                        break;
                }
                else
                {
                    wrongKeyAudio?.Play();
                    timer -= 1f;
                }
            }

            timer -= Time.deltaTime;
            challengeTimerBar.size = Mathf.Clamp01(timer / panel.timeLimit);
            yield return null;
        }

        challengeTimerBar.gameObject.SetActive(false);
        panel.panelObject.SetActive(false);
        panel.panelText.gameObject.SetActive(false);

        if (timer > 0)
        {
            completedPanelCount++;
            StartCoroutine(StartNextPanel());
        }
        else
        {
            ShowFailPanel();
        }
    }

    void ShowFailPanel()
    {
        failPanel.SetActive(true);
    }

    void RestartChallenge()
    {
        failPanel.SetActive(false);
        completedPanelCount = 0;
        activePanels.Clear();
        activePanels.AddRange(level1Panels);
        StartCoroutine(StartNextPanel());
    }
}
