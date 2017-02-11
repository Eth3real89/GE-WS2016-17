using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Enviro/Weather Zone")]
public class EnviroZone : MonoBehaviour {

	[Tooltip("Defines the zone name.")]
	public string zoneName;

	[HideInInspector]
	public List<EnviroWeatherPrefab> zoneWeather = new List<EnviroWeatherPrefab>();
	[HideInInspector]
	public List<EnviroWeatherPrefab> curPossibleZoneWeather;

	[Header("Zone weather settings:")]
	[Tooltip("Add alll weather prefabs for this zone here.")]
	public List<GameObject> zoneWeatherPrefabs = new List<GameObject>();
	[Tooltip("Defines how often (gametime hours) the system will heck to change the current weather conditions.")]
	public float WeatherUpdateIntervall = 6f;
	[Header("Zone scaling and gizmo:")]
	[Tooltip("Defines the zone scale.")]
	public Vector3 zoneScale = new Vector3 (100f, 100f, 100f);
	[Tooltip("Defines the color of the zone's gizmo in editor mode.")]
	public Color zoneGizmoColor = Color.gray;

	[Header("Current active weather:")]
	[Tooltip("The current active weather conditions.")]
	public EnviroWeatherPrefab currentActiveZoneWeatherID;
	[HideInInspector]
	public EnviroWeatherPrefab lastActiveZoneWeatherID;

	private BoxCollider zoneCollider;
	private float nextUpdate;
	private bool init = false;


	void Start () 
	{
		zoneCollider = gameObject.AddComponent<BoxCollider> ();
		zoneCollider.isTrigger = true;

		UpdateZoneScale ();

		if(!GetComponent<EnviroSky>())
			EnviroSky.instance.RegisterZone (this);

	}

	public void UpdateZoneScale ()
	{
			zoneCollider.size = zoneScale;
	}

	public void CreateZoneWeatherTypeList ()
	{
		// Add new WeatherPrefabs
		for ( int i = 0; i < zoneWeatherPrefabs.Count; i++)
		{
			bool addThis = true;
			for (int i2 = 0; i2 < EnviroSky.instance.Weather.WeatherPrefabs.Count; i2++)
			{
				if (zoneWeatherPrefabs [i] == EnviroSky.instance.Weather.WeatherPrefabs [i2]) 
				{
					addThis = false;
					zoneWeather.Add (EnviroSky.instance.Weather.WeatherTemplates [i2]);
				}
			}
			if (addThis) {
				GameObject templ = Instantiate (zoneWeatherPrefabs [i]);
				templ.transform.parent = EnviroSky.instance.Weather.VFXHolder.transform;
				templ.transform.localPosition = Vector3.zero;
				templ.transform.localRotation = Quaternion.identity;
				zoneWeather.Add(templ.GetComponent<EnviroWeatherPrefab>());
				EnviroSky.instance.Weather.WeatherPrefabs.Add (zoneWeatherPrefabs [i]);
				EnviroSky.instance.Weather.WeatherTemplates.Add (templ.GetComponent<EnviroWeatherPrefab> ());
			}
		}
		// Setup Particle Systems
		for (int i = 0; i < zoneWeather.Count; i++)
		{
			for (int i2 = 0; i2 < zoneWeather[i].effectParticleSystems.Count; i2++)
			{
				zoneWeather[i].effectEmmisionRates.Add(EnviroSky.GetEmissionRate(zoneWeather[i].effectParticleSystems[i2]));
				EnviroSky.SetEmissionRate(zoneWeather[i].effectParticleSystems[i2],0f);
			}   
		}
			
		currentActiveZoneWeatherID = zoneWeather [0];
		lastActiveZoneWeatherID = zoneWeather [0];
	}
		
	void BuildNewWeatherList ()
	{
		curPossibleZoneWeather = new List<EnviroWeatherPrefab> ();
		for (int i = 0; i < zoneWeather.Count; i++) 
		{
			switch (EnviroSky.instance.Seasons.currentSeasons)
			{
			case SeasonVariables.Seasons.Spring:
				if (zoneWeather[i].Spring)
					curPossibleZoneWeather.Add(zoneWeather[i]);
				break;
			case SeasonVariables.Seasons.Summer:
				if (zoneWeather[i].Summer)
					curPossibleZoneWeather.Add(zoneWeather[i]);
				break;
			case SeasonVariables.Seasons.Autumn:
				if (zoneWeather[i].Autumn)
					curPossibleZoneWeather.Add(zoneWeather[i]);
				break;
			case SeasonVariables.Seasons.Winter:
				if (zoneWeather[i].winter)
					curPossibleZoneWeather.Add(zoneWeather[i]);
				break;
			}
		} 
	}

	EnviroWeatherPrefab PossibiltyCheck ()
	{
		List<EnviroWeatherPrefab> over = new List<EnviroWeatherPrefab> ();

		for (int i = 0 ; i < curPossibleZoneWeather.Count;i++)
		{
			int würfel = UnityEngine.Random.Range (0,100);

			if (EnviroSky.instance.Seasons.currentSeasons == SeasonVariables.Seasons.Spring)
			{
				if (würfel <= curPossibleZoneWeather[i].possibiltyInSpring)
					over.Add(curPossibleZoneWeather[i]);
			}
			if (EnviroSky.instance.Seasons.currentSeasons == SeasonVariables.Seasons.Summer)
			{
				if (würfel <= curPossibleZoneWeather[i].possibiltyInSummer)
					over.Add(curPossibleZoneWeather[i]);
			}
			if (EnviroSky.instance.Seasons.currentSeasons == SeasonVariables.Seasons.Autumn)
			{
				if (würfel <= curPossibleZoneWeather[i].possibiltyInAutumn)
					over.Add(curPossibleZoneWeather[i]);
			}
			if (EnviroSky.instance.Seasons.currentSeasons == SeasonVariables.Seasons.Winter)
			{
				if (würfel <= curPossibleZoneWeather[i].possibiltyInWinter)
					over.Add(curPossibleZoneWeather[i]);
			}
		} 

		if (over.Count > 0)
		{		
			EnviroSky.instance.NotifyZoneWeatherChanged (over [0], this);
			return over [0];
		}
		else
			return currentActiveZoneWeatherID;
	}
		
	void WeatherUpdate ()
	{
		nextUpdate = EnviroSky.instance.currentTimeInHours + WeatherUpdateIntervall;
		BuildNewWeatherList ();

		lastActiveZoneWeatherID = currentActiveZoneWeatherID;
		currentActiveZoneWeatherID = PossibiltyCheck ();
	}
		

	void LateUpdate () 
	{
		if (EnviroSky.instance.started && !init) {
			CreateZoneWeatherTypeList ();
			init = true;
		}

		if (EnviroSky.instance.currentTimeInHours >= nextUpdate && EnviroSky.instance.Weather.updateWeather && EnviroSky.instance.started)
			WeatherUpdate ();
	}


	/// Triggers
	void OnTriggerEnter (Collider col)
	{
		if (EnviroSky.instance.Weather.useTag) {
			if (col.gameObject.tag == EnviroSky.instance.gameObject.tag) {
				//print ("Enter: " + name);
				EnviroSky.instance.Weather.currentActiveZone = this;
			}
		} else {
			if (col.gameObject.GetComponent<EnviroSky> ()) {
				//print ("Enter: " + name);
				EnviroSky.instance.Weather.currentActiveZone = this;
			}
		}
	}

	void OnTriggerExit (Collider col)
	{

		if (EnviroSky.instance.Weather.useTag) {
			if (col.gameObject.tag == EnviroSky.instance.gameObject.tag) {
				//print ("Enter: " + name);
				EnviroSky.instance.Weather.currentActiveZone = EnviroSky.instance.Weather.zones[0];
			}
		} else {
			if (col.gameObject.GetComponent<EnviroSky> ()) {
				//print ("Enter: " + name);
				EnviroSky.instance.Weather.currentActiveZone = EnviroSky.instance.Weather.zones[0];
			}
		}
	}


	void OnDrawGizmos () 
	{
		Gizmos.color = zoneGizmoColor;
		Gizmos.DrawCube (transform.position, new Vector3(zoneScale.x,zoneScale.y,zoneScale.z));
	}
}
