using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeedController : MonoBehaviour
{
    private Animator animator;
    private float speed;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.speed = speed;
    }

    public void ChangeAnimationSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
