using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlidingGame : MonoBehaviour
{
    public Slider slider;
    public Scrollbar timerBar;
    public float gameTime = 10f;
    public string winScene;
    public string loseScene;
    public AudioSource audioSource;
    public AudioClip slideSound;
    public GameObject[] fallingImages;
    public float fallSpeed = 3f;

    private float timeLeft;
    private bool gameActive = true;
    private int slideCount = 0;
    private int currentImageIndex = 0;
    private bool reachedTop = false;
    private bool reachedBottom = false;

    void Start()
    {
        timeLeft = gameTime;
        slider.onValueChanged.AddListener(OnSliderMove);
        timerBar.size = 1f;
        timerBar.interactable = false;
    }

    void Update()
    {
        if (gameActive)
        {
            timeLeft -= Time.deltaTime;
            timerBar.size = timeLeft / gameTime;

            if (timeLeft <= 0)
            {
                EndGame(false);
            }
        }
    }

    void OnSliderMove(float value)
    {
        if (!gameActive) return;

        if (value >= slider.maxValue)
        {
            reachedTop = true;
        }
        else if (value <= slider.minValue)
        {
            reachedBottom = true;
        }

        if (reachedTop && reachedBottom)
        {
            TriggerImageFall();
            reachedTop = false;
            reachedBottom = false;
        }
    }

    void TriggerImageFall()
    {
        if (currentImageIndex < fallingImages.Length)
        {
            GameObject image = fallingImages[currentImageIndex];
            if (image.GetComponent<Rigidbody2D>() == null)
            {
                Rigidbody2D rb = image.AddComponent<Rigidbody2D>();
                rb.gravityScale = fallSpeed;
                image.AddComponent<FallingImage>();
            }
            currentImageIndex++;
        }

        if (audioSource && slideSound)
        {
            audioSource.PlayOneShot(slideSound);
        }

        if (currentImageIndex >= fallingImages.Length)
        {
            EndGame(true);
        }
    }

    void EndGame(bool won)
    {
        gameActive = false;
        SceneManager.LoadScene(won ? winScene : loseScene);
    }
}

public class FallingImage : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -Screen.height)
        {
            Destroy(gameObject);
        }
    }
}



