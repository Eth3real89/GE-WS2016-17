using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableEffect : MonoBehaviour {

    public abstract void Play(Vector3 position = new Vector3());

    public abstract void Hide();
	
}
