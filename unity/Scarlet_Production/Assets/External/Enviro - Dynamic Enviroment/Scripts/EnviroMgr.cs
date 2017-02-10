/////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////        EnviroMgr - Manage all Enviro Instances and Seasons.      	             	 ////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




[Serializable]
public class EnviroPositioning
{
	public bool FixedHeight = false;
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
	public CloudQuality CloudsQuality;
	public bool CloudsShadowCast = true;

	[Range(0,1)]
	public float GlobalParticleEmissionRates = 1f;
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

    public bool calcSeasons; // if unticked you can manually overwrite current seas. Ticked = automaticly updates seasons
    public Seasons currentSeasons;
	[HideInInspector]
	public Seasons lastSeason;
	public float SpringInDays = 90f;
	public float SummerInDays = 93f;
	public float AutumnInDays = 92f;
	public float WinterInDays = 90f;
	
}

[Serializable]
public class AudioVariables // AudioSetup variables
{
	public GameObject SFXHolderPrefab;
	public AudioClip SpringDayAmbient;
	public AudioClip SpringNightAmbient;
	public AudioClip SummerDayAmbient;
	public AudioClip SummerNightAmbient;
	public AudioClip AutumnDayAmbient;
	public AudioClip AutumnNightAmbient;
	public AudioClip WinterDayAmbient;
	public AudioClip WinterNightAmbient;
}


[AddComponentMenu("Enviro/Enviro Manager")]
public class EnviroMgr : MonoBehaviour {

	private static EnviroMgr _instance; // Creat a static instance for easy access!

	public static EnviroMgr instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<EnviroMgr>();
			return _instance;
		}
	}

	public GameObject Player;
	public Camera PlayerCamera;
   
    [HideInInspector]
	public EnviroSky Envirosky; // Use to access it from other scripts
    [HideInInspector]
    public EnviroWeather EnviroWeather; // Use to access it from other scripts


	public EnviroPositioning AdvancedPositioning = null;
	public AudioVariables Audio = null;
    public EnviroQualitySettings Quality = null;
	public SeasonVariables seasons = null;

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

	public delegate void HourPassed();
	public delegate void DayPassed();
	public delegate void YearPassed();
	public delegate void WeatherChanged(EnviroWeatherPrefab weatherType);
	public delegate void SeasonChanged(SeasonVariables.Seasons season);
	public delegate void isNight();
	public delegate void isDay();

	public event HourPassed OnHourPassed;
	public event DayPassed OnDayPassed;
	public event YearPassed OnYearPassed;
	public event WeatherChanged OnWeatherChanged;
	public event SeasonChanged OnSeasonChanged;
	public event isNight OnNightTime;
	public event isDay OnDayTime;

	void Awake ()
	{
        if (Envirosky == null)
            Envirosky = GetComponent<EnviroSky>();
        if (EnviroWeather == null)
            EnviroWeather = GetComponent<EnviroWeather>();

        CreateEffects ();

		Envirosky.enabled = false;
		Envirosky.enabled = true;
	}

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


	void Start()
	{
		currentTimeInHours = GetInHours (Envirosky.GameTime.Hours, Envirosky.GameTime.Days, Envirosky.GameTime.Years);
		InvokeRepeating("UpdateEnviroment", 0, Quality.UpdateInterval);// Starts a repeating Method with custom interval
	}

	void CreateEffects ()
	{
		EffectsHolder = new GameObject ();
		EffectsHolder.name = "Effect Holder";
		EffectsHolder.transform.parent = Player.transform;
        EffectsHolder.transform.localPosition = Vector3.zero;

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


	// Check for correct Setup
	void OnEnable ()
	{
		if(Envirosky == null)
		{
			Debug.LogError("Please assign the EnviroSky component in Inspector!");
			this.enabled = false;
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
		seasons.currentSeasons = season;
		NotifySeasonChanged (season);
	}
		
	// Update the Season according gameDays
	void UpdateSeason ()
	{
		if (currentDay >= 0 && currentDay < seasons.SpringInDays)
		{
			seasons.currentSeasons = SeasonVariables.Seasons.Spring;

			if (seasons.lastSeason != seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Spring);
			
			seasons.lastSeason = seasons.currentSeasons;
		} 
		else if (currentDay >= seasons.SpringInDays && currentDay < (seasons.SpringInDays + seasons.SummerInDays))
		{
			seasons.currentSeasons = SeasonVariables.Seasons.Summer;

			if (seasons.lastSeason != seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Summer);

			seasons.lastSeason = seasons.currentSeasons;
		} 
		else if (currentDay >= (seasons.SpringInDays + seasons.SummerInDays) && currentDay < (seasons.SpringInDays + seasons.SummerInDays + seasons.AutumnInDays)) 
		{
			seasons.currentSeasons = SeasonVariables.Seasons.Autumn;

			if (seasons.lastSeason != seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Autumn);

			seasons.lastSeason = seasons.currentSeasons;
		}
		else if(currentDay >= (seasons.SpringInDays + seasons.SummerInDays + seasons.AutumnInDays) && currentDay <= (seasons.SpringInDays + seasons.SummerInDays + seasons.AutumnInDays + seasons.WinterInDays))
		{
			seasons.currentSeasons = SeasonVariables.Seasons.Winter;

			if (seasons.lastSeason != seasons.currentSeasons)
				NotifySeasonChanged (SeasonVariables.Seasons.Winter);
			
			seasons.lastSeason = seasons.currentSeasons;
		}
	}

	public float GetInHours (float hours,float days, float years)
	{
		float inHours  = hours + (days*24f) + ((years * (seasons.SpringInDays + seasons.SummerInDays + seasons.AutumnInDays + seasons.WinterInDays)) * 24f);
		return inHours;
		
	}


	void UpdateEnviroment () // Update the all GrowthInstances
	{
		if (Envirosky != null)
		{
			currentHour = Envirosky.GameTime.Hours;
			currentDay = Envirosky.GameTime.Days;
			currentYear = Envirosky.GameTime.Years;
			currentTimeInHours = GetInHours (currentHour, currentDay, currentYear);
		}
		// Set correct Season.
		if(seasons.calcSeasons)
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
}
