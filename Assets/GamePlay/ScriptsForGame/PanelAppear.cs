using UnityEngine;
using UnityEngine.UI;

public class PanelAppear : MonoBehaviour
{
    public float startDelay = 2f; // Delay before panel appears

    private CanvasGroup canvasGroup;

    private void Start()
    {
        // Get or add a CanvasGroup component
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Make panel invisible at start
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Schedule panel appearance
        Invoke(nameof(ShowPanel), startDelay);
    }

    private void ShowPanel()
    {
        // Make panel fully visible
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
