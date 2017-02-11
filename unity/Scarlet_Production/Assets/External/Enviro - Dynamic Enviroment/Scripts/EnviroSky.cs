////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////        EnviroSky- Renders a SkyDome with sun,moon,clouds and weather.          ////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class EnviroPositioning
{
	[Tooltip("When enabled, clouds will stay at this height. When disabled clouds height will be calculated by player position.")]
	public bool FixedHeight = false;
	[Tooltip("The height value of the clouds when 'FixedHeight' is enabled.")]
	public float fixedSkyHeight = 0f;
}

[Serializable]
public class EnviroQualitySettings
{
	public enum CloudQuality
	{
		None,
		OneLayer,
		TwoLayer
	}
	[Tooltip("How many layers of clouds that should be rendered.")]
	public CloudQuality CloudsQuality;
	[Tooltip("Whether clouds should cast shadows.")]
	public bool CloudsShadowCast = true;

	[Range(0,1)][Tooltip("Modifies the amount of particles used in weather effects.")]
	public float GlobalParticleEmissionRates = 1f;
	[Tooltip("How often Enviro Growth Instances should be updated. Lower value = smoother growth and more frequent updates but more perfomance hungry!")]
	public float UpdateInterval = 0.5f; //Attention: lower value = smoother growth and more frequent updates but more perfomance hungry!
}
[Serializable]
public class SeasonVariables
{
	public enum Seasons
	{
		Spring,
		Summer,
		Autumn,
		Winter,
	}
	[Tooltip("When enabled the system will change seasons automaticly when enough days passed.")]
	public bool calcSeasons; // if unticked you can manually overwrite current seas. Ticked = automaticly updates seasons
	[Tooltip("The current season.")]
	public Seasons currentSeasons;
	[HideInInspector]
	public Seasons lastSeason;
	[Tooltip("How many days in spring?")]
	public float SpringInDays = 90f;
	[Tooltip("How many days in summer?")]
	public float SummerInDays = 93f;
	[Tooltip("How many days in autumn?")]
	public float AutumnInDays = 92f;
	[Tooltip("How many days in winter?")]
	public float WinterInDays = 90f;

}

[Serializable]
public class AudioVariables // AudioSetup variables
{
	[Tooltip("The prefab with AudioSources used by Enviro. Will be instantiated at runtime.")]
	public GameObject SFXHolderPrefab;
	[Tooltip("This sound wil be played in spring at day.(looped)")]
	public AudioClip SpringDayAmbient;
	[Tooltip("This sound wil be played in spring at night.(looped)")]
	public AudioClip SpringNightAmbient;
	[Tooltip("This sound wil be played in summer at day.(looped)")]
	public AudioClip SummerDayAmbient;
	[Tooltip("This sound wil be played in summer at night.(looped)")]
	public AudioClip SummerNightAmbient;
	[Tooltip("This sound wil be played in autumn at day.(looped)")]
	public AudioClip AutumnDayAmbient;
	[Tooltip("This sound wil be played in autumn at night.(looped)")]
	public AudioClip AutumnNightAmbient;
	[Tooltip("This sound wil be played in winter at day.(looped)")]
	public AudioClip WinterDayAmbient;
	[Tooltip("This sound wil be played in winter at night.(looped)")]
	public AudioClip WinterNightAmbient;
}




[Serializable]
public class ObjectVariables // References - setup these in inspector! Or use the provided prefab.
{
	[Tooltip("The Enviro sun object.")]
	public GameObject Sun = null;
	[Tooltip("The Enviro moon object.")]
	public GameObject Moon = null;
	[Tooltip("The Enviro clouds first layer.")]
	public GameObject VolumeCloudsLayer1 = null;
	[Tooltip("The Enviro clouds first layer shadows.")]
	public GameObject VolumeCloudsShadowsLayer1 = null;
	[Tooltip("The Enviro clouds second layer.")]
	public GameObject VolumeCloudsLayer2 = null;
	[Tooltip("The Enviro clouds second layer shadows.")]
	public GameObject VolumeCloudsShadowsLayer2 = null;
	[Tooltip("The Enviro skybox material.")]
	public Material sky;
}

[Serializable]
public class Satellite 
{
	[Tooltip("Name of this satellite")]
	public string name;
	[Tooltip("Prefab with model that get instantiated.")]
	public GameObject prefab = null;
	[Tooltip("This value will influence the satellite orbit.")]
	public float orbit;
	[Tooltip("This value will influence the satellite orbit/coords.")]
	public Vector3 coords;
	[Tooltip("The speed of the satellites orbit.")]
	public float speed;
}

[Serializable]
public class EnviroWeather 
{
	[Header("Weather Setup:")]
	[Tooltip("If disabled the weather will never change.")]
	public bool updateWeather = true;
	[Tooltip("Your WindZone that reflect our weather wind settings.")]
	public WindZone windZone;
	[Tooltip("The Enviro Lighting Flash Component.")]
	public EnviroLightning LightningGenerator; // Creates lightning Flashes
	[Tooltip("A list of all possible thunder audio effects.")]
	public List<AudioClip> ThunderSFX = new List<AudioClip> ();

	[HideInInspector]public List<EnviroWeatherPrefab> WeatherTemplates = new List<EnviroWeatherPrefab>();
	[HideInInspector]public List<GameObject> WeatherPrefabs = new List<GameObject>();

	[Header("Zones:")]
	[Tooltip("List of additional zones. Will be updated on startup!")]
	public List<EnviroZone> zones = new List<EnviroZone>();
	[Tooltip("Tag for zone triggers. Create and assign a tag to this gameObject")]
	public bool useTag;

	[Header("Weather Settings:")]
	[Tooltip("Defines the speed of wetness will raise when it is raining.")]
	public float wetnessAccumulationSpeed = 0.05f;
	[Tooltip("Defines the speed of snow will raise when it is snowing.")]
	public float snowAccumulationSpeed = 0.05f;
	[Tooltip("Defines the speed of clouds and fog will change when weather conditions changed.")]
	public float cloudChangeSpeed = 0.01f;
	[Tooltip("Defines the speed of particle effects will change when weather conditions changed.")]
	public float effectChangeSpeed = 0.01f;

	[Header("Current active zone:")]
	[Tooltip("The current active zone.")]
	public EnviroZone currentActiveZone;

	[Header("Current active weather:")]
	[Tooltip("The current active weather conditions.")]
	public EnviroWeatherPrefab currentActiveWeatherID;
	[HideInInspector]public EnviroWeatherPrefab lastActiveWeatherID;

	[HideInInspector]
	public GameObject VFXHolder;
	[HideInInspector]
	public float wetness;
	[HideInInspector]
	public float curWetness;
	[HideInInspector]
	public float SnowStrenght;
	[HideInInspector]
	public float curSnowStrenght;
	[HideInInspector]
	public float nextUpdate;
	[HideInInspector]
	public int thundersfx;
	[HideInInspector]
	public AudioSource currentAudioSource;
}

[Serializable]
public class Sky 
{
	[Header("Scattering")]
	[Tooltip("Light Wavelenght used for atmospheric scattering. Keep it near defaults for earthlike atmospheres, or change for alien or fantasy atmospheres for example.")]
	public Vector3 waveLenght;
	[Tooltip("Influence atmospheric scattering.")]
	public float rayleigh = 1f;
	[Tooltip("Sky turbidity. Particle in air. Influence atmospheric scattering.")]
	public float turbidity = 1f;
	[Tooltip("Influence scattering near sun.")]
	public float mie = 1f;
	[Tooltip("Influence scattering near sun.")]
	public float g = 0.75f;
	[Tooltip("Intensity gradient for atmospheric scattering. Influence atmospheric scattering based on current time. 0h - 24h")]
	public AnimationCurve scatteringCurve;
	[Tooltip("Color gradient for atmospheric scattering. Influence atmospheric scattering based on current time. 0h - 24h")]
	public Gradient scatteringColor;

	[Header("Sun")]
	[Tooltip("Intensity of Sun Influence Scale and Dropoff of sundisk.")]
	public float SunIntensity = 50f;
	[Tooltip("Scale of rendered sundisk.")]
	public float SunDiskScale = 50f;
	[Tooltip("Intenisty of rendered sundisk.")]
	public float SunDiskIntensity = 5f;
	[Tooltip("Color gradient for sundisk. Influence sundisk color based on current time. 0h - 24h")]
	public Gradient SunDiskColor;


	[Header("Tonemapping")]
	[Tooltip("Higher values = brighter sky.")]
	public float SkyExposure = 1.5f;
	[Tooltip("Higher values = brighter sky.")]
	public float SkyLuminence = 1f;
	[Tooltip("Higher values = stronger colors.")]
	public float SkyCorrection = 2f;
	[Tooltip("Will be changed on runtime based on current weather.")]
	public Color weatherMod = Color.white;


	[Header("Stars")]
	[Tooltip("Intensity of stars on nighttime.")]
	public float StarsIntensity = 5f;
	[HideInInspector]
	public float currentStarsIntensity = 0f;
	[Tooltip("Stars flickering effect intensity.")]
	public float StarsBlinking = 5f;
}

[Serializable]
public class SatellitesVariables 
{
	[Tooltip("List of satellites.")]
	public List<Satellite> additionalSatellites = new List<Satellite>();
}

[Serializable]
public class TimeVariables // GameTime variables
{
	[Header("Date and Time")]
	[Tooltip("Wether time should pass. Disable to stop time.")]
	public bool ProgressTime = true;
	[Tooltip("Current Time: hour")]
    public float Hours  = 12f;  // 0 -  24 Hours day
	[Tooltip("Current Time: Days")]
    public float Days = 1f; //  1 - 365 Days of the year
	[Tooltip("Current Time: Years")]
	public float Years = 1f; // Count the Years maybe usefull!
	[Tooltip("How long a day needs in real minutes.")]
	public float DayLengthInMinutes = 30; // DayLenght in realtime minutes
	[Range(0,24)]	[Tooltip("Time when OnNightEvent and Audio will be played.")]
	public float NightTimeInHours = 18f; 
	[Range(0,24)] [Tooltip("Time when OnDayEvent and Audio will be played.")]
	public float MorningTimeInHours = 5f; 
	[Header("Location")]
	[Range(-90,90)] [Tooltip("-90,  90   Horizontal earth lines")]
    public float Latitude   = 0f;   // -90,  90   Horizontal earth lines
	[Range(-180,180)] [Tooltip("-180, 180  Vertical earth line")]
    public float Longitude  = 0f;   // -180, 180  Vertical earth line

}

[Serializable] 
public class LightVariables // All Lightning Variables
{
	[Header("Direct")]
	[Tooltip("Color gradient for sun and moon light based on current time. 0h - 24h")]
	public Gradient LightColor;
	[Tooltip("Sunlight intensity.")]
    public float SunIntensity = 2f;
	[Tooltip("Moonlight intensity.")]
	public float MoonIntensity = 0.5f;
	[Tooltip("Current Moon phase.-1f - 1f")]
	public float MoonPhase = 0.0f;
	[Header("Ambient")]
	[Tooltip("Ambient Rendering Mode.")]
	public UnityEngine.Rendering.AmbientMode ambientMode;
	[Tooltip("Ambientlight ntensity based on current time. 0h - 24h")]
	public AnimationCurve ambientIntensity;
	[Tooltip("Ambientlight sky color.")]
	public Gradient ambientSkyColor;
	[Tooltip("Ambientlight Equator color.")]
	public Gradient ambientEquatorColor;
	[Tooltip("Ambientlight Ground color.")]
	public Gradient ambientGroundColor;
	[HideInInspector]public float SunWeatherMod = 0.0f;
}


[Serializable]
public class FogSettings 
{
	[Tooltip("Unity's fog mode.")]
	public FogMode Fogmode;
	[Tooltip("Use the enviro fog image effect?")]
	public bool AdvancedFog = true;

	[Header("Fog Material")]
	[Tooltip("The Enviro fog material.")]
	public Material fogMaterial;

	[Header("Distance Fog")]
	[Tooltip("Use distance fog?")]
	public bool distanceFog = true;
	[Tooltip("Use radial distance fog?")]
	public bool useRadialDistance = false;
	[Tooltip("The distance where fog starts.")]
	public float startDistance = 0.0f;

	[Range(0f,100f)][Tooltip("The intensity of distance fog.")]
	public float distanceFogIntensity = 0.0f;

	[Header("Height Fog")]
	[Tooltip("Use heightbased fog?")]
	public bool  heightFog = true;
	[Tooltip("The height of heightbased fog.")]
	public float height = 90.0f;
	[Range(0f,1f)][Tooltip("The intensity of heightbased fog.")]
	public float heightDensity = 0.15f;
	[Range(0f,1f)][Tooltip("The noise intensity of heightbased fog.")]
	public float noiseIntensity = 0.01f;
	[Range(0f,0.1f)][Tooltip("The noise scale of heightbased fog.")]
	public float noiseScale = 0.1f;
	[Tooltip("The speed and direction of heightbased fog.")]
	public Vector2 heightFogVelocity;

	[Header("Fog Scattering")]
	[Range(0f,1f)][Tooltip("This value influence fog from horizon to top.")]
	public float skyFogHeight = 1f;
	[Range(0f,1f)][Tooltip("This value influence fogstrength in sky.")]
	public float skyFogStrength = 0.1f;
	[Range(0f,1f)][Tooltip("This value influence fog color based on current time of day.")]
	public float scatteringStrenght = 0.5f;
}

[Serializable]
public class LightShaftsSettings 
{
	[Tooltip("Use light shafts?")]
	public bool lightShafts = true;

	[Header("Quality Settings")]
	[Tooltip("Lightshafts resolution quality setting.")]
	public EnviroLightShafts.SunShaftsResolution resolution = EnviroLightShafts.SunShaftsResolution.Normal;
	[Tooltip("Lightshafts blur mode.")]
	public EnviroLightShafts.ShaftsScreenBlendMode screenBlendMode = EnviroLightShafts.ShaftsScreenBlendMode.Screen;
	[Tooltip("Use cameras depth to hide lightshafts?")]
	public bool useDepthTexture = true;

	[Header("Intensity Settings")]
	[Tooltip("Color gradient for lightshafts based on current time. 0h - 24h")]
	public Gradient lighShaftsColor;
	[Tooltip("Treshhold gradient for lightshafts based on current time. This will influence lightshafts intensity! 0h - 24h")]
	public Gradient thresholdColor;
	[Tooltip("Radius of blurring applied.")]
	public float blurRadius = 2.5f;
	[Tooltip("Global Lightshafts intensity.")]
	public float intensity = 1.15f;
	[Tooltip("Lightshafts maximum radius.")]
	public float maxRadius = 0.75f;

	[Header("Materials")]
	[Tooltip("The Enviro Lightshaft material.")]
	public Material lightShaftsMaterial;
	[Tooltip("The Enviro Lightshaft clearing material.")]
	public Material clearMaterial;
}

[Serializable]
public class CloudVariables // Default cloud settings, will be changed on runtime if Weather is enabled!
{
	[Header("Setup")]
	[Range(10,200)][Tooltip("Clouds Quality. High Performance Impact! Call InitClouds() to apply change in runtime.")]
	public int Quality = 25;	
	[Tooltip("Segments of generated clouds mesh. Good for curvate meshes. If curved disabled keep it low.")]
	public int segmentCount = 3;
	[Tooltip("Thickness of generated clouds mesh.")]
	public float thickness = 0.4f;
	[Tooltip("Clouds mesh curved at horizon?")]
	public bool curved;
	[Tooltip("Clouds mesh curve intensity.")]
	public float curvedIntensity = 0.001f;
	[Range(0.5f,2f)][Tooltip("Clouds tiling/scale modificator.")]
	public float Scaling = 1f; 

	[Header("Runtime Settings")]
	[Tooltip("Base color of clouds.")]
	public Color FirstColor;
	[Tooltip("Shading color of clouds.")]
	public Color SecondColor;
	[Tooltip("Coverage rate of clouds generated.")]
    public float Coverage = 1.0f; // 
	[Tooltip("Density of clouds generated.")]
	public float Density = 1.0f; 
	[Tooltip("Clouds alpha modificator.")]
	public float Alpha = 0.5f;
	[Tooltip("Clouds morph speed modificator.")]
	public float Speed1    = 0.5f; // Animate speed1 of clouds
	[Tooltip("Clouds morph speed modificator.")]
	public float Speed2    = 1.0f; // Animate speed2 of clouds
	[Tooltip("Global wind direction.")]
	public Vector2 WindDir = new Vector2 (1f, 1f);
	[Tooltip("Offset used for multi layer clouds rendering.")]
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
	private static EnviroSky _instance; // Creat a static instance for easy access!

	public static EnviroSky instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<EnviroSky>();
			return _instance;
		}
	}
		
	[Tooltip("Assign your player gameObject here. Required Field! or enable AssignInRuntime!")]
	public GameObject Player;
	[Tooltip("Assign your main camera here. Required Field! or enable AssignInRuntime!")]
	public Camera PlayerCamera;
	[Tooltip("If enabled Enviro will search for your Player and Camera by Tag!")]
	public bool AssignInRuntime;
	[Tooltip("Your Player Tag")]
	public string PlayerTag = "";
	[Tooltip("Your CameraTag")]
	public string CameraTag = "MainCamera";


    // Parameters
	public EnviroPositioning Position = null;
	public ObjectVariables Components = null;
	public AudioVariables Audio = null;
	public TimeVariables GameTime = null;
	public LightVariables Lighting = null;
	public Sky  Sky = null;
	public EnviroWeather Weather = null;
	public SeasonVariables Seasons = null;
	public CloudVariables Clouds = null;
	public FogSettings Fog = null;
	public LightShaftsSettings LightShafts = null;
	public SatellitesVariables Satellites = null;
	public EnviroQualitySettings Quality = null;

	[HideInInspector]
	public List<Transform> mySatellites = new List<Transform>();
	[HideInInspector]
	public bool started;
	// Private Variables
	private bool isNight = true;

	[HideInInspector]
	public EnviroFog atmosphericFog;
	[HideInInspector]
	public EnviroLightShafts lightShaftsScript;
	[HideInInspector]
	public GameObject EffectsHolder;
	[HideInInspector]
	public AudioSource AudioSourceWeather;
	[HideInInspector]
	public AudioSource AudioSourceWeather2;
	[HideInInspector]
	public AudioSource AudioSourceAmbient;
	[HideInInspector]
	public AudioSource AudioSourceThunder;

	[HideInInspector]
	public List<EnviroVegetationInstance> EnviroVegetationInstances = new List<EnviroVegetationInstance>(); // All EnviroInstance that getting updated at the moment.

	// Used from other Enviro componets
	[HideInInspector]
	public float currentHour;
	[HideInInspector]
	public float currentDay;
	[HideInInspector]
	public float currentYear;
	[HideInInspector]
	public float currentTimeInHours;

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

	private float lastHourUpdate;
	private float starsRot;
	private float lastHour;
	private float OrbitRadius
    {
        get { return DomeTransform.localScale.x; }
    }

	// Scattering constants
    const float pi = Mathf.PI;
	private Vector3 K =  new Vector3(686.0f, 678.0f, 666.0f);
	private const float n =  1.0003f;   
	private const float N =  2.545E25f;
	private const float pn =  0.035f;    

	// Events
	public delegate void HourPassed();
	public delegate void DayPassed();
	public delegate void YearPassed();
	public delegate void WeatherChanged(EnviroWeatherPrefab weatherType);
	public delegate void ZoneWeatherChanged(EnviroWeatherPrefab weatherType,EnviroZone zone);
	public delegate void SeasonChanged(SeasonVariables.Seasons season);
	public delegate void isNightE();
	public delegate void isDay();
	public event HourPassed OnHourPassed;
	public event DayPassed OnDayPassed;
	public event YearPassed OnYearPassed;
	public event WeatherChanged OnWeatherChanged;
	public event ZoneWeatherChanged OnZoneWeatherChanged;
	public event SeasonChanged OnSeasonChanged;
	public event isNightE OnNightTime;
	public event isDay OnDayTime;
	///

    void Start()
    {
		started = false;
		lastHourUpdate = GameTime.Hours;
		currentTimeInHours = GetInHours (GameTime.Hours, GameTime.Days, GameTime.Years);
		RemoveSatellites ();
		CreateSatellites ();
		lastHourUpdate = currentTimeInHours;

		//Weather Setup
		Weather.currentActiveWeatherID = Weather.zones[0].zoneWeatherPrefabs[0].GetComponent<EnviroWeatherPrefab>();
		Weather.lastActiveWeatherID = Weather.zones[0].zoneWeatherPrefabs[0].GetComponent<EnviroWeatherPrefab>();//Weather.zones[0].zoneWeather[0];
		Weather.nextUpdate = currentTimeInHours + 1f;
		Weather.currentAudioSource = AudioSourceWeather; 
		InvokeRepeating("UpdateEnviroment", 0, Quality.UpdateInterval);
		if(PlayerCamera != null && Player != null && AssignInRuntime == false)
		 {
				Init ();
				CreateEffects ();  //Create Weather Effects and go!
				started = true;
			}
	}

    void OnEnable()
    {
        DomeTransform = transform;

		if (AssignInRuntime) 
		{
			started = false;	//Wait for assignment
		} 
		else if (PlayerCamera != null && Player != null)
		{
			Init ();
			CreateEffects ();  //Create Weather Effects and go!
			started = true;
		}
    }

	void Init ()
	{
		atmosphericFog = PlayerCamera.gameObject.GetComponent<EnviroFog> ();

		if (atmosphericFog == null) 
		{
			atmosphericFog = PlayerCamera.gameObject.AddComponent<EnviroFog> ();
			atmosphericFog.fogMaterial = Fog.fogMaterial;
			atmosphericFog.fogShader = Fog.fogMaterial.shader;
		}

		lightShaftsScript = PlayerCamera.gameObject.GetComponent<EnviroLightShafts> ();

		if (lightShaftsScript == null) 
		{
			lightShaftsScript = PlayerCamera.gameObject.AddComponent<EnviroLightShafts> ();
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
		if(PlayerCamera != null)
			PlayerCamera.clearFlags = CameraClearFlags.Skybox;

		InitClouds ();

		if (Components.Sun)
		{
			SunTransform = Components.Sun.transform;
			SunLight = Components.Sun.GetComponent<Light>();
		}
		else
		{
			Debug.LogError("Please set Sun object in inspector!");
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
		}
	}

	/// <summary>
	/// Assign your Player and Camera and Initilize.////
	/// </summary>
	public void AssignAndStart (GameObject player, Camera Camera)
	{
		this.Player = player;
		PlayerCamera = Camera;
		Init ();
		CreateEffects ();
		started = true;
	}

/// <summary>
/// Changes focus on other Player or Camera on runtime.////
/// </summary>
/// <param name="Player">Player.</param>
/// <param name="Camera">Camera.</param>
	public void ChangeFocus (GameObject player, Camera Camera)
	{
		this.Player = player;
		PlayerCamera = Camera;

		atmosphericFog = PlayerCamera.gameObject.GetComponent<EnviroFog> ();

		if (atmosphericFog == null) 
		{
			atmosphericFog = PlayerCamera.gameObject.AddComponent<EnviroFog> ();
			atmosphericFog.fogMaterial = Fog.fogMaterial;
			atmosphericFog.fogShader = Fog.fogMaterial.shader;
		}

		lightShaftsScript = PlayerCamera.gameObject.GetComponent<EnviroLightShafts> ();

		if (lightShaftsScript == null) 
		{
			lightShaftsScript = PlayerCamera.gameObject.AddComponent<EnviroLightShafts> ();
			lightShaftsScript.sunShaftsMaterial = LightShafts.lightShaftsMaterial;
			lightShaftsScript.sunShaftsShader = LightShafts.lightShaftsMaterial.shader;
			lightShaftsScript.simpleClearMaterial = LightShafts.clearMaterial;
			lightShaftsScript.simpleClearShader = LightShafts.clearMaterial.shader;
		}
	}
	/// <summary>
	/// Recalculate clouds.////
	/// </summary>
	public void InitClouds ()
	{
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
		}

		if (Components.VolumeCloudsShadowsLayer1)
		{
			VolumeCloudShadowShader1 = Components.VolumeCloudsShadowsLayer1.GetComponent<Renderer>().sharedMaterial;
		}
		else if (Components.VolumeCloudsShadowsLayer1 == null)
		{
			Debug.LogError("Please set VolumeCloudsShadowsLayer1 object in inspector!");
		}

		if (Components.VolumeCloudsShadowsLayer2)
		{
			VolumeCloudShadowShader2 = Components.VolumeCloudsShadowsLayer2.GetComponent<Renderer>().sharedMaterial;
		}
		else if (Components.VolumeCloudsShadowsLayer2 == null)
		{
			Debug.LogError("Please set VolumeCloudsShadowsLayer2 object in inspector!");
		}
	}

	// Events:
	public virtual void NotifyHourPassed()
	{
		if(OnHourPassed != null)
			OnHourPassed();
	}
	public virtual void NotifyDayPassed()
	{
		if(OnDayPassed != null)
			OnDayPassed();
	}
	public virtual void NotifyYearPassed()
	{
		if(OnYearPassed != null)
			OnYearPassed();
	}
	public virtual void NotifyWeatherChanged(EnviroWeatherPrefab type)
	{
		if(OnWeatherChanged != null)
			OnWeatherChanged (type);
	}
	public virtual void NotifyZoneWeatherChanged(EnviroWeatherPrefab type, EnviroZone zone)
	{
		if(OnZoneWeatherChanged != null)
			OnZoneWeatherChanged (type,zone);
	}
	public virtual void NotifySeasonChanged(SeasonVariables.Seasons season)
	{
		if(OnSeasonChanged != null)
			OnSeasonChanged (season);
	}
	public virtual void NotifyIsNight()
	{
		if(OnNightTime != null)
			OnNightTime ();
	}
	public virtual void NotifyIsDay()
	{
		if(OnDayTime != null)
			OnDayTime ();
	}

	public void CreateEffects ()
	{
		if (EffectsHolder == null) {
			EffectsHolder = new GameObject ();
			EffectsHolder.name = "Enviro Effects";
			//EffectsHolder.transform.parent = Player.transform;
			EffectsHolder.transform.position = Player.transform.position;


		CreateWeatherEffectHolder ();

		GameObject SFX = (GameObject)Instantiate (Audio.SFXHolderPrefab, Vector3.zero, Quaternion.identity);

		SFX.transform.parent = EffectsHolder.transform;

		EnviroAudioSource[] srcs = SFX.GetComponentsInChildren<EnviroAudioSource> ();

		for (int i = 0; i < srcs.Length; i++) 
		{
			switch (srcs [i].myFunction) {

			case EnviroAudioSource.AudioSourceFunction.Weather1:
				AudioSourceWeather = srcs [i].audiosrc;
				break;

			case EnviroAudioSource.AudioSourceFunction.Weather2:
				AudioSourceWeather2 = srcs [i].audiosrc;
				break;

			case EnviroAudioSource.AudioSourceFunction.Ambient:
				AudioSourceAmbient = srcs [i].audiosrc;
				break;

			case EnviroAudioSource.AudioSourceFunction.Thunder:
				AudioSourceThunder = srcs [i].audiosrc;
				break;
			}
		}
		}
	}
		
	// EnviroGrowInstancesSeason Component will register on start!
	public int RegisterMe (EnviroVegetationInstance me) 
	{
		EnviroVegetationInstances.Add (me);
		return EnviroVegetationInstances.Count - 1;
	}

	// Manuell change season.
	public void ChangeSeason (SeasonVariables.Seasons season)
	{
		Seasons.currentSeasons = season;
		NotifySeasonChanged (season);
	}

	// Update the Season according gameDays
	void UpdateSeason ()
	{
		if (currentDay >= 0 && currentDay < Seasons.SpringInDays)
		{
			Seasons.currentSeasons = SeasonVariables.Seasons.Spring;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Spring);

			Seasons.lastSeason = Seasons.currentSeasons;
		} 
		else if (currentDay >= Seasons.SpringInDays && currentDay < (Seasons.SpringInDays + Seasons.SummerInDays))
		{
			Seasons.currentSeasons = SeasonVariables.Seasons.Summer;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Summer);

			Seasons.lastSeason = Seasons.currentSeasons;
		} 
		else if (currentDay >= (Seasons.SpringInDays + Seasons.SummerInDays) && currentDay < (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays)) 
		{
			Seasons.currentSeasons = SeasonVariables.Seasons.Autumn;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Autumn);

			Seasons.lastSeason = Seasons.currentSeasons;
		}
		else if(currentDay >= (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays) && currentDay <= (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays + Seasons.WinterInDays))
		{
			Seasons.currentSeasons = SeasonVariables.Seasons.Winter;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Winter);

			Seasons.lastSeason = Seasons.currentSeasons;
		}
	}

	public float GetInHours (float hours,float days, float years)
	{
		float inHours  = hours + (days*24f) + ((years * (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays + Seasons.WinterInDays)) * 24f);
		return inHours;
	}


	void UpdateEnviroment () // Update the all GrowthInstances
	{
		// Set correct Season.
		if(Seasons.calcSeasons)
			UpdateSeason ();

		// Update all EnviroGrowInstancesSeason in scene!
		if (EnviroVegetationInstances.Count > 0) 
		{
			for (int i = 0; i < EnviroVegetationInstances.Count; i++) {
				if (EnviroVegetationInstances [i] != null)
					EnviroVegetationInstances [i].UpdateInstance ();

			}
		}
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
		switch (Quality.CloudsQuality) {

			case EnviroQualitySettings.CloudQuality.None:
			Components.VolumeCloudsLayer1.SetActive (false);
			Components.VolumeCloudsLayer2.SetActive (false);
			Components.VolumeCloudsShadowsLayer1.SetActive (false);
			Components.VolumeCloudsShadowsLayer2.SetActive (false);
			break;

			case EnviroQualitySettings.CloudQuality.OneLayer:
			Components.VolumeCloudsLayer1.SetActive (true);
			Components.VolumeCloudsLayer2.SetActive (false);

			if (Quality.CloudsShadowCast) 
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
			if (Quality.CloudsShadowCast) 
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
		newPosition.x = Player.transform.position.x;
		newPosition.z = Player.transform.position.z;
		newPosition.y = Position.fixedSkyHeight;

		return newPosition;
	}

	void UpdateSatellites ()
	{
		for (int i = 0; i < mySatellites.Count; i++)
		{
			float s = (currentTimeInHours - lastHour) * Satellites.additionalSatellites [i].speed;
			mySatellites [i].RotateAround (transform.position, Satellites.additionalSatellites [i].coords, s);

			Vector3 desiredPos = (mySatellites [i].position - transform.position).normalized * Satellites.additionalSatellites [i].orbit + transform.position;
			mySatellites [i].position = Vector3.Slerp (mySatellites [i].position,desiredPos, Time.deltaTime * 5f);
		}

		lastHour = currentTimeInHours;
	}

    void Update()
    {
		if (!started) {
			if (AssignInRuntime && PlayerTag != "" && CameraTag != "" && Application.isPlaying) {
				Player = GameObject.FindGameObjectWithTag (PlayerTag);
				PlayerCamera = GameObject.FindGameObjectWithTag (CameraTag).GetComponent<Camera>();

				if (Player != null && PlayerCamera != null) {
					Init ();
					CreateEffects ();
					started = true;
				}
				else  {started = false; return;}
			} else {started = false; return;}
		}

        UpdateTime();
		ValidateParameters();
		UpdateQuality ();
		UpdateAmbientLight ();
		UpdateWeather ();

		if(EffectsHolder != null)
			EffectsHolder.transform.position = Player.transform.position;

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

		if (PlayerCamera != null)
        {
			if (!Position.FixedHeight)
				transform.position = PlayerCamera.transform.position;
			else 
			{
				transform.position = CalculatePosition ();
			}

			transform.localScale = new Vector3(PlayerCamera.farClipPlane - (PlayerCamera.farClipPlane * 0.1f), PlayerCamera.farClipPlane - (PlayerCamera.farClipPlane * 0.1f), PlayerCamera.farClipPlane - (PlayerCamera.farClipPlane * 0.1f));
        }

		if (!isNight && (GameTime.Hours >= GameTime.NightTimeInHours || GameTime.Hours <= GameTime.MorningTimeInHours) && AudioSourceAmbient!=null)
		{
			switch (Seasons.currentSeasons)
			{
			case SeasonVariables.Seasons.Spring:
				AudioSourceAmbient.clip = Audio.SpringNightAmbient;
				break;
				
			case SeasonVariables.Seasons.Summer:
				AudioSourceAmbient.clip = Audio.SummerNightAmbient;
				break;
				
			case SeasonVariables.Seasons.Autumn:
				AudioSourceAmbient.clip = Audio.AutumnNightAmbient;
				break;
				
			case SeasonVariables.Seasons.Winter:
				AudioSourceAmbient.clip = Audio.WinterNightAmbient;
				break;
			}
			
			AudioSourceAmbient.loop = true;
			AudioSourceAmbient.Play();
			isNight = true;

			NotifyIsNight ();

		}
		else if (isNight && (GameTime.Hours <= GameTime.NightTimeInHours && GameTime.Hours >= GameTime.MorningTimeInHours)&& AudioSourceAmbient!=null)
		{
			
			switch (Seasons.currentSeasons)
			{
			case SeasonVariables.Seasons.Spring:
				AudioSourceAmbient.clip = Audio.SpringDayAmbient;
				break;
				
			case SeasonVariables.Seasons.Summer:
				AudioSourceAmbient.clip = Audio.SummerDayAmbient;
				break;
				
			case SeasonVariables.Seasons.Autumn:
				AudioSourceAmbient.clip = Audio.AutumnDayAmbient;
				break;
				
			case SeasonVariables.Seasons.Winter:
				AudioSourceAmbient.clip = Audio.WinterDayAmbient;
				break;
			}

			AudioSourceAmbient.loop = true;
			AudioSourceAmbient.Play();
			isNight = false;

			NotifyIsDay ();
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
			
		if (VolumeCloudShader1 != null && Quality.CloudsQuality == EnviroQualitySettings.CloudQuality.OneLayer)
        {
			VolumeCloudShader1.SetFloat("_Scale", PlayerCamera.farClipPlane * Clouds.Scaling);
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

		if (VolumeCloudShader1 != null && VolumeCloudShader2 != null && Quality.CloudsQuality == EnviroQualitySettings.CloudQuality.TwoLayer)
		{
			VolumeCloudShader1.SetFloat("_Scale", PlayerCamera.farClipPlane * Clouds.Scaling);
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

			VolumeCloudShader2.SetFloat("_Scale", PlayerCamera.farClipPlane * Clouds.Scaling);
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

		if (VolumeCloudShadowShader1 != null && Quality.CloudsShadowCast && Quality.CloudsQuality == EnviroQualitySettings.CloudQuality.OneLayer)
        {
			VolumeCloudShadowShader1.SetFloat("_Scale", PlayerCamera.farClipPlane * Clouds.Scaling);
			VolumeCloudShadowShader1.SetFloat("_CloudCover", Clouds.Coverage);
			VolumeCloudShadowShader1.SetFloat("_CloudAlpha", Clouds.Alpha);
			VolumeCloudShadowShader1.SetFloat("_Speed1Layer", Clouds.Speed1);
			VolumeCloudShadowShader1.SetFloat("_Speed2Layer", Clouds.Speed2);
			VolumeCloudShadowShader1.SetVector("_WindDir", Clouds.WindDir);
        }

		if (VolumeCloudShadowShader2 != null && Quality.CloudsShadowCast && Quality.CloudsQuality == EnviroQualitySettings.CloudQuality.TwoLayer)
		{
			VolumeCloudShadowShader1.SetFloat("_Scale", PlayerCamera.farClipPlane * Clouds.Scaling);
			VolumeCloudShadowShader1.SetFloat("_CloudCover", Clouds.Coverage);
			VolumeCloudShadowShader1.SetFloat("_CloudAlpha", Clouds.Alpha);
			VolumeCloudShadowShader1.SetFloat("_Speed1Layer", Clouds.Speed1);
			VolumeCloudShadowShader1.SetFloat("_Speed2Layer", Clouds.Speed2);
			VolumeCloudShadowShader1.SetVector("_WindDir", Clouds.WindDir);

			VolumeCloudShadowShader2.SetFloat("_Scale", PlayerCamera.farClipPlane * Clouds.Scaling);
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
		if (!started)
			return;
		
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
				NotifyHourPassed ();
			}

			if (GameTime.Hours >= 24)
            {
				GameTime.Hours = 0;
				NotifyHourPassed ();
				lastHourUpdate = 0f;
				GameTime.Days = GameTime.Days + 1;
				NotifyDayPassed ();
            }

			if(GameTime.Days >= (Seasons.SpringInDays + Seasons.SummerInDays + Seasons.AutumnInDays + Seasons.WinterInDays))
			{
				GameTime.Years = GameTime.Years + 1;
				GameTime.Days = 0;
				NotifyYearPassed ();
			}

			currentHour = GameTime.Hours;
			currentDay = GameTime.Days;
			currentYear = GameTime.Years;

			currentTimeInHours = GetInHours (currentHour, currentDay, currentYear);
        }
    }

	void UpdateAmbientLight ()
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


	///////////////////////////////////////////////////////////////////cloud meshes/////////////////////////////////////////////////////////////////////////
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
		
	///////////////////////////////////////////////////////////////////WEATHER SYSTEM /////////////////////////////////////////////////////////////////////////
	public void RegisterZone (EnviroZone zoneToAdd)
	{
		Weather.zones.Add (zoneToAdd);
	}


	public void EnterZone (EnviroZone zone)
	{
		Weather.currentActiveZone = zone;
	}

	public void ExitZone ()
	{

	}

	public void CreateWeatherEffectHolder()
	{
		if (Weather.VFXHolder == null) {
			GameObject VFX = new GameObject ();
			VFX.name = "VFX";
			VFX.transform.parent = EffectsHolder.transform;
			VFX.transform.localPosition = Vector3.zero;
			Weather.VFXHolder = VFX;
		}
	}

	void UpdateAudioSource (EnviroWeatherPrefab i)
	{
		if (i != null && i.Sfx != null)
		{
			if (i.Sfx == Weather.currentAudioSource.clip)
			{
				Weather.currentAudioSource.GetComponent<EnviroAudioSource>().FadeIn(i.Sfx);
				return;
			}

			if (Weather.currentAudioSource == AudioSourceWeather)
			{
				AudioSourceWeather.GetComponent<EnviroAudioSource>().FadeOut();
				AudioSourceWeather2.GetComponent<EnviroAudioSource>().FadeIn(i.Sfx);
				Weather.currentAudioSource = AudioSourceWeather2;
			}
			else if (Weather.currentAudioSource == AudioSourceWeather2)
			{
				AudioSourceWeather2.GetComponent<EnviroAudioSource>().FadeOut();
				AudioSourceWeather.GetComponent<EnviroAudioSource>().FadeIn(i.Sfx);
				Weather.currentAudioSource = AudioSourceWeather;
			}
		} 
		else
		{
			AudioSourceWeather.GetComponent<EnviroAudioSource>().FadeOut();
			AudioSourceWeather2.GetComponent<EnviroAudioSource>().FadeOut();
		}
	}

	void UpdateClouds (EnviroWeatherPrefab i)
	{
		if (i != null) {
			Lighting.SunWeatherMod = i.sunLightMod;
			Clouds.FirstColor = Color.Lerp (Clouds.FirstColor, i.cloudConfig.FirstColor, Weather.cloudChangeSpeed);
			Clouds.SecondColor = Color.Lerp (Clouds.SecondColor, i.cloudConfig.SecondColor, Weather.cloudChangeSpeed);
			Clouds.DirectLightIntensity = Mathf.Lerp (Clouds.DirectLightIntensity, i.cloudConfig.DirectLightInfluence, Weather.cloudChangeSpeed);
			Clouds.AmbientLightIntensity = Mathf.Lerp (Clouds.AmbientLightIntensity, i.cloudConfig.AmbientLightInfluence, Weather.cloudChangeSpeed);
			Clouds.Coverage = Mathf.Lerp (Clouds.Coverage, i.cloudConfig.Coverage, Weather.cloudChangeSpeed);
			Clouds.Density = Mathf.Lerp (Clouds.Density, i.cloudConfig.Density, Weather.cloudChangeSpeed);
			Clouds.Alpha = Mathf.Lerp (Clouds.Alpha, i.cloudConfig.Alpha, Weather.cloudChangeSpeed);
			Clouds.Speed1 = Mathf.Lerp (Clouds.Speed1, i.cloudConfig.Speed1, (0.00025f) * Time.deltaTime);
			Clouds.Speed2 = Mathf.Lerp (Clouds.Speed2, i.cloudConfig.Speed2, (0.00025f) * Time.deltaTime);
			Sky.weatherMod = Color.Lerp (Sky.weatherMod, i.WeatherColorMod, Weather.cloudChangeSpeed);
		}
	}


	void UpdateFog (EnviroWeatherPrefab i)
	{
		if (i != null) {
			if (Fog.Fogmode == FogMode.Linear) {
				RenderSettings.fogEndDistance = Mathf.Lerp (RenderSettings.fogEndDistance, i.fogDistance, 0.01f);
				RenderSettings.fogStartDistance = Mathf.Lerp (RenderSettings.fogStartDistance, i.fogStartDistance, 0.01f);
			} else
				RenderSettings.fogDensity = Mathf.Lerp (RenderSettings.fogDensity, i.fogDensity, 0.01f);

			if (Fog.AdvancedFog) {
				Fog.skyFogHeight = Mathf.Lerp (Fog.skyFogHeight, i.SkyFogHeight, Weather.cloudChangeSpeed);
				Fog.skyFogStrength = Mathf.Lerp (Fog.skyFogStrength, i.SkyFogIntensity, Weather.cloudChangeSpeed);
				Fog.scatteringStrenght = Mathf.Lerp (Fog.scatteringStrenght, i.FogScatteringIntensity, Weather.cloudChangeSpeed);
			}
		}
	}

	void UpdateEffectSystems (EnviroWeatherPrefab id)
	{

		if (id != null) {
			for (int i = 0; i < id.effectParticleSystems.Count; i++) {
				// Set EmissionRate
				float val = Mathf.Lerp (GetEmissionRate (id.effectParticleSystems [i]), id.effectEmmisionRates [i] * Quality.GlobalParticleEmissionRates, Weather.effectChangeSpeed);
				SetEmissionRate (id.effectParticleSystems [i], val);
			}

			for (int i = 0; i < Weather.WeatherTemplates.Count; i++) {
				if (Weather.WeatherTemplates [i].gameObject != id.gameObject) {
					for (int i2 = 0; i2 < Weather.WeatherTemplates [i].effectParticleSystems.Count; i2++) {
						float val2 = Mathf.Lerp (GetEmissionRate (Weather.WeatherTemplates [i].effectParticleSystems [i2]), 0f, Weather.effectChangeSpeed);

						if (val2 < 1f)
							val2 = 0f;
					
						SetEmissionRate (Weather.WeatherTemplates [i].effectParticleSystems [i2], val2);
					}
				}
			}

			Weather.windZone.windMain = id.WindStrenght; // Set Wind Strenght

			Weather.curWetness = Weather.wetness;
			Weather.wetness = Mathf.Lerp (Weather.curWetness, id.wetnessLevel, Weather.wetnessAccumulationSpeed * Time.deltaTime);
			Weather.wetness = Mathf.Clamp (Weather.wetness, 0f, 1f);

			Weather.curSnowStrenght = Weather.SnowStrenght;
			Weather.SnowStrenght = Mathf.Lerp (Weather.curSnowStrenght, id.snowLevel, Weather.snowAccumulationSpeed * Time.deltaTime);
			Weather.SnowStrenght = Mathf.Clamp (Weather.SnowStrenght, 0f, 1f);
		}
	}

	public static float GetEmissionRate (ParticleSystem system)
	{
		return system.emission.rate.constantMax;
	}


	public static void SetEmissionRate (ParticleSystem sys, float emissionRate)
	{
		var emission = sys.emission;
		var rate = emission.rate;
		rate.constantMax = emissionRate;
		emission.rate = rate;
	}

	IEnumerator PlayThunderRandom()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(10,20));
		int i = UnityEngine.Random.Range(0,Weather.ThunderSFX.Count);
		AudioSourceThunder.clip = Weather.ThunderSFX[i];
		AudioSourceThunder.loop = false;
		AudioSourceThunder.Play ();
		Weather.LightningGenerator.Lightning ();
		Weather.thundersfx = 0;
	}

	public void SetWeatherOverwrite (int weatherId)
	{
		if (Weather.WeatherTemplates[weatherId] != Weather.currentActiveWeatherID)
		{
			Weather.currentActiveZone.currentActiveZoneWeatherID = Weather.WeatherTemplates[weatherId];
		}
	}
		
	void UpdateWeather ()
	{	

			//Current active weather not matching current zones weather:
			if(Weather.currentActiveWeatherID != Weather.currentActiveZone.currentActiveZoneWeatherID)
				{
					Weather.lastActiveWeatherID = Weather.currentActiveWeatherID;
					Weather.currentActiveWeatherID = Weather.currentActiveZone.currentActiveZoneWeatherID;
			if (Weather.currentActiveWeatherID != null) {
				NotifyWeatherChanged (Weather.currentActiveWeatherID);
				UpdateAudioSource (Weather.currentActiveWeatherID);
			}
				}

		if (Weather.currentActiveWeatherID != null) 
			{
			UpdateClouds (Weather.currentActiveWeatherID);
			UpdateFog (Weather.currentActiveWeatherID);
			UpdateEffectSystems (Weather.currentActiveWeatherID);

			//Play ThunderSFX
			if (Weather.thundersfx == 0 && Weather.currentActiveWeatherID && Weather.currentActiveWeatherID.isLightningStorm)
			{
				Weather.thundersfx = 1;
				StartCoroutine(PlayThunderRandom());
			}
		}
	}
}


