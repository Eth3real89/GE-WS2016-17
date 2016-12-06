using UnityEngine;
using System.Collections;

public class AttackSetupFadeIn : MonoBehaviour {

    public float m_Duration;
    public float m_TargetAlpha;

    private Renderer rend;
    private Color startColor;
    private Color endColor;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();

        StartCoroutine(FadeIn(m_Duration));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator FadeIn(float duration)
    {
        Material mat = rend.material;

        startColor = mat.color;
        endColor = new Color(startColor.r, startColor.g, startColor.b, m_TargetAlpha);

        for(float f = 0.0f; f < duration; f += Time.deltaTime)
        {
            rend.material.color = Color.Lerp(startColor, endColor, f / duration);
            yield return null;
        }
    }
}
