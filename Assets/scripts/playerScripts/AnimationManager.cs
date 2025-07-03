using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator anim;

    // calls to end grab animation on event at end of grab animation
    public void endGrab()
    {
        Debug.Log("Function called, ending grab");
        anim.SetBool("isGrabbing", false);
        Debug.Log("isGrabbing: " + anim.GetBool("isGrabbing"));
    }
}
