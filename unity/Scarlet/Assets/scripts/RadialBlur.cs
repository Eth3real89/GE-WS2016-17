using UnityEngine;
using System.Collections;

public class RadialBlur : MonoBehaviour
{
    public Shader rbShader;
   
    public float blurStrength;
    public float blurWidth;
    public float blurDuration;
    public float liftBlurDuration;

    private float liftBlurStart;
    private float originalBlurStrength;

    private Material rbMaterial = null;
 
    private Material GetMaterial()
    {
        if (rbMaterial == null)
        {
            rbMaterial = new Material(rbShader);
            rbMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        return rbMaterial;
    }
 
    void Start()
    {
        if (rbShader == null)
        {
            Debug.LogError("shader missing!", this);
        }

        GetMaterial().SetFloat("_BlurWidth", blurWidth);

        liftBlurStart = 0f;
        originalBlurStrength = blurStrength;
    }
 
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if(blurDuration > 0f)
        {
            blurDuration -= Time.deltaTime;
        }
        else
        {
            if(liftBlurStart < liftBlurDuration)
            {
                liftBlurStart += Time.deltaTime;

                blurStrength = Mathf.Lerp(originalBlurStrength, 0f, (liftBlurStart / liftBlurDuration));
            }
        }

        GetMaterial().SetFloat("_BlurStrength", blurStrength);
        Graphics.Blit(source, dest, GetMaterial());
    }

    public void Reset()
    {
        liftBlurStart = Time.deltaTime;
        blurStrength = originalBlurStrength;
    }
}