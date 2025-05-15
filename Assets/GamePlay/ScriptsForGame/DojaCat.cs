using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DojaCat : MonoBehaviour
{
    public float spawnRate = 1.0f; // Time between key spawns
    public float fallSpeed = 2.0f; // Speed at which arrows fall
    public Transform spawnPoint; // Where the arrows spawn from (top of the screen)
    public RectTransform hitZone; // The area where player needs to press keys
    public GameObject arrowPrefab; // The arrow object that falls
    public Text scoreText; // Text to display the score
    public AudioSource hitSound; // Sound for a correct key press

    private int score = 0;

    private void Start()
    {
        // Start the arrow spawning
        StartCoroutine(SpawnArrows());
    }

    private IEnumerator SpawnArrows()
    {
        while (true)
        {
            // Wait for the spawn rate before creating a new arrow
            yield return new WaitForSeconds(spawnRate);

            // Randomly choose an arrow (up, down, left, right)
            string arrowDirection = GetRandomArrowDirection();

            // Instantiate the arrow at the spawn point
            GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
            arrow.GetComponent<Arrow>().Initialize(arrowDirection, fallSpeed, hitZone, this);

            // Optionally: You can add animation or effects when an arrow is instantiated
        }
    }

    private string GetRandomArrowDirection()
    {
        // Choose a random arrow direction
        string[] directions = { "Up", "Down", "Left", "Right" };
        return directions[Random.Range(0, directions.Length)];
    }

    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }
}

public class Arrow : MonoBehaviour
{
    private string direction;
    private float fallSpeed;
    private RectTransform hitZone;
    private DojaCat gameManager;

    public Text arrowText; // Display the arrow (this can be set up to use UI text or images for arrows)

    public void Initialize(string dir, float speed, RectTransform zone, DojaCat game)
    {
        direction = dir;
        fallSpeed = speed;
        hitZone = zone;
        gameManager = game;

        // Set the direction as text or image on the arrow
        arrowText.text = direction;

        // Start the fall process
        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        // Keep moving the arrow down until it reaches the hit zone or goes off screen
        while (transform.position.y > hitZone.position.y)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            yield return null;
        }

        // Check if the arrow is in the hit zone and the player pressed the right key
        if (IsInHitZone())
        {
            // Optionally, add logic to detect the key press here
            gameManager.AddScore();
            Destroy(gameObject);
        }
        else
        {
            // Handle when arrow goes past the hit zone (optional)
            Destroy(gameObject);
        }
    }

    private bool IsInHitZone()
    {
        // You can use Input.GetKeyDown for a keypress check when the arrow is in the hit zone
        if (Input.GetKeyDown(KeyCode.UpArrow) && direction == "Up")
        {
            return true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && direction == "Down")
        {
            return true;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && direction == "Left")
        {
            return true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && direction == "Right")
        {
            return true;
        }
        return false;
    }
}
