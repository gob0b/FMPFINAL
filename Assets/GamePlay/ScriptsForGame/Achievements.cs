using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
    [Header("Achievement Sets")]
    public Image[] achievementImages;
    public Text[] achievementTexts;

    [Header("Audio")]
    public AudioSource switchSound;

    private int currentIndex = 0;

    void Start()
    {
        // Ensure only the first set is visible at the start
        for (int i = 0; i < achievementImages.Length; i++)
        {
            bool isActive = i == 0;
            achievementImages[i].gameObject.SetActive(isActive);
            achievementTexts[i].gameObject.SetActive(isActive);
        }
    }

    public void NextAchievement()
    {
        // Play sound if assigned
        if (switchSound != null)
        {
            switchSound.Play();
        }

        // Hide current set
        achievementImages[currentIndex].gameObject.SetActive(false);
        achievementTexts[currentIndex].gameObject.SetActive(false);

        // Increment and wrap index
        currentIndex = (currentIndex + 1) % achievementImages.Length;

        // Show next set
        achievementImages[currentIndex].gameObject.SetActive(true);
        achievementTexts[currentIndex].gameObject.SetActive(true);
    }
}
