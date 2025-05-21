using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio; // Add this at the top
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class VintageRhythmGame : MonoBehaviour
{
    [Header("Arrow Prefabs")]
    public GameObject upArrowPrefab;
    public GameObject downArrowPrefab;
    public GameObject leftArrowPrefab;
    public GameObject rightArrowPrefab;

    [Header("Spawn Points")]
    public Transform upSpawnPoint;
    public Transform downSpawnPoint;
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Gameplay Settings")]
    public float initialArrowSpeed = 200f; // Starting speed
    public float arrowSpeedIncreaseRate = 10f; // Speed increase per second
    private float arrowSpeed;

    public float spawnInterval = 1f; // Time between arrow spawns

    public Image hitZoneImage;      // Assign in inspector (the hit zone UI image)
    public Slider healthBar;        // Assign health slider in inspector
    public float maxHealth = 100f;
    public float healthLossPerMiss = 10f;

    [Header("Key Bindings")]
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.DownArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;

    [Header("Combo and Points")]
    public TextMeshProUGUI comboText;    // Assign in inspector
    public TextMeshProUGUI pointsText;   // Assign in inspector

    private int consecutiveHits = 0;
    private int points = 0;

    [Header("Mash Phase Settings")]
    public int pointsToMashPhase = 8;   // How many points needed to trigger mash phase
    public int mashTargetCount = 20;    // Mash count needed during mash phase
    public Image mashGrowImage;         // Image that grows during mash phase
    public float mashGrowAmount = 10f;  // How much the image grows per mash
    public float pauseDuration = 1f;    // Pause duration before mash phase starts

    [Header("Gold Rush Settings")]
    public int goldRushStartPoints = 10;     // Points threshold to start gold rush
    public float goldRushDuration = 10f;     // Duration of gold rush in seconds
    public float goldRushSpawnRate = 0.1f;   // Time between spawns in gold rush mode

    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // Assign your MainMixer in Inspector
    public float normalCutoff = 22000f;
    public float muffledCutoff = 500f;

    private bool isPaused = false;
    private int currentMashCount = 0;
    private bool doublePointsActive = false;

    private bool goldRushActive = false;
    private float nextSpawnTime = 0f;
    private List<Arrow> activeArrows = new List<Arrow>();

    [Header("Audio")]
    public AudioSource upKeySound;      // Assign unique AudioSource for Up Arrow
    public AudioSource downKeySound;    // Assign unique AudioSource for Down Arrow
    public AudioSource leftKeySound;    // Assign unique AudioSource for Left Arrow
    public AudioSource rightKeySound;
    public AudioSource backgroundMusic;      // Assign your background music AudioSource here
    public AudioDistortionFilter distortionFilter; // Add AudioDistortionFilter component to your music GameObject and assign here// Assign unique AudioSource for Right Arrow

    public AudioSource wrongKeySound;   // Shared AudioSource for incorrect inputs

    [Header("Game Over")]
    public GameObject gameOverPanel;  // Assign the GameOver panel in the Inspector
    public Button restartButton;      // Assign the restart button on the panel

    [Header("Countdown Settings")]
    public float startCountdownTime = 3f; // Duration of countdown before game starts
    public TextMeshProUGUI countdownText; // Assign in inspector

    [Header("Win Settings")]
    public int winScoreThreshold = 150;
    public float delayBeforeSceneChange = 3f;
    public string nextSceneName;
    public Image fadeImage; // a full-screen UI image for fade-out
    public float fadeDuration = 1.5f;

    private bool hasWon = false;

    private bool gameWon = false;             // Track if the game has been won to stop gameplay



    private void Start()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        arrowSpeed = initialArrowSpeed;
        StartCoroutine(StartCountdownRoutine());


        comboText.gameObject.SetActive(false);
        UpdatePointsText();

        if (mashGrowImage != null)
            mashGrowImage.rectTransform.localScale = Vector3.one;

        if (mashGrowImage != null)
            mashGrowImage.gameObject.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

    }

    private void Update()
    {
        if (gameWon) return;  // skip arrow spawning or updates after win

        if (gameOverPanel.activeSelf)
            return; // Completely stop game logic if game is over

        if (!hasWon && points >= winScoreThreshold)
        {
            hasWon = true;
            WinGame();
        }


        if (isPaused)
        {
            HandleMashInput();
            return;
        }

        // Gradually increase arrow speed over time (only if not gold rush)
        if (!goldRushActive)
            arrowSpeed += arrowSpeedIncreaseRate * Time.deltaTime;

        if (Time.time >= nextSpawnTime)
        {
            if (goldRushActive)
            {
                SpawnRandomArrow();
                nextSpawnTime = Time.time + goldRushSpawnRate;
            }
            else
            {
                SpawnRandomArrow();
                nextSpawnTime = Time.time + spawnInterval;
            }
        }

        for (int i = activeArrows.Count - 1; i >= 0; i--)
        {
            Arrow arrow = activeArrows[i];
            arrow.MoveUp(arrowSpeed * Time.deltaTime);

            if (arrow.IsPastHitZone(hitZoneImage.rectTransform))
            {
                LoseHealth(healthLossPerMiss);
                Destroy(arrow.gameObject);
                activeArrows.RemoveAt(i);
                ResetCombo();
            }
            else if (arrow.IsInsideHitZone(hitZoneImage.rectTransform) && CheckKeyPress(arrow.direction))
            {
                Destroy(arrow.gameObject);
                activeArrows.RemoveAt(i);
                RegisterHit();
            }
        }
    }

    void WinGame()
    {
        if (gameWon) return;  // prevent multiple triggers
        gameWon = true;

        // Stop spawning/updating arrows happens via Update() check

        // Distort the music and then start fade
        StartCoroutine(WinSequenceWithAudioDistortion());
    }
    IEnumerator WinSequenceWithAudioDistortion()
    {
        // Enable distortion effect (make sure AudioDistortionFilter component is on your music GameObject)
        if (distortionFilter != null)
        {
            distortionFilter.enabled = true;
            distortionFilter.distortionLevel = 0f;

            float targetDistortion = 0.8f; // distortion intensity to ramp up
            float duration = 2f; // duration to apply distortion

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                distortionFilter.distortionLevel = Mathf.Lerp(0f, targetDistortion, timer / duration);
                yield return null;
            }
        }
        else
        {
            // If no distortion filter, just fade out music
            float fadeOutDuration = 2f;
            float startVolume = backgroundMusic.volume;

            float t = 0f;
            while (t < fadeOutDuration)
            {
                t += Time.deltaTime;
                backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
                yield return null;
            }
            backgroundMusic.Stop();
        }

        // Now fade screen and change scene
        yield return FadeAndLoadScene();
    }
    IEnumerator FadeAndLoadScene()
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }




    IEnumerator HandleWinSequence()
    {
        yield return new WaitForSeconds(delayBeforeSceneChange);

        // Start fading out
        float t = 0;
        Color color = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }


    private IEnumerator StartCountdownRoutine()
    {
        isPaused = true;
        float countdown = startCountdownTime;
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        while (countdown > 0)
        {
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(countdown).ToString();

            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        if (countdownText != null)
        {
            countdownText.text = "GO!";
            yield return new WaitForSeconds(0.5f);
            countdownText.gameObject.SetActive(false);
        }

        isPaused = false;

        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
    }

    private void SpawnRandomArrow()
    {
        ArrowDirection dir = (ArrowDirection)Random.Range(0, 4);
        GameObject prefabToSpawn = null;
        Transform spawnPoint = null;

        switch (dir)
        {
            case ArrowDirection.Up:
                prefabToSpawn = upArrowPrefab;
                spawnPoint = upSpawnPoint;
                break;
            case ArrowDirection.Down:
                prefabToSpawn = downArrowPrefab;
                spawnPoint = downSpawnPoint;
                break;
            case ArrowDirection.Left:
                prefabToSpawn = leftArrowPrefab;
                spawnPoint = leftSpawnPoint;
                break;
            case ArrowDirection.Right:
                prefabToSpawn = rightArrowPrefab;
                spawnPoint = rightSpawnPoint;
                break;
        }

        if (prefabToSpawn != null && spawnPoint != null)
        {
            GameObject arrowObj = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity, spawnPoint.parent);
            Arrow arrow = arrowObj.AddComponent<Arrow>();
            arrow.Initialize(dir);
            activeArrows.Add(arrow);
        }
        else
        {
            Debug.LogWarning("Prefab or SpawnPoint not assigned for direction: " + dir);
        }
    }

    private bool CheckKeyPress(ArrowDirection dir)
    {
        // Detect if any directional key was pressed
        if (Input.GetKeyDown(upKey) || Input.GetKeyDown(downKey) ||
            Input.GetKeyDown(leftKey) || Input.GetKeyDown(rightKey))
        {
            bool correct = false;

            switch (dir)
            {
                case ArrowDirection.Up:
                    if (Input.GetKeyDown(upKey))
                    {
                        correct = true;
                        if (upKeySound != null) upKeySound.Play();
                    }
                    break;

                case ArrowDirection.Down:
                    if (Input.GetKeyDown(downKey))
                    {
                        correct = true;
                        if (downKeySound != null) downKeySound.Play();
                    }
                    break;

                case ArrowDirection.Left:
                    if (Input.GetKeyDown(leftKey))
                    {
                        correct = true;
                        if (leftKeySound != null) leftKeySound.Play();
                    }
                    break;

                case ArrowDirection.Right:
                    if (Input.GetKeyDown(rightKey))
                    {
                        correct = true;
                        if (rightKeySound != null) rightKeySound.Play();
                    }
                    break;
            }

            if (!correct && wrongKeySound != null)
                wrongKeySound.Play();

            return correct;
        }

        return false;
    }
    private void LoseHealth(float amount)
    {
        healthBar.value -= amount;
        if (healthBar.value <= 0)
        {
            healthBar.value = 0;
            Debug.Log("Game Over!");
            ShowGameOverPanel();
            return;
        }

        ResetCombo();
    }
    private void ShowGameOverPanel()
    {
        isPaused = true;

        foreach (var arrow in activeArrows)
        {
            if (arrow != null)
                Destroy(arrow.gameObject);
        }
        activeArrows.Clear();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            MonoBehaviour[] components = gameOverPanel.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var comp in components)
                comp.enabled = true;
        }

        if (backgroundMusic != null)
        {
            StartCoroutine(SlowAndDistortMusic());
        }
    }
    private IEnumerator SlowAndDistortMusic()
    {
        float duration = 2f;
        float timer = 0f;

        float startPitch = backgroundMusic.pitch;
        float startVolume = backgroundMusic.volume;

        while (timer < duration)
        {
            float t = timer / duration;
            backgroundMusic.pitch = Mathf.Lerp(startPitch, 0.3f, t);
            backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, t);

            timer += Time.deltaTime;
            yield return null;
        }

        backgroundMusic.Stop();
        backgroundMusic.pitch = startPitch;
        backgroundMusic.volume = startVolume;
    }


    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }



    private void RegisterHit()
    {
        consecutiveHits++;
        if (goldRushActive)
        {
            // In gold rush, every hit adds a point instantly (no combo needed)
            AddPoints(1);
        }
        else
        {
            if (consecutiveHits % 4 == 0)
            {
                ShowComboText();
                int pointsToAdd = doublePointsActive ? 2 : 1;
                AddPoints(pointsToAdd);
            }
        }
    }

    private void AddPoints(int amount)
    {
        points += amount;
        UpdatePointsText();

        if (!goldRushActive && points > 0 && points % pointsToMashPhase == 0)
        {
            StartCoroutine(StartMashPhase());
        }

        if (!goldRushActive && points >= goldRushStartPoints)
        {
            StartCoroutine(StartGoldRush());
        }
    }

    private void UpdatePointsText()
    {
        pointsText.text = "Points: " + points;
    }

    private void ShowComboText()
    {
        StopCoroutine("ComboTextRoutine");
        comboText.gameObject.SetActive(true);
        comboText.text = "COMBO!";
        StartCoroutine("ComboTextRoutine");
    }

    private IEnumerator ComboTextRoutine()
    {
        yield return new WaitForSeconds(1f);
        comboText.gameObject.SetActive(false);
    }

    private void ResetCombo()
    {
        consecutiveHits = 0;
        comboText.gameObject.SetActive(false);
    }

    private IEnumerator StartMashPhase()
    {
        isPaused = true;
        doublePointsActive = false;
        currentMashCount = 0;

        // Pause for a short duration before mash phase
        yield return new WaitForSeconds(pauseDuration);

        if (mashGrowImage != null)
        {
            mashGrowImage.rectTransform.localScale = Vector3.one;
            mashGrowImage.gameObject.SetActive(true);
        }
    }

    private void HandleMashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentMashCount++;
            if (mashGrowImage != null)
            {
                Vector3 scale = mashGrowImage.rectTransform.localScale;
                scale += new Vector3(mashGrowAmount, mashGrowAmount, 0f);
                mashGrowImage.rectTransform.localScale = scale;
            }

            if (currentMashCount >= mashTargetCount)
            {
                doublePointsActive = true;
                EndMashPhase();
            }
        }
    }

    private void EndMashPhase()
    {
        if (mashGrowImage != null)
            mashGrowImage.gameObject.SetActive(false);

        isPaused = false;
    }

    private IEnumerator StartGoldRush()
    {
        goldRushActive = true;
        comboText.text = "GOLD RUSH!";
        comboText.gameObject.SetActive(true);

        float goldRushEndTime = Time.time + goldRushDuration;
        while (Time.time < goldRushEndTime)
        {
            yield return null;
        }

        goldRushActive = false;
        comboText.gameObject.SetActive(false);

        // Clear all active arrows after gold rush ends
        foreach (var arrow in activeArrows)
        {
            if (arrow != null)
                Destroy(arrow.gameObject);
        }
        activeArrows.Clear();

        consecutiveHits = 0; // Reset combo
    }
}

public enum ArrowDirection { Up, Down, Left, Right }

public class Arrow : MonoBehaviour
{
    public ArrowDirection direction;
    private RectTransform rectTransform;

    public void Initialize(ArrowDirection dir)
    {
        direction = dir;
        rectTransform = GetComponent<RectTransform>();
        // No rotation applied; keep prefab's original look
    }


    public void MoveUp(float distance)
    {
        rectTransform.anchoredPosition += new Vector2(0, distance);
    }

    public bool IsPastHitZone(RectTransform hitZone)
    {
        return rectTransform.anchoredPosition.y > hitZone.anchoredPosition.y + hitZone.rect.height / 2f;
    }

    public bool IsInsideHitZone(RectTransform hitZone)
    {
        float yPos = rectTransform.anchoredPosition.y;
        float top = hitZone.anchoredPosition.y + hitZone.rect.height / 2f;
        float bottom = hitZone.anchoredPosition.y - hitZone.rect.height / 2f;

        return yPos >= bottom && yPos <= top;
    }
}
