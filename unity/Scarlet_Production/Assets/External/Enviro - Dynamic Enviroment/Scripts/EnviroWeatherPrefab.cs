using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnviroWeatherPrefab : MonoBehaviour {

	public string Name;
	[Header("Season Settings")]
	public bool Spring = true;
	[Range(1,100)]
	public float possibiltyInSpring = 100f;
	public bool Summer = true;
	[Range(1,100)]
	public float possibiltyInSummer = 100f;
	public bool Autumn = true;
	[Range(1,100)]
	public float possibiltyInAutumn = 100f;
	public bool winter = true;
	[Range(1,100)]
	public float possibiltyInWinter = 100f;

	[HideInInspector]
    public List<float> effectEmmisionRates = new List<float>();

	[Header("Cloud Settings")]
	public EnviroWeatherCloudConfig cloudConfig;

	[Header("Fog Settings")]
	[Header("Linear:")]
	public float fogStartDistance;
	public float fogDistance;
	[Header("Exp:")]
	public float fogDensity;

	[Header("Advanced Fog Settings:")]
	[Tooltip("Override the Sky with defined Color. The color alpha value defines the override intensity")]
	public Color WeatherColorMod = Color.white;
	[Tooltip("Define the height of fog rendered in sky.")]
	[Range(0,1)]
	public float SkyFogHeight = 0.5f;
	[Tooltip("Define the intensity of fog rendered in sky.")]
	[Range(0,1)]
	public float SkyFogIntensity = 0.5f;
	[Tooltip("Define the scattering intensity of fog.")]
	[Range(0,1)]
	public float FogScatteringIntensity = 0.5f;

	[Header("Weather Settings")]
	public List<ParticleSystem> effectParticleSystems = new List<ParticleSystem>();

	[Tooltip("Lower or higher the sun/moon light intensity for this weathertpye.")]
	public float sunLightMod = 0.0f;
	[Tooltip("Wind intensity that will applied to wind zone.")]
	[Range(0,1)]
	public float WindStrenght = 0.5f;
	[Tooltip("The maximum wetness level that can be reached.")]
	[Range(0,1)]
	public float wetnessLevel = 0f;
	[Tooltip("The maximum snow level that can be reached.")]
	[Range(0,1)]
	public float snowLevel = 0f;
	[Tooltip("Activate this to enable thunder and lightning.")]
	public bool isLightningStorm;

	[Header("Audio Settings")]
	[Tooltip("Define an ambient sound for this weather.")]
	public AudioClip Sfx;


}

