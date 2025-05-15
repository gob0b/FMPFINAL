using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BrightnessSquaresUI : MonoBehaviour
{
    [Header("Brightness Settings")]
    public int maxBrightnessLevel = 10;
    [Range(0, 10)] public int currentBrightnessLevel = 5;

    [Header("UI References")]
    public List<Image> brightnessSquares; // assign square images in order
    public Color filledColor = Color.white;
    public Color emptyColor = Color.gray;

    public Button increaseButton;
    public Button decreaseButton;

    [Header("Screen Overlay")]
    public Image brightnessOverlay; // full-screen image overlay (black with alpha)

    void Start()
    {
        UpdateBrightnessUI();

        increaseButton.onClick.AddListener(IncreaseBrightness);
        decreaseButton.onClick.AddListener(DecreaseBrightness);
    }

    void IncreaseBrightness()
    {
        if (currentBrightnessLevel < maxBrightnessLevel)
        {
            currentBrightnessLevel++;
            UpdateBrightnessUI();
        }
    }

    void DecreaseBrightness()
    {
        if (currentBrightnessLevel > 0)
        {
            currentBrightnessLevel--;
            UpdateBrightnessUI();
        }
    }

    void UpdateBrightnessUI()
    {
        // Update square visuals
        for (int i = 0; i < brightnessSquares.Count; i++)
        {
            brightnessSquares[i].color = i < currentBrightnessLevel ? filledColor : emptyColor;
        }

        // Calculate alpha for overlay (0 = no overlay, 1 = full black)
        float brightnessValue = 1f - (float)currentBrightnessLevel / maxBrightnessLevel;

        if (brightnessOverlay != null)
        {
            Color overlayColor = brightnessOverlay.color;
            overlayColor.a = brightnessValue;
            brightnessOverlay.color = overlayColor;
        }
    }
}
