using UnityEngine;
using UnityEngine.UI;

public class SliderLevel : MonoBehaviour
{
    public Slider slider;
    public int requiredMoves = 5;
    public float moveThreshold = 0.8f;
    public AudioSource slideSound;

    private int moveCount = 0;
    private float lastValue;
    private bool movedUp = false;

    void StartLevel()
    {
        moveCount = 0;
        lastValue = slider.value;
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        float delta = slider.value - lastValue;

        if (!movedUp && slider.value >= moveThreshold)
        {
            moveCount++;
            movedUp = true;
            slideSound?.Play();
        }

        if (slider.value <= 1 - moveThreshold)
        {
            movedUp = false;
        }

        lastValue = slider.value;

        if (moveCount >= requiredMoves)
        {
            FindObjectOfType<TutorialGameMode>().CompleteLevel();
        }
    }
}
