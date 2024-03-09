using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    void Update()
    {
        if (! AnimatorIsPlaying())
        {
            GameObject.Destroy(gameObject, 0f);
        }
    }
    
    private bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void Play(string animation)
    {
        animator.Play(animation);
    }
}
