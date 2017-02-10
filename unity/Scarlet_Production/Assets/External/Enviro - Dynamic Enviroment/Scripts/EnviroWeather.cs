////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////              EnviroWeather - Customizable Weather Engine                            ///////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Enviro/Weather Manager")]
public class EnviroWeather : MonoBehaviour {

    [HideInInspector]
    public List<EnviroWeatherPrefab> WeatherTemplates = new List<EnviroWeatherPrefab>();
	[Header("Weather Setup")]
	public List<GameObject> WeatherEffectsPrefabs = new List<GameObject>();
	public WindZone windZone;
	public List<AudioClip> ThunderSFX = new List<AudioClip> ();
	public EnviroLightning LightningGenerator; // Creates lightning Flashes

	[Header("Weather Settings")]
	public float WeatherUpdateIntervall = 6f;
	public float wetnessAccumulationSpeed = 0.05f;
	public float snowAccumulationSpeed = 0.05f;
	public float cloudChangeSpeed = 0.001f;
	public float weatherEffectChangeSpeed = 0.001f;

	[Header("Current Weather")]
	public EnviroWeatherPrefab currentActiveWeatherID;
	private EnviroWeatherPrefab lastActiveWeatherID;




	[HideInInspector]
	public float wetness;
	private float curWetness;
	[HideInInspector]
 	public float SnowStrenght;
	private float curSnowStrenght;
	
	private List<EnviroWeatherPrefab> curPossibleWeatherTypes = new List<EnviroWeatherPrefab>();
	private SeasonVariables.Seasons curSeason;
	//private float lastUpdate;
	private float nextUpdate;
	private int thundersfx;
	private AudioSource currentAudioSource;


 

	//Use this for initialization
	void Start () 
	{
        CreateWeatherTypeList();
        currentActiveWeatherID = WeatherTemplates[0];
		lastActiveWeatherID = WeatherTemplates[0];
		currentAudioSource = EnviroMgr.instance.AudioSourceWeather;    
    }
	
	void OnEnable()
	{
		if (WeatherEffectsPrefabs[0] == null)
		{
			Debug.LogError("Please add a WeatherPrefab in EnviroWeather component!");
			this.enabled = false;
		}
	}

    void CreateWeatherTypeList()
    {
		GameObject VFX = new GameObject ();
		VFX.name = "VFX";
		VFX.transform.parent = EnviroMgr.instance.EffectsHolder.transform;
		VFX.transform.localPosition = Vector3.zero;

        for (int i = 0; i < WeatherEffectsPrefabs.Count; i++)
        {
            GameObject templ = Instantiate(WeatherEffectsPrefabs[i]);
			templ.transform.parent = VFX.transform;
            templ.transform.localPosition = Vector3.zero;
            templ.transform.localRotation = Quaternion.identity;
            WeatherTemplates.Add(templ.GetComponent<EnviroWeatherPrefab>());
        }

        for (int i = 0; i < WeatherTemplates.Count; i++)
        {
            for (int i2 = 0; i2 < WeatherTemplates[i].effectParticleSystems.Count; i2++)
            {
				WeatherTemplates[i].effectEmmisionRates.Add(GetEmissionRate(WeatherTemplates[i].effectParticleSystems[i2]));
				SetEmissionRate(WeatherTemplates[i].effectParticleSystems[i2],0f);
            }   
        }
    }
	
	void UpdateAudioSource (EnviroWeatherPrefab i)
	{
		if (i.Sfx != null)
		{
			if (i.Sfx == currentAudioSource.clip)
			{
				currentAudioSource.GetComponent<EnviroAudioSource>().FadeIn(i.Sfx);
				return;
			}

			if (currentAudioSource == EnviroMgr.instance.AudioSourceWeather)
			{
				EnviroMgr.instance.AudioSourceWeather.GetComponent<EnviroAudioSource>().FadeOut();
				EnviroMgr.instance.AudioSourceWeather2.GetComponent<EnviroAudioSource>().FadeIn(i.Sfx);
				currentAudioSource = EnviroMgr.instance.AudioSourceWeather2;
			}
			else if (currentAudioSource == EnviroMgr.instance.AudioSourceWeather2)
			{
				EnviroMgr.instance.AudioSourceWeather2.GetComponent<EnviroAudioSource>().FadeOut();
				EnviroMgr.instance.AudioSourceWeather.GetComponent<EnviroAudioSource>().FadeIn(i.Sfx);
				currentAudioSource = EnviroMgr.instance.AudioSourceWeather;
			}
		} 
		else
		{
			EnviroMgr.instance.AudioSourceWeather.GetComponent<EnviroAudioSource>().FadeOut();
			EnviroMgr.instance.AudioSourceWeather2.GetComponent<EnviroAudioSource>().FadeOut();
		}
	}

	void UpdateClouds (EnviroWeatherPrefab i)
	{
		EnviroMgr.instance.Envirosky.Lighting.SunWeatherMod = i.sunLightMod;
		EnviroMgr.instance.Envirosky.Clouds.FirstColor = Color.Lerp(EnviroMgr.instance.Envirosky.Clouds.FirstColor,i.cloudConfig.FirstColor,cloudChangeSpeed);
		EnviroMgr.instance.Envirosky.Clouds.SecondColor = Color.Lerp(EnviroMgr.instance.Envirosky.Clouds.SecondColor,i.cloudConfig.SecondColor,cloudChangeSpeed);
		EnviroMgr.instance.Envirosky.Clouds.DirectLightIntensity = Mathf.Lerp(EnviroMgr.instance.Envirosky.Clouds.DirectLightIntensity,i.cloudConfig.DirectLightInfluence,cloudChangeSpeed);
		EnviroMgr.instance.Envirosky.Clouds.AmbientLightIntensity = Mathf.Lerp(EnviroMgr.instance.Envirosky.Clouds.AmbientLightIntensity,i.cloudConfig.AmbientLightInfluence,cloudChangeSpeed);
		EnviroMgr.instance.Envirosky.Clouds.Coverage = Mathf.Lerp(EnviroMgr.instance.Envirosky.Clouds.Coverage,i.cloudConfig.Coverage,cloudChangeSpeed);
		EnviroMgr.instance.Envirosky.Clouds.Density = Mathf.Lerp(EnviroMgr.instance.Envirosky.Clouds.Density,i.cloudConfig.Density,cloudChangeSpeed);
		EnviroMgr.instance.Envirosky.Clouds.Alpha = Mathf.Lerp(EnviroMgr.instance.Envirosky.Clouds.Alpha,i.cloudConfig.Alpha,cloudChangeSpeed);
		EnviroMgr.instance.Envirosky.Clouds.Speed1 = Mathf.Lerp(EnviroMgr.instance.Envirosky.Clouds.Speed1,i.cloudConfig.Speed1,(0.00025f) * Time.deltaTime);
		EnviroMgr.instance.Envirosky.Clouds.Speed2 = Mathf.Lerp(EnviroMgr.instance.Envirosky.Clouds.Speed2,i.cloudConfig.Speed2,(0.00025f) * Time.deltaTime);
		EnviroMgr.instance.Envirosky.Sky.weatherMod = Color.Lerp(EnviroMgr.instance.Envirosky.Sky.weatherMod,i.WeatherColorMod,cloudChangeSpeed);
	}


	void UpdateFog (EnviroWeatherPrefab i)
	{
		if (EnviroMgr.instance.Envirosky.Fog.Fogmode == FogMode.Linear) {
			RenderSettings.fogEndDistance = Mathf.Lerp (RenderSettings.fogEndDistance, i.fogDistance, 0.01f);
			RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance,i.fogStartDistance,0.01f);
		}
		else
			RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity,i.fogDensity,0.01f);

		if (EnviroMgr.instance.Envirosky.Fog.AdvancedFog) 
		{
			EnviroMgr.instance.Envirosky.Fog.skyFogHeight = Mathf.Lerp(EnviroMgr.instance.Envirosky.Fog.skyFogHeight,i.SkyFogHeight,cloudChangeSpeed);
			EnviroMgr.instance.Envirosky.Fog.skyFogStrength = Mathf.Lerp(EnviroMgr.instance.Envirosky.Fog.skyFogStrength,i.SkyFogIntensity,cloudChangeSpeed);
			EnviroMgr.instance.Envirosky.Fog.scatteringStrenght = Mathf.Lerp(EnviroMgr.instance.Envirosky.Fog.scatteringStrenght,i.FogScatteringIntensity,cloudChangeSpeed);
		}
	}

	void UpdateEffectSystems (EnviroWeatherPrefab id)
	{
		for (int i = 0; i < id.effectParticleSystems.Count; i++) 
		{
				// Set EmissionRate
			    float val = Mathf.Lerp(GetEmissionRate(id.effectParticleSystems[i]), id.effectEmmisionRates[i] * EnviroMgr.instance.Quality.GlobalParticleEmissionRates, weatherEffectChangeSpeed);
				SetEmissionRate (id.effectParticleSystems [i], val);
		}

		for (int i = 0; i < lastActiveWeatherID.effectParticleSystems.Count; i++) 
		{
			     // Set EmissionRates
			    float val2 = Mathf.Lerp(GetEmissionRate(lastActiveWeatherID.effectParticleSystems[i]),0f,weatherEffectChangeSpeed);
				SetEmissionRate (lastActiveWeatherID.effectParticleSystems [i], val2);
		}

		windZone.windMain = id.WindStrenght; // Set Wind Strenght

		curWetness = wetness;
		wetness = Mathf.Lerp (curWetness, id.wetnessLevel, wetnessAccumulationSpeed * Time.deltaTime);
		wetness = Mathf.Clamp(wetness,0f,1f);

		curSnowStrenght = SnowStrenght;
		SnowStrenght = Mathf.Lerp (curSnowStrenght, id.snowLevel, snowAccumulationSpeed * Time.deltaTime);
		SnowStrenght = Mathf.Clamp(SnowStrenght,0f,1f);

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

	void BuildNewWeatherList ()
	{
		curPossibleWeatherTypes = new List<EnviroWeatherPrefab> ();
		for (int i = 0; i < WeatherTemplates.Count; i++) 
		{
			switch (EnviroMgr.instance.seasons.currentSeasons)
			{
			case SeasonVariables.Seasons.Spring:
			if (WeatherTemplates[i].Spring)
					curPossibleWeatherTypes.Add(WeatherTemplates[i]);
			break;
			case SeasonVariables.Seasons.Summer:
				if (WeatherTemplates[i].Summer)
					curPossibleWeatherTypes.Add(WeatherTemplates[i]);
				break;
			case SeasonVariables.Seasons.Autumn:
				if (WeatherTemplates[i].Autumn)
					curPossibleWeatherTypes.Add(WeatherTemplates[i]);
				break;
			case SeasonVariables.Seasons.Winter:
				if (WeatherTemplates[i].winter)
					curPossibleWeatherTypes.Add(WeatherTemplates[i]);
				break;
			}
		} 
	}

	IEnumerator PlayThunderRandom()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(10,20));
		int i = UnityEngine.Random.Range(0,ThunderSFX.Count);
		EnviroMgr.instance.AudioSourceThunder.clip = ThunderSFX[i];
		EnviroMgr.instance.AudioSourceThunder.loop = false;
		EnviroMgr.instance.AudioSourceThunder.Play ();
		LightningGenerator.Lightning ();
		thundersfx = 0;
	}

	void WeatherUpdate ()
	{
		//lastUpdate = EnviroMgr.instance.currentTimeInHours;
		nextUpdate = EnviroMgr.instance.currentTimeInHours + WeatherUpdateIntervall;
		BuildNewWeatherList ();
		lastActiveWeatherID = currentActiveWeatherID;
		currentActiveWeatherID = PossibiltyCheck ();
		if( currentActiveWeatherID != lastActiveWeatherID)
			UpdateWeather(currentActiveWeatherID);
	}

	public void SetWeatherOverwrite (int weatherId)
	{
		if (WeatherTemplates[weatherId] != currentActiveWeatherID)
		{
		lastActiveWeatherID = currentActiveWeatherID;
		currentActiveWeatherID = WeatherTemplates[weatherId];
		UpdateWeather(currentActiveWeatherID);
		}
	}

    // returns the winnerID
    EnviroWeatherPrefab PossibiltyCheck ()
	{

		List<EnviroWeatherPrefab> over = new List<EnviroWeatherPrefab> ();

		for (int i = 0 ; i < curPossibleWeatherTypes.Count;i++)
		{
			int würfel = UnityEngine.Random.Range (0,100);

			if (curSeason == SeasonVariables.Seasons.Spring)
			{
				if (würfel <= curPossibleWeatherTypes[i].possibiltyInSpring)
					over.Add(curPossibleWeatherTypes[i]);
			}
			if (curSeason == SeasonVariables.Seasons.Summer)
			{
				if (würfel <= curPossibleWeatherTypes[i].possibiltyInSummer)
					over.Add(curPossibleWeatherTypes[i]);
			}
			if (curSeason == SeasonVariables.Seasons.Autumn)
			{
				if (würfel <= curPossibleWeatherTypes[i].possibiltyInAutumn)
					over.Add(curPossibleWeatherTypes[i]);
			}
			if (curSeason == SeasonVariables.Seasons.Winter)
			{
				if (würfel <= curPossibleWeatherTypes[i].possibiltyInWinter)
					over.Add(curPossibleWeatherTypes[i]);
			}

		} 

		if (over.Count > 0) // We found new Weather Change if not keep ours
			return over [0];
		else
			return currentActiveWeatherID;

		}

	void LateUpdate ()
	{	
		if (EnviroMgr.instance.currentTimeInHours >= nextUpdate)
			WeatherUpdate ();

		if (EnviroMgr.instance.seasons.currentSeasons != curSeason)
			curSeason = EnviroMgr.instance.seasons.currentSeasons;

		UpdateClouds (currentActiveWeatherID);
		UpdateFog (currentActiveWeatherID);
	
		//Play ThunderSFX
		if ( thundersfx == 0 && currentActiveWeatherID.isLightningStorm)
		{
			thundersfx = 1;
			StartCoroutine(PlayThunderRandom());
		}

		UpdateEffectSystems (currentActiveWeatherID);
	}
	
	void UpdateWeather (EnviroWeatherPrefab ID) 
	{
		EnviroMgr.instance.NotifyWeatherChanged (ID);
		UpdateAudioSource (ID);
	}
}
