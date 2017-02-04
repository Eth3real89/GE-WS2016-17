using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBullet : Bullet {
	
	// Update is called once per frame
	new void Update () {
        base.Update();

        transform.position = Vector3.Lerp(transform.position,
            transform.position
                + new Vector3(m_Velocity, 0, 0) * Mathf.Sin(Mathf.Deg2Rad * m_Angle)
                + new Vector3(0, 0, m_Velocity) * Mathf.Cos(Mathf.Deg2Rad * m_Angle),
            Time.deltaTime * m_Velocity);
    }

}
