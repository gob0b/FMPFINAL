using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SliderFallingImagesGame : MonoBehaviour
{
    [Header("Game Settings")]
    public float timeLimit = 10f;
    public float fallSpeed = 300f;
    public List<GameObject> images;

    [Header("UI Elements")]
    public Scrollbar timerBar;
    public Slider controlSlider;

    [Header("Scene Settings")]
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";

    private float timer;
    private bool gameActive = false;
    private bool imagesStartedFalling = false;
    private int fallenImages = 0;

    void Start()
    {
        timer = timeLimit;
        timerBar.size = 1f;
        controlSlider.onValueChanged.AddListener(OnSliderMoved);

        foreach (GameObject image in images)
        {
            image.SetActive(true); // Ensure images are initially visible
        }

        StartCoroutine(StartCountdown());
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

    void OnSliderMoved(float value)
    {
        if (!gameActive || imagesStartedFalling) return;

        if (value >= controlSlider.maxValue)
        {
            imagesStartedFalling = true;
            StartCoroutine(FallAndDestroyImages());
        }
    }

    IEnumerator FallAndDestroyImages()
    {
        foreach (GameObject image in images)
        {
            RectTransform rectTransform = image.GetComponent<RectTransform>();

            while (rectTransform.anchoredPosition.y > -Screen.height / 2)
            {
                rectTransform.anchoredPosition -= new Vector2(0, fallSpeed * Time.deltaTime);
                yield return null;
            }

            Destroy(image);
            fallenImages++;

            if (fallenImages == images.Count)
            {
                SceneManager.LoadScene(winScene);
            }
        }
    }
}
