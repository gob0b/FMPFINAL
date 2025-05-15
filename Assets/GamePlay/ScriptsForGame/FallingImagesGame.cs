using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class FallingImagesGame : MonoBehaviour
{
    [Header("Game Settings")]
    public float timeLimit = 10f;
    public int pressesPerButton = 2;
    public List<Button> buttons;
    public List<GameObject> images;

    [Header("UI Elements")]
    public Scrollbar timerBar;

    [Header("Audio Settings")]
    public AudioSource buttonPressSound;

    [Header("Scene Settings")]
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";

    private Dictionary<Button, int> buttonPressCounts = new Dictionary<Button, int>();
    private Dictionary<Button, GameObject> buttonImageMap = new Dictionary<Button, GameObject>();
    private float timer;
    private bool gameActive = false;
    private int fallenImages = 0;

    void Start()
    {
        timer = timeLimit;
        timerBar.size = 1f;

        for (int i = 0; i < buttons.Count && i < images.Count; i++)
        {
            Button currentButton = buttons[i];
            GameObject correspondingImage = images[i];

            buttonPressCounts[currentButton] = 0;
            buttonImageMap[currentButton] = correspondingImage;

            correspondingImage.SetActive(true); // Ensure images are initially visible

            currentButton.onClick.AddListener(() => ButtonPressed(currentButton));
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

    void ButtonPressed(Button button)
    {
        if (!gameActive || !buttonImageMap.ContainsKey(button)) return;

        if (buttonPressSound) buttonPressSound.Play();

        buttonPressCounts[button]++;
        if (buttonPressCounts[button] >= pressesPerButton)
        {
            DropImage(button);
        }
    }

    void DropImage(Button button)
    {
        if (buttonImageMap.TryGetValue(button, out GameObject image))
        {
            StartCoroutine(FallAndDestroy(image));
        }
    }

    IEnumerator FallAndDestroy(GameObject image)
    {
        float fallSpeed = 300f; // Adjust falling speed
        RectTransform rectTransform = image.GetComponent<RectTransform>();

        while (rectTransform.anchoredPosition.y > -Screen.height / 2)
        {
            rectTransform.anchoredPosition -= new Vector2(0, fallSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(image);
        fallenImages++;
        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        if (fallenImages == images.Count)
        {
            SceneManager.LoadScene(winScene);
        }
    }
}






