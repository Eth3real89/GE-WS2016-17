using UnityEngine;
using System.Collections;

public class EnviroReflections : MonoBehaviour {

	public ReflectionProbe probe;
	public float ReflectionUpdateInGameHours = 1f;

	private float lastUpdate;

	// Use this for initialization
	void Start () 
	{

	if (probe == null)
			probe = GetComponent<ReflectionProbe> ();

	}

	void  UpdateProbe ()
	{
		probe.RenderProbe ();
		lastUpdate = EnviroMgr.instance.currentTimeInHours;
	}

	// Update is called once per frame
	void Update ()
	{
		if (EnviroMgr.instance.currentTimeInHours > lastUpdate + ReflectionUpdateInGameHours)
			UpdateProbe ();

	}
}
