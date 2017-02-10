////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////        								EnviroSky									    ////////////      
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


[Serializable]
public class ObjectVariables // References - setup these in inspector! Or use the provided prefab.
{
	public GameObject Sun        = null;
	public GameObject Moon       = null;
	public GameObject VolumeCloudsLayer1 = null;
	public GameObject VolumeCloudsShadowsLayer1 = null;
	public GameObject VolumeCloudsLayer2 = null;
	public GameObject VolumeCloudsShadowsLayer2 = null;
	public Material sky;
}

[Serializable]
public class Satellite 
{
	public string name;
	public GameObject prefab = null;
	public float orbit;
	public Vector3 coords;
	public float speed;
}

[Serializable]
public class Sky 
{
	[Header("Scattering")]
	public Vector3 waveLenght;
	public float rayleigh = 1f;
	public float turbidity = 1f;
	public float mie = 1f;
	public float g = 0.75f;
	public AnimationCurve scatteringCurve;
	public Gradient scatteringColor;

	[Header("Sun")]
	public float SunIntensity = 50f;
	public float SunDiskScale = 50f;
	public float SunDiskIntensity = 5f;
	public Gradient SunDiskColor;


	[Header("Tonemapping")]
	public float SkyExposure = 1.5f;
	public float SkyLuminence = 1f;
	public float SkyCorrection = 2f;	
	public Color weatherMod = Color.white;


	[Header("Stars")]
	public float StarsIntensity = 5f;
	[HideInInspector]
	public float currentStarsIntensity = 0f;
	public float StarsBlinking = 5f;
}

[Serializable]
public class SatellitesVariables 
{
	public List<Satellite> additionalSatellites = new List<Satellite>();
}

[Serializable]
public class TimeVariables // GameTime variables
{
	[Header("Date and Time")]
	public bool ProgressTime = true;
    public float Hours  = 12f;  // 0 -  24 Hours day
    public float Days = 1f; //  1 - 365 Days of the year
	public float Years = 1f; // Count the Years maybe usefull!
	public float DayLengthInMinutes = 30; // DayLenght in realtime minutes
	[Range(0,24)]
	public float NightTimeInHours = 18f; 
	[Range(0,24)]
	public float MorningTimeInHours = 5f; 
	[Header("Location")]
    public float Latitude   = 0f;   // -90,  90   Horizontal earth lines
    public float Longitude  = 0f;   // -180, 180  Vertical earth line

}

[Serializable] 
public class LightVariables // All Lightning Variables
{
	[Header("Direct")]
	public Gradient LightColor;
    public float SunIntensity = 2f;
	public float MoonIntensity = 0.5f;
	public float MoonPhase = 0.0f;
	[Header("Ambient")]
	public UnityEngine.Rendering.AmbientMode ambientMode;
	public AnimationCurve ambientIntensity;
	public Gradient ambientSkyColor;
	public Gradient ambientEquatorColor;
	public Gradient ambientGroundColor;
	[HideInInspector]public float SunWeatherMod = 0.0f;
}


[Serializable]
public class FogSettings 
{
	public FogMode Fogmode;

	public bool AdvancedFog = true;

	[Header("Fog Material")]
	public Material fogMaterial;

	[Header("Distance Fog")]
	public bool  distanceFog = true;
	public bool  useRadialDistance = false;
	public float startDistance = 0.0f;
	[Range(0f,100f)]
	public float distanceFogIntensity = 0.0f;

	[Header("Height Fog")]
	public bool  heightFog = true;
	public float height = 90.0f;
	[Range(0f,1f)]
	public float heightDensity = 0.15f;
	[Range(0f,1f)]
	public float noiseIntensity = 0.01f;
	[Range(0f,0.1f)]
	public float noiseScale = 0.1f;

	public Vector2 heightFogVelocity;

	[Header("Fog Scattering")]
	[Range(0f,1f)]
	public float skyFogHeight = 1f;
	[Range(0f,1f)]
	public float skyFogStrength = 0.1f;
	[Range(0f,1f)]
	public float scatteringStrenght = 0.5f;
}

[Serializable]
public class LightShaftsSettings 
{
	public bool lightShafts = true;

	[Header("Quality Settings")]
	public EnviroLightShafts.SunShaftsResolution resolution = EnviroLightShafts.SunShaftsResolution.Normal;
	public EnviroLightShafts.ShaftsScreenBlendMode screenBlendMode = EnviroLightShafts.ShaftsScreenBlendMode.Screen;
	public bool  useDepthTexture = true;

	[Header("Intensity Settings")]
	public Gradient lighShaftsColor;
	public Gradient thresholdColor;
	public float blurRadius = 2.5f;
	public float intensity = 1.15f;
	public float maxRadius = 0.75f;

	[Header("Materials")]
	public Material lightShaftsMaterial;
	public Material clearMaterial;
}

[Serializable]
public class CloudVariables // Default cloud settings, will be changed on runtime if Weather is enabled!
{
	[Header("Setup")]
	[Range(10,200)]
	public int Quality = 25;
	public int segmentCount = 3;
	public float thickness = 0.4f;
	public bool curved;
	public float curvedIntensity = 0.001f;
	[Range(0.5f,2f)]
	public float Scaling = 1f; 

	[Header("Runtime Settings")]
	public Color FirstColor;
	public Color SecondColor;
    public float Coverage = 1.0f; // Dense of clouds
	public float Density = 1.0f; 
	public float Alpha = 0.5f;
	public float Speed1    = 0.5f; // Animate speed1 of clouds
	public float Speed2    = 1.0f; // Animate speed2 of clouds
	public Vector2 WindDir = new Vector2 (1f, 1f);
	public float LayerOffset = 0.5f;


	[HideInInspector]
	public float AmbientLightIntensity = 0.3f;
	[HideInInspector]
	public float DirectLightIntensity = 1f;
	[Header("Cloud Lighting")]
	public AnimationCurve DirectLightIntensityTimed;

}
	
[ExecuteInEditMode]
[AddComponentMenu("Enviro/Sky System")]
public class EnviroSky : MonoBehaviour
{
    // Parameters
	public ObjectVariables Components = null;
	public TimeVariables GameTime  = null;
	public LightVariables  Lighting   = null;
	public Sky  Sky   = null;
	public CloudVariables Clouds = null;
	public FogSettings Fog = null;
	public LightShaftsSettings LightShafts = null;
	public SatellitesVariables Satellites = null;
	[HideInInspector]
	public List<Transform> mySatellites = new List<Transform>();

	// Private Variables
	private bool isNight = true;

	[HideInInspector]
	public EnviroFog atmosphericFog;
	[HideInInspector]
	public EnviroLightShafts lightShaftsScript;

    //Some Pointers
    private Transform DomeTransform;

    private Material  VolumeCloudShader1;
	private Material  VolumeCloudShadowShader1;    
	private Material  VolumeCloudShader2;
	private Material  VolumeCloudShadowShader2;

    private Transform SunTransform;
	private Light     SunLight;
	private Transform MoonTransform;
	private Renderer  MoonRenderer;
	private Material  MoonShader;
	private Light     MoonLight;
	private EnviroMgr EnvMgr;

	private float lastHourUpdate;
	private float starsRot;

	private float OrbitRadius
    {
        get { return DomeTransform.localScale.x; }
    }

	// Scattering constants
    const float pi = Mathf.PI;
	private Vector3 K          =  new Vector3(686.0f, 678.0f, 666.0f);
	private const float n      =  1.0003f;   
	private const float N      =  2.545E25f;
	private const float pn     =  0.035f;    

	private float lastHour;

    void Start()
    {
		lastHourUpdate = GameTime.Hours;
		RemoveSatellites ();
		CreateSatellites ();
		lastHourUpdate = EnviroMgr.instance.currentTimeInHours;
    }
		
    void OnEnable()
    {
        DomeTransform = transform;

		atmosphericFog = EnviroMgr.instance.PlayerCamera.gameObject.GetComponent<EnviroFog> ();

		if (atmosphericFog == null) 
		{
			atmosphericFog = EnviroMgr.instance.PlayerCamera.gameObject.AddComponent<EnviroFog> ();
			atmosphericFog.fogMaterial = Fog.fogMaterial;
			atmosphericFog.fogShader = Fog.fogMaterial.shader;
		}

		lightShaftsScript = EnviroMgr.instance.PlayerCamera.gameObject.GetComponent<EnviroLightShafts> ();

		if (lightShaftsScript == null) 
		{
			lightShaftsScript = EnviroMgr.instance.PlayerCamera.gameObject.AddComponent<EnviroLightShafts> ();
			lightShaftsScript.sunShaftsMaterial = LightShafts.lightShaftsMaterial;
			lightShaftsScript.sunShaftsShader = LightShafts.lightShaftsMaterial.shader;
			lightShaftsScript.simpleClearMaterial = LightShafts.clearMaterial;
			lightShaftsScript.simpleClearShader = LightShafts.clearMaterial.shader;
		}
			
		RemoveSatellites ();
		CreateSatellites ();

		// Setup Fog
		RenderSettings.fogMode = Fog.Fogmode;

		// Setup Skybox Material
		RenderSettings.skybox = Components.sky;
		RenderSettings.ambientMode = Lighting.ambientMode;
		// Setup Camera
		if(EnviroMgr.instance.PlayerCamera != null)
		 EnviroMgr.instance.PlayerCamera.clearFlags = CameraClearFlags.Skybox;

		if (Components.VolumeCloudsLayer1 && Components.VolumeCloudsLayer2)
        {
			VolumeCloudShader1 = Components.VolumeCloudsLayer1.GetComponent<Renderer>().sharedMaterial;
			VolumeCloudShader2 = Components.VolumeCloudsLayer2.GetComponent<Renderer>().sharedMaterial;

			MeshFilter filter1 = Components.VolumeCloudsLayer1.GetComponent<MeshFilter>();
			MeshFilter filter2 = Components.VolumeCloudsLayer2.GetComponent<MeshFilter>();

			if (filter1 != null)
			{
				if (filter1.sharedMesh != null)
				{
					UnityEngine.Object.DestroyImmediate(filter1.sharedMesh);
				}
			}

			if (filter2 != null)
			{
				if (filter2.sharedMesh != null)
				{
					UnityEngine.Object.DestroyImmediate(filter2.sharedMesh);
				}
			}

			CreateVolumeCloudMesh (Clouds.Quality); // Create Cloudmesh!
			CreateVolumeCloudMesh (1); // Create shadow mesh!
        }
		else if (Components.VolumeCloudsLayer1 == null)
        {
			Debug.LogError("Please set VolumeCloudsLayer1 object in inspector!");
            this.enabled = false;
        }

		if (Components.VolumeCloudsShadowsLayer1)
        {
			VolumeCloudShadowShader1 = Components.VolumeCloudsShadowsLayer1.GetComponent<Renderer>().sharedMaterial;
        }
		else if (Components.VolumeCloudsShadowsLayer1 == null)
        {
			Debug.LogError("Please set VolumeCloudsShadowsLayer1 object in inspector!");
            this.enabled = false;
        }

		if (Components.VolumeCloudsShadowsLayer2)
		{
			VolumeCloudShadowShader2 = Components.VolumeCloudsShadowsLayer2.GetComponent<Renderer>().sharedMaterial;
		}
		else if (Components.VolumeCloudsShadowsLayer2 == null)
		{
			Debug.LogError("Please set VolumeCloudsShadowsLayer2 object in inspector!");
			this.enabled = false;
		}
	
		if (Components.Sun)
        {
			SunTransform = Components.Sun.transform;
			SunLight = Components.Sun.GetComponent<Light>();
        }
        else
        {
            Debug.LogError("Please set Sun object in inspector!");
            this.enabled = false;
        }

		if (Components.Moon)
        {
			MoonTransform = Components.Moon.transform;
			MoonRenderer = Components.Moon.GetComponent<Renderer>();
            MoonShader = MoonRenderer.sharedMaterial;
			MoonLight = Components.Moon.GetComponent<Light>();
        }
        else
        {
			Debug.LogError("Please set Moon object in inspector!");
            this.enabled = false;
        }

		if(!EnviroMgr.instance.PlayerCamera)
		{
			Debug.LogError("Please assign your MainCamera in EnviroMgr-component!");
			this.enabled = false;
		}

		if(!EnviroMgr.instance.Audio.SpringDayAmbient || !EnviroMgr.instance.Audio.SummerDayAmbient || !EnviroMgr.instance.Audio.AutumnDayAmbient || !EnviroMgr.instance.Audio.WinterDayAmbient)
		{
			Debug.LogError("Please assign your day audioclip in EnviroSky-component!");
			this.enabled = false;
		}
		if(!EnviroMgr.instance.Audio.SpringNightAmbient || !EnviroMgr.instance.Audio.SummerNightAmbient || !EnviroMgr.instance.Audio.AutumnNightAmbient || !EnviroMgr.instance.Audio.WinterNightAmbient)
		{
			Debug.LogError("Please assign your night audioclip in EnviroSky-component!");
			this.enabled = false;
		}

		EnvMgr = GetComponent<EnviroMgr> ();


    }

	void CreateSatellites ()
	{
		for (int i = 0; i < Satellites.additionalSatellites.Count; i++) {
			GameObject sat = (GameObject)Instantiate (Satellites.additionalSatellites [i].prefab, transform.position + new Vector3 (0f, Satellites.additionalSatellites [i].orbit, 0f), Quaternion.identity);
			sat.transform.parent = transform;
			mySatellites.Add (sat.transform);
		}
	}

	void RemoveSatellites ()
	{
		for (int i = 0; i < mySatellites.Count; i++) 
		{
			DestroyImmediate (mySatellites [i].gameObject);
		}
		mySatellites = new List<Transform> ();
	}


	void UpdateQuality()
	{
		// Setup QualitySettings
		switch (EnviroMgr.instance.Quality.CloudsQuality) {

		case EnviroQualitySettings.CloudQuality.None:
			Components.VolumeCloudsLayer1.SetActive (false);
			Components.VolumeCloudsLayer2.SetActive (false);
			Components.VolumeCloudsShadowsLayer1.SetActive (false);
			Components.VolumeCloudsShadowsLayer2.SetActive (false);
		break;

		case EnviroQualitySettings.CloudQuality.OneLayer:
			Components.VolumeCloudsLayer1.SetActive (true);
			Components.VolumeCloudsLayer2.SetActive (false);

			if (EnviroMgr.instance.Quality.CloudsShadowCast) 
			{
				Components.VolumeCloudsShadowsLayer1.SetActive (true);
				Components.VolumeCloudsShadowsLayer2.SetActive (false);
			}
			else
			{
				Components.VolumeCloudsShadowsLayer1.SetActive (false);
				Components.VolumeCloudsShadowsLayer2.SetActive (false);	
			}
		break;

		case EnviroQualitySettings.CloudQuality.TwoLayer:
			Components.VolumeCloudsLayer1.SetActive (true);
			Components.VolumeCloudsLayer2.SetActive (true);
			if (EnviroMgr.instance.Quality.CloudsShadowCast) 
			{
				Components.VolumeCloudsShadowsLayer1.SetActive (true);
				Components.VolumeCloudsShadowsLayer2.SetActive (true);
			}
			else
			{
				Components.VolumeCloudsShadowsLayer1.SetActive (false);
				Components.VolumeCloudsShadowsLayer2.SetActive (false);	
			}
		break;

		}

		//Update Fog
		if (atmosphericFog != null) 
		{
			atmosphericFog.distanceFog = Fog.distanceFog;
			atmosphericFog.heightFog = Fog.heightFog;
			atmosphericFog.height = Fog.height;
			atmosphericFog.heightDensity = Fog.heightDensity;
			atmosphericFog.useRadialDistance = Fog.useRadialDistance;
			atmosphericFog.startDistance = Fog.startDistance;

			if (Fog.AdvancedFog)
				atmosphericFog.enabled = true;
			else
				atmosphericFog.enabled = false;
		}

		//Update LightShafts
		if (lightShaftsScript != null) 
		{
			lightShaftsScript.resolution = LightShafts.resolution;
			lightShaftsScript.screenBlendMode = LightShafts.screenBlendMode;
			lightShaftsScript.useDepthTexture = LightShafts.useDepthTexture;
			lightShaftsScript.sunThreshold = LightShafts.thresholdColor.Evaluate (GameTime.Hours / 24);;

			lightShaftsScript.sunShaftBlurRadius = LightShafts.blurRadius;
			lightShaftsScript.sunShaftIntensity = LightShafts.intensity;
			lightShaftsScript.maxRadius = LightShafts.maxRadius;
			lightShaftsScript.sunColor = LightShafts.lighShaftsColor.Evaluate (GameTime.Hours / 24);

			if (LightShafts.lightShafts)
				lightShaftsScript.enabled = true;
			else
				lightShaftsScript.enabled = false;
		}
	}

	Vector3 CalculatePosition ()
	{
		Vector3 newPosition;
		newPosition.x = EnviroMgr.instance.Player.transform.position.x;
		newPosition.z = EnviroMgr.instance.Player.transform.position.z;
		newPosition.y = EnviroMgr.instance.AdvancedPositioning.fixedSkyHeight;

		return newPosition;
	}

	void UpdateSatellites ()
	{
		for (int i = 0; i < mySatellites.Count; i++)
		{
			float s = (EnviroMgr.instance.currentTimeInHours - lastHour) * Satellites.additionalSatellites [i].speed;
			mySatellites [i].RotateAround (transform.position, Satellites.additionalSatellites [i].coords, s);

			Vector3 desiredPos = (mySatellites [i].position - transform.position).normalized * Satellites.additionalSatellites [i].orbit + transform.position;
			mySatellites [i].position = Vector3.Slerp (mySatellites [i].position,desiredPos, Time.deltaTime * 5f);
		}

		lastHour = EnviroMgr.instance.currentTimeInHours;
	}

    void Update()
    {
        UpdateTime();
		ValidateParameters();
		UpdateQuality ();
		updateAmbientLight ();

		if (Fog.AdvancedFog)
			UpdateAdvancedFog ();

        // Calculates the Solar latitude
        float latitudeRadians = Mathf.Deg2Rad * GameTime.Latitude;
        float latitudeRadiansSin = Mathf.Sin(latitudeRadians);
        float latitudeRadiansCos = Mathf.Cos(latitudeRadians);

		// Calculates the Solar longitude
		float longitudeRadians = Mathf.Deg2Rad * GameTime.Longitude;

        // Solar declination - constant for the whole globe at any given day
		float solarDeclination = 0.4093f * Mathf.Sin(2f * pi / 368f * (GameTime.Days - 81f));
        float solarDeclinationSin = Mathf.Sin(solarDeclination);
        float solarDeclinationCos = Mathf.Cos(solarDeclination);

        // Calculate Solar time
		float timeZone = (int)(GameTime.Longitude / 15f);
        float meridian = Mathf.Deg2Rad * 15f * timeZone;
		float solarTime = GameTime.Hours + 0.170f * Mathf.Sin(4f * pi / 373f * (GameTime.Days - 80f)) - 0.129f * Mathf.Sin(2f * pi / 355f * (GameTime.Days - 8f))  + 12f / pi * (meridian - longitudeRadians);
        float solarTimeRadians = pi / 12f * solarTime;
        float solarTimeSin = Mathf.Sin(solarTimeRadians);
        float solarTimeCos = Mathf.Cos(solarTimeRadians);

        // Solar altitude angle between the sun and the horizon
        float solarAltitudeSin = latitudeRadiansSin * solarDeclinationSin - latitudeRadiansCos * solarDeclinationCos * solarTimeCos;
        float solarAltitude = Mathf.Asin(solarAltitudeSin);

        // Solar azimuth angle of the sun around the horizon
        float solarAzimuthY = -solarDeclinationCos * solarTimeSin;
        float solarAzimuthX = latitudeRadiansCos * solarDeclinationSin - latitudeRadiansSin * solarDeclinationCos * solarTimeCos;
        float solarAzimuth = Mathf.Atan2(solarAzimuthY, solarAzimuthX);

        // Convert to spherical coords
        float coord = pi / 2 - solarAltitude;
        float phi = solarAzimuth;

        // Update sun position
        SunTransform.position = DomeTransform.position + DomeTransform.rotation * SphericalToCartesian(OrbitRadius, coord, phi);
        SunTransform.LookAt(DomeTransform.position);

        // Update moon position
        MoonTransform.position = DomeTransform.position + DomeTransform.rotation * SphericalToCartesian(OrbitRadius, coord + pi, phi);
        MoonTransform.LookAt(DomeTransform.position);

		if (isNight)
			Sky.currentStarsIntensity = Mathf.Lerp (Sky.currentStarsIntensity, Sky.StarsIntensity, Time.deltaTime * 20f);
		else
			Sky.currentStarsIntensity = Mathf.Lerp (Sky.currentStarsIntensity, 0f, Time.deltaTime * 20f);



        // Update sun and fog color according to the new position of the sun
		SetupSunAndMoonColor(coord);
		SetupShader(coord);

		if (EnviroMgr.instance.PlayerCamera != null)
        {
			if (!EnviroMgr.instance.AdvancedPositioning.FixedHeight)
				transform.position = EnviroMgr.instance.PlayerCamera.transform.position;
			else 
			{
				transform.position = CalculatePosition ();
			}

			transform.localScale = new Vector3(EnviroMgr.instance.PlayerCamera.farClipPlane - (EnviroMgr.instance.PlayerCamera.farClipPlane * 0.1f), EnviroMgr.instance.PlayerCamera.farClipPlane - (EnviroMgr.instance.PlayerCamera.farClipPlane * 0.1f), EnviroMgr.instance.PlayerCamera.farClipPlane - (EnviroMgr.instance.PlayerCamera.farClipPlane * 0.1f));
        }

		if (!isNight && (GameTime.Hours >= GameTime.NightTimeInHours || GameTime.Hours <= GameTime.MorningTimeInHours) && EnviroMgr.instance.AudioSourceAmbient!=null)
		{
			switch (EnviroMgr.instance.seasons.currentSeasons)
			{
			case SeasonVariables.Seasons.Spring:
				EnviroMgr.instance.AudioSourceAmbient.clip = EnviroMgr.instance.Audio.SpringNightAmbient;
				break;
				
			case SeasonVariables.Seasons.Summer:
				EnviroMgr.instance.AudioSourceAmbient.clip = EnviroMgr.instance.Audio.SummerNightAmbient;
				break;
				
			case SeasonVariables.Seasons.Autumn:
				EnviroMgr.instance.AudioSourceAmbient.clip = EnviroMgr.instance.Audio.AutumnNightAmbient;
				break;
				
			case SeasonVariables.Seasons.Winter:
				EnviroMgr.instance.AudioSourceAmbient.clip = EnviroMgr.instance.Audio.WinterNightAmbient;
				break;
			}
			
			EnviroMgr.instance.AudioSourceAmbient.loop = true;
			EnviroMgr.instance.AudioSourceAmbient.Play();
			isNight = true;

			EnviroMgr.instance.NotifyIsNight ();

		}
		else if (isNight && (GameTime.Hours <= GameTime.NightTimeInHours && GameTime.Hours >= GameTime.MorningTimeInHours)&& EnviroMgr.instance.AudioSourceAmbient!=null)
		{
			
			switch (EnviroMgr.instance.seasons.currentSeasons)
			{
			case SeasonVariables.Seasons.Spring:
				EnviroMgr.instance.AudioSourceAmbient.clip = EnviroMgr.instance.Audio.SpringDayAmbient;
				break;
				
			case SeasonVariables.Seasons.Summer:
				EnviroMgr.instance.AudioSourceAmbient.clip = EnviroMgr.instance.Audio.SummerDayAmbient;
				break;
				
			case SeasonVariables.Seasons.Autumn:
				EnviroMgr.instance.AudioSourceAmbient.clip = EnviroMgr.instance.Audio.AutumnDayAmbient;
				break;
				
			case SeasonVariables.Seasons.Winter:
				EnviroMgr.instance.AudioSourceAmbient.clip = EnviroMgr.instance.Audio.WinterDayAmbient;
				break;
			}

			EnviroMgr.instance.AudioSourceAmbient.loop = true;
			EnviroMgr.instance.AudioSourceAmbient.Play();
			isNight = false;

			EnviroMgr.instance.NotifyIsDay ();
		}
	
    }

	private Vector3 BetaRay() {
		Vector3 Br;

		Vector3 realWavelenght = Sky.waveLenght * 1.0e-9f; // Converting the wavelength values given in Inpector for real scale used in formula.
	
		Br.x = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(realWavelenght.x, 4.0f))*(6.0f-7.0f*pn) ))* 2000f;
		Br.y = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(realWavelenght.y, 4.0f))*(6.0f-7.0f*pn) ))* 2000f;
		Br.z = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(realWavelenght.z, 4.0f))*(6.0f-7.0f*pn) ))* 2000f;

		return Br;
	}


	private Vector3 BetaMie() {
		Vector3 Bm;

		float c = (0.2f * Sky.turbidity ) * 10.0f;
	
		Bm.x = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / Sky.waveLenght.x, 2.0f) * K.x);
		Bm.y = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / Sky.waveLenght.y, 2.0f) * K.y);
		Bm.z = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / Sky.waveLenght.z, 2.0f) * K.z);

		Bm.x=Mathf.Pow(Bm.x,-1.0f);
		Bm.y=Mathf.Pow(Bm.y,-1.0f);
		Bm.z=Mathf.Pow(Bm.z,-1.0f);

		return Bm;
	}

	private Vector3 GetMieG() {
		return new Vector3(1.0f - Sky.g * Sky.g, 1.0f + Sky.g * Sky.g, 2.0f * Sky.g);
	}

	// Setup the Shaders with correct information
    private void SetupShader(float setup)
    {
		RenderSettings.skybox.SetVector ("_SunDir", -SunLight.transform.forward);
		RenderSettings.skybox.SetMatrix ("_Sun",  SunTransform.worldToLocalMatrix);
		RenderSettings.skybox.SetColor("_scatteringColor", Sky.scatteringColor.Evaluate(GameTime.Hours / 24f));
		RenderSettings.skybox.SetColor("_sunDiskColor", Sky.SunDiskColor.Evaluate(GameTime.Hours / 24f));

		RenderSettings.skybox.SetColor("_weatherColor", Sky.weatherMod);

		RenderSettings.skybox.SetVector ("_waveLenght", Sky.waveLenght);
		RenderSettings.skybox.SetVector ("_Bm", BetaMie () * Sky.mie);
		RenderSettings.skybox.SetVector ("_Br", BetaRay() * Sky.rayleigh);
		RenderSettings.skybox.SetVector ("_mieG",         GetMieG ());
		RenderSettings.skybox.SetFloat ("_SunIntensity",  Sky.SunIntensity);
		RenderSettings.skybox.SetFloat ("_SunDiskSize",  Sky.SunDiskScale);
		RenderSettings.skybox.SetFloat ("_SunDiskIntensity",  Sky.SunDiskIntensity);
		RenderSettings.skybox.SetFloat ("_SunDiskSize",  Sky.SunDiskScale);

		RenderSettings.skybox.SetFloat ("_Exposure", Sky.SkyExposure);
		RenderSettings.skybox.SetFloat ("_SkyLuminance", Sky.SkyLuminence);
		RenderSettings.skybox.SetFloat ("_scatteringPower", Sky.scatteringCurve.Evaluate(GameTime.Hours / 24f));
		RenderSettings.skybox.SetFloat ("_SkyCorrection", Sky.SkyCorrection);
		RenderSettings.skybox.SetFloat ("_StarsIntensity", Sky.currentStarsIntensity);

		if (Sky.StarsBlinking > 0.0f)
		{
			starsRot += Sky.StarsBlinking * Time.deltaTime;
			Quaternion rot = Quaternion.Euler (starsRot, starsRot, starsRot);
			Matrix4x4 NoiseRot = Matrix4x4.TRS (Vector3.zero, rot, new Vector3 (1, 1, 1));
			RenderSettings.skybox.SetMatrix ("_NoiseMatrix", NoiseRot);
		}
			
		if (VolumeCloudShader1 != null && EnviroMgr.instance.Quality.CloudsQuality == EnviroQualitySettings.CloudQuality.OneLayer)
        {
			VolumeCloudShader1.SetFloat("_Scale", EnviroMgr.instance.PlayerCamera.farClipPlane * Clouds.Scaling);
			VolumeCloudShader1.SetColor("_BaseColor", Clouds.FirstColor);
			VolumeCloudShader1.SetColor("_ShadingColor", Clouds.SecondColor);
			VolumeCloudShader1.SetFloat("_CloudCover", Clouds.Coverage);
			VolumeCloudShader1.SetFloat("_Density", Clouds.Density);
			VolumeCloudShader1.SetFloat("_CloudAlpha", Clouds.Alpha);
			VolumeCloudShader1.SetFloat("_Speed1Layer", Clouds.Speed1);
			VolumeCloudShader1.SetFloat("_Speed2Layer", Clouds.Speed2);
			VolumeCloudShader1.SetVector("_WindDir", Clouds.WindDir);
			VolumeCloudShader1.SetFloat ("_Exposure", Sky.SkyExposure); 
			VolumeCloudShader1.SetFloat ("_ambient", Clouds.AmbientLightIntensity);
			VolumeCloudShader1.SetFloat ("_direct", Clouds.DirectLightIntensity);
			VolumeCloudShader1.SetFloat ("_nightColorMod", Clouds.DirectLightIntensityTimed.Evaluate(GameTime.Hours / 24f));

        }

		if (VolumeCloudShader1 != null && VolumeCloudShader2 != null && EnviroMgr.instance.Quality.CloudsQuality == EnviroQualitySettings.CloudQuality.TwoLayer)
		{
			VolumeCloudShader1.SetFloat("_Scale", EnviroMgr.instance.PlayerCamera.farClipPlane * Clouds.Scaling);
			VolumeCloudShader1.SetColor("_BaseColor", Clouds.FirstColor);
			VolumeCloudShader1.SetColor("_ShadingColor", Clouds.SecondColor);
			VolumeCloudShader1.SetFloat("_CloudCover", Clouds.Coverage);
			VolumeCloudShader1.SetFloat("_Density", Clouds.Density);
			VolumeCloudShader1.SetFloat("_CloudAlpha", Clouds.Alpha);
			VolumeCloudShader1.SetFloat("_Speed1Layer", Clouds.Speed1);
			VolumeCloudShader1.SetFloat("_Speed2Layer", Clouds.Speed2);
			VolumeCloudShader1.SetVector("_WindDir", Clouds.WindDir);
			VolumeCloudShader1.SetFloat ("_direct", Clouds.DirectLightIntensity);
			VolumeCloudShader1.SetFloat ("_ambient", Clouds.AmbientLightIntensity);
			VolumeCloudShader1.SetFloat ("_nightColorMod", Clouds.DirectLightIntensityTimed.Evaluate(GameTime.Hours / 24f));

			VolumeCloudShader2.SetFloat("_Scale", EnviroMgr.instance.PlayerCamera.farClipPlane * Clouds.Scaling);
			VolumeCloudShader2.SetColor("_BaseColor", Clouds.FirstColor);
			VolumeCloudShader2.SetColor("_ShadingColor", Clouds.SecondColor);
			VolumeCloudShader2.SetFloat("_CloudCover", Clouds.Coverage);
			VolumeCloudShader2.SetFloat("_Density", Clouds.Density);
			VolumeCloudShader2.SetFloat("_CloudAlpha", Clouds.Alpha);
			VolumeCloudShader2.SetFloat("_Speed1Layer", Clouds.Speed1);
			VolumeCloudShader2.SetFloat("_Speed2Layer", Clouds.Speed2);
			VolumeCloudShader2.SetVector("_WindDir", Clouds.WindDir);
			VolumeCloudShader2.SetFloat("_Offset", Clouds.LayerOffset);
			VolumeCloudShader2.SetFloat ("_direct", Clouds.DirectLightIntensity);
			VolumeCloudShader2.SetFloat ("_ambient", Clouds.AmbientLightIntensity);
			VolumeCloudShader2.SetFloat ("_nightColorMod", Clouds.DirectLightIntensityTimed.Evaluate(GameTime.Hours / 24f));
		}

		if (VolumeCloudShadowShader1 != null && EnviroMgr.instance.Quality.CloudsShadowCast && EnviroMgr.instance.Quality.CloudsQuality == EnviroQualitySettings.CloudQuality.OneLayer)
        {
			VolumeCloudShadowShader1.SetFloat("_Scale", EnviroMgr.instance.PlayerCamera.farClipPlane * Clouds.Scaling);
			VolumeCloudShadowShader1.SetFloat("_CloudCover", Clouds.Coverage);
			VolumeCloudShadowShader1.SetFloat("_CloudAlpha", Clouds.Alpha);
			VolumeCloudShadowShader1.SetFloat("_Speed1Layer", Clouds.Speed1);
			VolumeCloudShadowShader1.SetFloat("_Speed2Layer", Clouds.Speed2);
			VolumeCloudShadowShader1.SetVector("_WindDir", Clouds.WindDir);
        }

		if (VolumeCloudShadowShader2 != null && EnviroMgr.instance.Quality.CloudsShadowCast && EnviroMgr.instance.Quality.CloudsQuality == EnviroQualitySettings.CloudQuality.TwoLayer)
		{
			VolumeCloudShadowShader1.SetFloat("_Scale", EnviroMgr.instance.PlayerCamera.farClipPlane * Clouds.Scaling);
			VolumeCloudShadowShader1.SetFloat("_CloudCover", Clouds.Coverage);
			VolumeCloudShadowShader1.SetFloat("_CloudAlpha", Clouds.Alpha);
			VolumeCloudShadowShader1.SetFloat("_Speed1Layer", Clouds.Speed1);
			VolumeCloudShadowShader1.SetFloat("_Speed2Layer", Clouds.Speed2);
			VolumeCloudShadowShader1.SetVector("_WindDir", Clouds.WindDir);

			VolumeCloudShadowShader2.SetFloat("_Scale", EnviroMgr.instance.PlayerCamera.farClipPlane * Clouds.Scaling);
			VolumeCloudShadowShader2.SetFloat("_CloudCover", Clouds.Coverage);
			VolumeCloudShadowShader2.SetFloat("_CloudAlpha", Clouds.Alpha);
			VolumeCloudShadowShader2.SetFloat("_Speed1Layer", Clouds.Speed1);
			VolumeCloudShadowShader2.SetFloat("_Speed2Layer", Clouds.Speed2);
			VolumeCloudShadowShader2.SetVector("_WindDir", Clouds.WindDir);
			VolumeCloudShadowShader2.SetFloat("_Offset", Clouds.LayerOffset);
		}

        if (MoonShader != null)
        {
			MoonShader.SetFloat("_Phase", Lighting.MoonPhase);
        }
    }

	void UpdateAdvancedFog ()
	{
		Fog.fogMaterial.SetVector ("_SunDir", -SunLight.transform.forward);
		Fog.fogMaterial.SetMatrix ("_SunMatrix",  SunTransform.worldToLocalMatrix);
		Fog.fogMaterial.SetColor("_scatteringColor", Sky.scatteringColor.Evaluate(GameTime.Hours / 24f));
		Fog.fogMaterial.SetColor("_sunDiskColor", Sky.SunDiskColor.Evaluate(GameTime.Hours / 24f));
		Fog.fogMaterial.SetColor("_weatherColor", Sky.weatherMod);

		Fog.fogMaterial.SetVector ("_waveLenght", Sky.waveLenght);
		Fog.fogMaterial.SetVector ("_Bm", BetaMie () * Sky.mie);
		Fog.fogMaterial.SetVector ("_Br", BetaRay() * Sky.rayleigh);
		Fog.fogMaterial.SetVector ("_mieG",         GetMieG ());
		Fog.fogMaterial.SetFloat ("_SunIntensity",  Sky.SunIntensity);

		Fog.fogMaterial.SetFloat ("_SunDiskSize",  Sky.SunDiskScale);
		Fog.fogMaterial.SetFloat ("_SunDiskIntensity",  Sky.SunDiskIntensity);
		Fog.fogMaterial.SetFloat ("_SunDiskSize",  Sky.SunDiskScale);

		Fog.fogMaterial.SetFloat ("_Exposure", Sky.SkyExposure);
		Fog.fogMaterial.SetFloat ("_SkyLuminance", Sky.SkyLuminence);
		Fog.fogMaterial.SetFloat ("_scatteringPower", Sky.scatteringCurve.Evaluate(GameTime.Hours / 24f));
		Fog.fogMaterial.SetFloat ("_SkyCorrection", Sky.SkyCorrection);

		Fog.fogMaterial.SetFloat ("_SkyFogHeight", Fog.skyFogHeight);
		Fog.fogMaterial.SetFloat ("_SkyFogStrenght", Fog.skyFogStrength);
		Fog.fogMaterial.SetFloat ("_scatteringStrenght", Fog.scatteringStrenght);
		Fog.fogMaterial.SetFloat ("_distanceFogIntensity", Fog.distanceFogIntensity);

		Fog.fogMaterial.SetVector ("_NoiseData", new Vector4 (Fog.noiseScale, Fog.heightDensity, Fog.noiseIntensity, 0f));
		Fog.fogMaterial.SetVector ("_NoiseVelocity", new Vector4 (Fog.heightFogVelocity.x, Fog.heightFogVelocity.y,0f, 0f));
	}
		
	void FixedUpdate ()
	{
		UpdateSatellites ();
	}
		
	// Update the GameTime
    void UpdateTime()
    {
		float oneDay = GameTime.DayLengthInMinutes * 60f;
        float oneHour = oneDay / 24f;

        float hourTime = Time.deltaTime / oneHour;
        float moonTime = Time.deltaTime / (30f * oneDay) * 2f;

        if (GameTime.ProgressTime) // Calculate Time
        {
			GameTime.Hours += hourTime;
			Lighting.MoonPhase += moonTime;

			if (Lighting.MoonPhase < -1) Lighting.MoonPhase += 2;
			else if (Lighting.MoonPhase > 1) Lighting.MoonPhase -= 2;

			if (GameTime.Hours >= lastHourUpdate + 1f) 
			{
				lastHourUpdate = GameTime.Hours;
				EnviroMgr.instance.NotifyHourPassed ();
			}

			if (GameTime.Hours >= 24)
            {
				GameTime.Hours = 0;
				EnviroMgr.instance.NotifyHourPassed ();
				lastHourUpdate = 0f;
				GameTime.Days = GameTime.Days + 1;
				EnviroMgr.instance.NotifyDayPassed ();
            }

			if(GameTime.Days >= (EnvMgr.seasons.SpringInDays + EnvMgr.seasons.SummerInDays + EnvMgr.seasons.AutumnInDays + EnvMgr.seasons.WinterInDays))
			{
				GameTime.Years = GameTime.Years + 1;
				GameTime.Days = 0;
				EnviroMgr.instance.NotifyYearPassed ();
			}

        }
    }

	void updateAmbientLight ()
	{
		RenderSettings.ambientIntensity = Lighting.ambientIntensity.Evaluate(GameTime.Hours / 24f);

		switch (Lighting.ambientMode) {
		case UnityEngine.Rendering.AmbientMode.Flat:
			RenderSettings.ambientSkyColor = Lighting.ambientSkyColor.Evaluate(GameTime.Hours / 24f);
		break;

		case UnityEngine.Rendering.AmbientMode.Trilight:
			RenderSettings.ambientSkyColor = Lighting.ambientSkyColor.Evaluate(GameTime.Hours / 24f);
			RenderSettings.ambientEquatorColor = Lighting.ambientEquatorColor.Evaluate(GameTime.Hours / 24f);
			RenderSettings.ambientGroundColor = Lighting.ambientGroundColor.Evaluate(GameTime.Hours / 24f);
		break;

		case UnityEngine.Rendering.AmbientMode.Skybox:
			DynamicGI.UpdateEnvironment ();
		break;

		}
	}

    // Calculate sun and moon color
    private void SetupSunAndMoonColor(float setup)
    { 
		SunLight.color = Lighting.LightColor.Evaluate(GameTime.Hours / 24f);
		MoonLight.color = Lighting.LightColor.Evaluate(GameTime.Hours / 24f);
		Shader.SetGlobalColor ("_EnviroLighting", Lighting.LightColor.Evaluate (GameTime.Hours / 24f));


        // Sun altitude and intensity dropoff angle
		float altitude = pi/2 - (setup);
        float altitude_abs = Mathf.Abs(altitude);
        float dropoff_rad = 10f * Mathf.Deg2Rad;
		float sunIntensityMax = Lighting.SunIntensity + Lighting.SunWeatherMod;
		float moonIntensityMax = (Lighting.MoonIntensity * Mathf.Clamp01(1.3f - Mathf.Abs(Lighting.MoonPhase))) + Lighting.SunWeatherMod;

		if (moonIntensityMax <= 0.1f)
			moonIntensityMax = 0.1f;

        // Set sun and moon intensity
        if (altitude > 0)
        {
			MoonLight.enabled = false;
			SunLight.enabled = true;

			if (altitude_abs < dropoff_rad)
            {
				SunLight.intensity = Mathf.Lerp(0.3f, sunIntensityMax, (altitude_abs) / dropoff_rad);
            }
			else 
			{
				SunLight.intensity = Mathf.Lerp(SunLight.intensity, sunIntensityMax,0.01f);
			}

			if (lightShaftsScript != null && LightShafts.lightShafts) 
			{
				lightShaftsScript.sunTransform = Components.Sun.transform;
			}
        }
        else
        {
			SunLight.enabled = false;
			MoonLight.enabled = true;
           
			if (altitude_abs < dropoff_rad) 
			{
				MoonLight.intensity = Mathf.Lerp (0.3f, moonIntensityMax, (altitude_abs) / dropoff_rad);
			} 
			else 
			{
				MoonLight.intensity = Mathf.Lerp(MoonLight.intensity, moonIntensityMax,0.01f);
			}

			//if (lightShaftsScript != null && LightShafts.lightShafts) 
			//{
			//	lightShaftsScript.sunTransform = Components.Moon.transform;
			//}
        }

		// Set the Fog color to sun color to match Day-Night cycle and weather
		RenderSettings.fogColor = SunLight.color * Sky.weatherMod;
    }

    // Make the parameters stay in reasonable range
    private void ValidateParameters()
    {
        // Keep GameTime Parameters right!
		GameTime.Hours = Mathf.Repeat(GameTime.Hours, 24);
		GameTime.Longitude = Mathf.Clamp(GameTime.Longitude, -180, 180);
		GameTime.Latitude = Mathf.Clamp(GameTime.Latitude, -90, 90);

		#if UNITY_EDITOR
		if (GameTime.DayLengthInMinutes == 0)
		{
			GameTime.Hours = 12f;
			Lighting.MoonPhase = 0f;
		}
		#endif

		// Give correct preview in EditoMode:
        // Sun
        #if UNITY_EDITOR
		Lighting.SunIntensity = Mathf.Max(0, Lighting.SunIntensity);
        #endif

        // Moon
        #if UNITY_EDITOR
		Lighting.MoonIntensity = Mathf.Max(0, Lighting.MoonIntensity);
		Lighting.MoonPhase = Mathf.Clamp(Lighting.MoonPhase, -1, +1);
        #endif

        // Clouds
        #if UNITY_EDITOR
        Clouds.Density = Mathf.Max(-1, Clouds.Density);
        #endif
    }

    // Convert spherical coordinates to cartesian coordinates
    private Vector3 SphericalToCartesian(float radius, float theta, float phi)
    {
        Vector3 res;

        float sinTheta = Mathf.Sin(theta);
        float cosTheta = Mathf.Cos(theta);
        float sinPhi   = Mathf.Sin(phi);
        float cosPhi   = Mathf.Cos(phi);

        res.x = radius * sinTheta * cosPhi;
        res.y = radius * cosTheta;
        res.z = radius * sinTheta * sinPhi;

        return res;
    }

	void CreateVolumeCloudMesh (int slices)
	{
		//Setting arrays up
		Vector3[] vertices = new Vector3[(Clouds.segmentCount * Clouds.segmentCount) * slices];
		Vector2[] uvMap = new Vector2[vertices.Length];
		int[] triangleConstructor = new int[(Clouds.segmentCount-1) * (Clouds.segmentCount-1) * slices * 2 * 3];
		Color[] vertexColor = new Color[vertices.Length];
		float tempRatio = 1.0f / ((float)Clouds.segmentCount - 1);
		Vector3 posGainPerVertices = new Vector3(tempRatio * 2f, 1.0f/(Mathf.Clamp(slices - 1, 1, 999999)) * Clouds.thickness, tempRatio * 2f); 
		float posGainPerUV = tempRatio;

		// Lets Create our mesh yea!
		int iteration = 0; 
		int vIncrement = 0;
		int increment = 0;
		float curvature = 0.0f;

		float depthColor = -1.0f;
		float mirrorColor = 0.0f;
		//computes slices by vertices row, each time the row ends, do the next one.
		for(int s = 0; s < slices; s++){
			depthColor = -1 + (s*(2/(float)slices));

			if(s < slices * 0.5f)
				mirrorColor = 0 + (1.0f / ((float)slices * 0.5f)) * s;
			else 				 
				mirrorColor = 2 - (1.0f / ((float)slices * 0.5f)) * (s + 1);
			
			if(slices == 1)
				mirrorColor = 1;
			//horizontal vertices
			for(int h = 0; h < Clouds.segmentCount; h++){
				int incrementV = Clouds.segmentCount * iteration;
				//vertical vertices
				for(int v = 0; v < Clouds.segmentCount; v++){

					if(Clouds.curved)
						curvature = Vector3.Distance(new Vector3(posGainPerVertices.x*v - 1f, 0.0f, posGainPerVertices.z * h - 1f), Vector3.zero);
					
					if(slices == 1)					
						vertices[v+incrementV] = new Vector3(posGainPerVertices.x*v- 1f, 0f + (Mathf.Pow(curvature, 2f) * Clouds.curvedIntensity), posGainPerVertices.z*h-1f);
					else 
						vertices[v+incrementV] = new Vector3(posGainPerVertices.x*v- 1f, posGainPerVertices.y*s-(Clouds.thickness / 2f)+(Mathf.Pow(curvature, 2f) * Clouds.curvedIntensity), posGainPerVertices.z * h - 1f);

					uvMap[v+incrementV] = new Vector2(posGainPerUV*v, posGainPerUV*h);
					vertexColor[v+incrementV] = new Vector4(depthColor, depthColor, depthColor, mirrorColor);
				}
				iteration += 1;

				//Triangle construction
				if(h >= 1){
					for(int tri = 0; tri < Clouds.segmentCount-1; tri++){
						triangleConstructor[0+increment] = (0+tri)+vIncrement+(s*Clouds.segmentCount);//
						triangleConstructor[1+increment] = (Clouds.segmentCount+tri)+vIncrement+(s*Clouds.segmentCount);
						triangleConstructor[2+increment] = (1+tri)+vIncrement+(s*Clouds.segmentCount);//
						triangleConstructor[3+increment] = ((Clouds.segmentCount+1)+tri)+vIncrement+(s*Clouds.segmentCount);
						triangleConstructor[4+increment] = (1+tri)+vIncrement+(s*Clouds.segmentCount);
						triangleConstructor[5+increment] = (Clouds.segmentCount+tri)+vIncrement+(s*Clouds.segmentCount);
						increment +=6;
					}
					vIncrement += Clouds.segmentCount;
				}
			}
		}
		if (slices > 1) 
		{
			Mesh slicedCloudMesh = new Mesh ();
			slicedCloudMesh.Clear ();
			slicedCloudMesh.name = "volumeClouds";
			slicedCloudMesh.vertices = vertices;
			slicedCloudMesh.triangles = triangleConstructor;
			slicedCloudMesh.uv = uvMap;
			slicedCloudMesh.colors = vertexColor;
			slicedCloudMesh.RecalculateNormals ();
			slicedCloudMesh.RecalculateBounds ();
			CalcMeshTangents (slicedCloudMesh);
			;
			Components.VolumeCloudsLayer1.GetComponent<MeshFilter> ().mesh = slicedCloudMesh;
			Components.VolumeCloudsLayer2.GetComponent<MeshFilter> ().mesh = slicedCloudMesh;
		} 
		else if (slices == 1) 
		{
			Mesh shadowMesh = new Mesh ();
			shadowMesh.Clear ();
			shadowMesh.name = "volumeCloudsShadows";
			shadowMesh.vertices = vertices;
			shadowMesh.triangles = triangleConstructor;
			shadowMesh.uv = uvMap;
			shadowMesh.colors = vertexColor;
			shadowMesh.RecalculateNormals ();
			shadowMesh.RecalculateBounds ();
			CalcMeshTangents (shadowMesh);
			;
			Components.VolumeCloudsShadowsLayer1.GetComponent<MeshFilter> ().mesh = shadowMesh;
			Components.VolumeCloudsShadowsLayer2.GetComponent<MeshFilter> ().mesh = shadowMesh;
		}
	}

	public static void CalcMeshTangents(Mesh mesh)
	{
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;

		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;

		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];

		Vector4[] tangents = new Vector4[vertexCount];

		for (long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];

			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];

			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];

			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;

			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;

			float r = 1.0f / (s1 * t2 - s2 * t1);

			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;

			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}


		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			Vector3.OrthoNormalize(ref n, ref t);

			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;

			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		mesh.tangents = tangents;
	}

}
