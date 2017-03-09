using UnityEngine;

public class AnimationsTrigger : MonoBehaviour
{
    public Renderer body;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Transformation()
    {
        animator.SetTrigger("TransformationTrigger");
    }

    public void SetEyesWhite()
    {
        body.materials[8].SetColor("_Color", Color.black);
        body.materials[8].SetColor("_SpecColor", Color.black);
    }
}
