using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditScroller : MonoBehaviour
{
    public TextMeshProUGUI creditsText;  // Reference to the TextMeshPro text
    public RectTransform creditsRect;    // RectTransform of the text
    public float scrollSpeed = 30f;      // Normal speed
    public float speedMultiplier = 3f;   // Speed when holding space
    public Button endButton;             // Button to show at the end

    private float startY;
    private float targetY;
    private bool finished = false;

    void Start()
    {
        if (creditsText == null || creditsRect == null || endButton == null)
        {
            Debug.LogError("Assign all references in the inspector.");
            enabled = false;
            return;
        }

        endButton.gameObject.SetActive(false);
        startY = creditsRect.anchoredPosition.y;
        float textHeight = creditsText.preferredHeight;
        float screenHeight = ((RectTransform)creditsRect.parent).rect.height;

        // Target is when entire text has passed the screen
        targetY = startY + textHeight + screenHeight;
    }

    void Update()
    {
        if (finished) return;

        float currentSpeed = scrollSpeed * (Input.GetKey(KeyCode.Space) ? speedMultiplier : 1f);
        creditsRect.anchoredPosition += Vector2.up * currentSpeed * Time.deltaTime;

        if (creditsRect.anchoredPosition.y >= targetY)
        {
            finished = true;
            endButton.gameObject.SetActive(true);
        }
    }
}
