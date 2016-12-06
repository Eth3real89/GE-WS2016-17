using UnityEngine;
using System.Collections;

public class GreyScaleScript : MonoBehaviour {

    public Texture textureRamp;
    public float rampOffset; 
    public float _RedPower; 
    public float _RedDelta;

    public Material mat;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        mat.SetTexture("_RampTex", textureRamp);
        mat.SetFloat("_RampOffset", rampOffset);
        mat.SetFloat("_RedPower", _RedPower);
        mat.SetFloat("_RedDelta", _RedDelta);

        Graphics.Blit(src, dest, mat);
    }
}
