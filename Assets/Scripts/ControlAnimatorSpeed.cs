using UnityEngine;

public class ControlAnimatorSpeed : MonoBehaviour
{
    public GameObject animatorObject;
    // Start is called before the first frame update
    void Start()
    {
        animatorObject = GameObject.FindGameObjectWithTag("AnimatorObject");
    }

    // Update is called once per frame
    void Update()
    {
        Animator animator = animatorObject.GetComponent<Animator>();
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0,
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }
}
