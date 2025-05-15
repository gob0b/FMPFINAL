using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VolumeSquaresUI : MonoBehaviour
{
    [Header("Volume Settings")]
    public int maxVolumeLevel = 10;
    [Range(0, 10)] public int currentVolumeLevel = 5;

    [Header("Audio Sources")]
    public List<AudioSource> audioSources;

    [Header("UI References")]
    public List<Image> volumeSquares; // assign square images in order
    public Color filledColor = Color.white;
    public Color emptyColor = Color.gray;

    public Button increaseButton;
    public Button decreaseButton;

    void Start()
    {
        UpdateVolumeUI();

        increaseButton.onClick.AddListener(IncreaseVolume);
        decreaseButton.onClick.AddListener(DecreaseVolume);
    }

    void IncreaseVolume()
    {
        if (currentVolumeLevel < maxVolumeLevel)
        {
            currentVolumeLevel++;
            UpdateVolumeUI();
        }
    }

    void DecreaseVolume()
    {
        if (currentVolumeLevel > 0)
        {
            currentVolumeLevel--;
            UpdateVolumeUI();
        }
    }

    void UpdateVolumeUI()
    {
        // Update square visuals
        for (int i = 0; i < volumeSquares.Count; i++)
        {
            volumeSquares[i].color = i < currentVolumeLevel ? filledColor : emptyColor;
        }

        // Calculate volume as a float (0.0 to 1.0)
        float volumeValue = (float)currentVolumeLevel / maxVolumeLevel;

        // Apply to all audio sources
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
                source.volume = volumeValue;
        }
    }
}

