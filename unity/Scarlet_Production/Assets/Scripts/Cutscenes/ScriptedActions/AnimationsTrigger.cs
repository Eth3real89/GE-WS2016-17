using System.Collections;
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

    public void ChaseAway()
    {
        animator.SetTrigger("ChaseAwayTrigger");
    }

    public void SetEyesWhite()
    {
        body.materials[8].SetColor("_Color", Color.black);
        body.materials[8].SetColor("_SpecColor", Color.black);
    }

    public void TurnEyesRed()
    {
        StartCoroutine(FadeEyesRed());
    }

    IEnumerator FadeEyesRed()
    {
        LerpTimer t = new LerpTimer(0.2f);
        t.Start();
        while (t.GetLerpProgress() < 1)
        {
            Color c = Color.Lerp(Color.black, Color.red, t.GetLerpProgress());
            body.materials[8].SetColor("_Color", c);
            body.materials[8].SetColor("_SpecColor", c);
            yield return null;
        }
    }
}
