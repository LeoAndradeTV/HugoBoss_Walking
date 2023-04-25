
using UnityEngine;

public class AnimationSpeedController : MonoBehaviour
{
    private Animator animator;
    private float speed;

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        //animator = GetComponent<Animator>(); 
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void ChangeAnimationSpeed(float newSpeed)
    {
       // animator.speed = 0.75f;
       //animator.Play("Walking");
        //animator.speed = newSpeed;
        //Debug.Log("animator speed" + animator.speed);
    }
}
