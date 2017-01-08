using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkenCameraImage : MonoBehaviour {

    public Shader darkShader;
    private Material darkMaterial;

    // Use this for initialization
    void Start () {
        darkMaterial = new Material(darkShader);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(source, dest, darkMaterial);
    }
}
