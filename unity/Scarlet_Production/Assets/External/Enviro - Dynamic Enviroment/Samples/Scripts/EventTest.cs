using UnityEngine;
using System.Collections;

public class EventTest : MonoBehaviour {

	void Start ()
	{
		EnviroMgr.instance.OnWeatherChanged += (EnviroWeatherPrefab type) =>
		{
			Debug.Log("Weather changed to: " + type.Name);
		};

		EnviroMgr.instance.OnSeasonChanged += (SeasonVariables.Seasons season) =>
		{
			Debug.Log("Season changed");
		};

		EnviroMgr.instance.OnHourPassed += () =>
		{
			Debug.Log("Hour Passed!");
		};

		EnviroMgr.instance.OnDayPassed += () =>
		{
			Debug.Log("New Day!");
		};
		EnviroMgr.instance.OnYearPassed += () =>
		{
			Debug.Log("New Year!");
		};

	}

	public void TestEventsWWeather ()
	{
		print("Weather Changed though interface!");
	}

	public void TestEventsNight ()
	{
		print("Night now!!");
	}

	public void TestEventsDay ()
	{
		print("Day now!!");
	}
}
