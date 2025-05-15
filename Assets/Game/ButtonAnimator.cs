using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
    public Animator buttonPressAnimator; // Animator component for the button

    // Method to trigger the "Button Pressed" animation
    public void Buttonin()
    {
        if (buttonPressAnimator != null)
        {
            buttonPressAnimator.SetTrigger("Button Pressed"); // Triggers the first animation
        }
    }
}