using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsTrigger : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Transformation()
    {
        animator.SetTrigger("TransformationTrigger");
    }
}
