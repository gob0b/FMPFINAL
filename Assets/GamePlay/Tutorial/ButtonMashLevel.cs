using UnityEngine;
using UnityEngine.UI;

public class ButtonMashLevel : MonoBehaviour
{
    public Button mashButton;
    public int requiredPresses = 10;
    public AudioSource pressSound;
    private int currentPresses = 0;

    void StartLevel()
    {
        currentPresses = 0;
        mashButton.onClick.AddListener(Press);
    }

    void Press()
    {
        pressSound?.Play();
        currentPresses++;
        if (currentPresses >= requiredPresses)
        {
            FindObjectOfType<TutorialGameMode>().CompleteLevel();
        }
    }

    void OnDisable()
    {
        mashButton.onClick.RemoveListener(Press);
    }
}
