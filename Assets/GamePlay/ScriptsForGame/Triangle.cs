using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Triangle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("The image to show/hide on hover")]
    public Image targetImage;

    private void Start()
    {
        if (targetImage != null)
        {
            targetImage.enabled = false; // Make sure it's hidden at the start
        }
        else
        {
            Debug.LogWarning("No targetImage assigned to Triangle script on " + gameObject.name);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.enabled = false;
        }
    }
}
