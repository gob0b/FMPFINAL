using UnityEngine;
using UnityEngine.UI;

public class ButtonGrow : MonoBehaviour
{
    public Button button; // Assign your button in the inspector
    public float scaleIncrease = 0.1f; // Amount to grow per click
    public AudioSource audioSource; // Assign an AudioSource in the inspector

    private void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        button.onClick.AddListener(GrowButton);
    }

    void GrowButton()
    {
        button.transform.localScale += new Vector3(scaleIncrease, scaleIncrease, 0);

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}


