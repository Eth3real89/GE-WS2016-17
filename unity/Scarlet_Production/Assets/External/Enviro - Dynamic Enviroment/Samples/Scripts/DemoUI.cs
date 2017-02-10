using UnityEngine;
using System.Collections;

public class DemoUI : MonoBehaviour {


	public UnityEngine.UI.Slider sliderTime;
	public UnityEngine.UI.Slider sliderQuality;
	public UnityEngine.UI.Text timeText;
	public UnityEngine.UI.Text weatherText;

 	bool seasonmode = true;
	bool fastdays = false;

	// Use this for initialization
	void Start () 
	{
	
	}


	public void ChangeTimeSlider () 
	{
		if (sliderTime.value < 0.01f)
			sliderTime.value = 0.01f;
		EnviroMgr.instance.Envirosky.GameTime.Hours = sliderTime.value * 24f;
	}

	public void ChangeQualitySlider () 
	{
		EnviroMgr.instance.Quality.GlobalParticleEmissionRates = sliderQuality.value;
	}

	public void SetWeatherID (int id) 
	{
		EnviroMgr.instance.EnviroWeather.SetWeatherOverwrite (id);
	}

	public void OverwriteSeason ()
	{
		if (!seasonmode) {
			seasonmode = true;
			EnviroMgr.instance.seasons.calcSeasons = true;
		}
		else {
			seasonmode = false;
			EnviroMgr.instance.seasons.calcSeasons = false;
		}
		
	}

	public void FastDays ()
	{
		if (!fastdays) {
			fastdays = true;
			EnviroMgr.instance.Envirosky.GameTime.DayLengthInMinutes = 0.2f;
		}
		else {
			fastdays = false;
			EnviroMgr.instance.Envirosky.GameTime.DayLengthInMinutes = 5f;
		}

	}

	public void SetSeasonSpring ()
	{
		EnviroMgr.instance.ChangeSeason (SeasonVariables.Seasons.Spring);

	}

	public void SetSeasonSummer ()
	{
		EnviroMgr.instance.ChangeSeason (SeasonVariables.Seasons.Summer);
		
	}

	public void SetSeasonAutumn ()
	{
		EnviroMgr.instance.ChangeSeason (SeasonVariables.Seasons.Autumn);
		
	}

	public void SetSeasonWinter ()
	{
		EnviroMgr.instance.ChangeSeason (SeasonVariables.Seasons.Winter);
		
	}

	public void ToggleShadows (bool i)
	{
		EnviroMgr.instance.Quality.CloudsShadowCast = i;
	}
		
	public void SetFog (bool i)
	{
		EnviroMgr.instance.Envirosky.Fog.AdvancedFog = i;
	}

	public void SetClouds (int i)
	{
		if (i == 0) 
			EnviroMgr.instance.Quality.CloudsQuality = EnviroQualitySettings.CloudQuality.None;
		if (i == 1) 
			EnviroMgr.instance.Quality.CloudsQuality = EnviroQualitySettings.CloudQuality.OneLayer;
		if (i == 2) 
			EnviroMgr.instance.Quality.CloudsQuality = EnviroQualitySettings.CloudQuality.TwoLayer;
	}


	// Update is called once per frame
	void Update ()
	{
		int hours = (int)EnviroMgr.instance.Envirosky.GameTime.Hours;
		timeText.text = hours.ToString() + ":00";
		weatherText.text = EnviroMgr.instance.EnviroWeather.currentActiveWeatherID.Name;
		ChangeQualitySlider ();
	}

	void LateUpdate ()
	{
	
		sliderTime.value = EnviroMgr.instance.Envirosky.GameTime.Hours / 24f;
	}
}
