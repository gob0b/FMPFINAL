using UnityEngine;

public class AnimationTriggerReceiver : MonoBehaviour
{
    public Animator animator;

    public void TriggerAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Timer"); // Make sure "Timer" is a trigger in the Animator
        }
    }
}
