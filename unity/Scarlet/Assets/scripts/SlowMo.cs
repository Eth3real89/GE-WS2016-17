using UnityEngine;
using System.Collections;

public class SlowMo : MonoBehaviour {

    public float slowDuration;
    public float slowAmount;

    float currentSlowMo = 0f;

	// Update is called once per frame
	void Update () {
	    if(Input.GetButtonDown("Fire1"))
        {

            if(Time.timeScale == 1.0f)
                Time.timeScale = slowAmount;
            else
                Time.timeScale = 1.0f;

            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        if(Time.timeScale == slowAmount)
        {
            currentSlowMo += Time.deltaTime;

        }

        if(currentSlowMo >= slowDuration)
        {
            currentSlowMo = 0f;

            Time.timeScale = 1.0f;
        }
	}
}
