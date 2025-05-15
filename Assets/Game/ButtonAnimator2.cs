using UnityEngine;

public class Button2Animator : MonoBehaviour
{
    public Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>(); // Automatically finds Animator on the GameObject
        if (animator == null)
        {
            Debug.LogError("❌ Animator component is MISSING on " + gameObject.name + "! Add an Animator component.");
        }
    }

    public void PlayButton2Animation()
    {
        if (animator != null)
        {
            animator.ResetTrigger("button2pressed"); // Reset in case it was stuck
            animator.SetTrigger("button2pressed"); // Trigger the animation
            Debug.Log("✅ Animation 'button2pressed' triggered on " + gameObject.name);
        }
        else
        {
            Debug.LogError("❌ Animator component NOT FOUND on " + gameObject.name);
        }
    }
}



