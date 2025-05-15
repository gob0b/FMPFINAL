using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPanelController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject panel;        // The UI Panel to show/hide
    public GameObject textTrigger;  // The Text or trigger object

    private CanvasGroup canvasGroup;
    private bool isPointerOverText = false;
    private bool isPointerOverPanel = false;

    void Start()
    {
        // Ensure panel has a CanvasGroup for visibility and interaction control
        canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.AddComponent<CanvasGroup>();

        HidePanel();
    }

    void Update()
    {
        // If the mouse is over either the text or the panel, show the panel
        if (isPointerOverText || isPointerOverPanel)
        {
            ShowPanel();
        }
        else
        {
            HidePanel();
        }
    }

    // These handle when the mouse enters or exits the text or panel
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter == textTrigger)
        {
            isPointerOverText = true;
        }
        else if (eventData.pointerEnter == panel)
        {
            isPointerOverPanel = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter == textTrigger)
        {
            isPointerOverText = false;
        }
        else if (eventData.pointerEnter == panel)
        {
            isPointerOverPanel = false;
        }
    }

    void ShowPanel()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void HidePanel()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
