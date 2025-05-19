using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    private bool isPaused = false;
    private int currentMashCount = 0;
    private bool doublePointsActive = false;

    private bool goldRushActive = false;
    private float nextSpawnTime = 0f;
    private List<Arrow> activeArrows = new List<Arrow>();

    private void Start()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        arrowSpeed = initialArrowSpeed;

        comboText.gameObject.SetActive(false);
        UpdatePointsText();

        if (mashGrowImage != null)
            mashGrowImage.rectTransform.localScale = Vector3.one;

        if (mashGrowImage != null)
            mashGrowImage.gameObject.SetActive(false);
    }

    private void Update()
    {
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
        switch (dir)
        {
            case ArrowDirection.Up:
                return Input.GetKeyDown(upKey);
            case ArrowDirection.Down:
                return Input.GetKeyDown(downKey);
            case ArrowDirection.Left:
                return Input.GetKeyDown(leftKey);
            case ArrowDirection.Right:
                return Input.GetKeyDown(rightKey);
            default:
                return false;
        }
    }

    private void LoseHealth(float amount)
    {
        healthBar.value -= amount;
        if (healthBar.value <= 0)
        {
            Debug.Log("Game Over!");
            // Game over logic here
        }
        ResetCombo();
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

        switch (direction)
        {
            case ArrowDirection.Up:
                rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case ArrowDirection.Down:
                rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case ArrowDirection.Left:
                rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case ArrowDirection.Right:
                rectTransform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }
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
