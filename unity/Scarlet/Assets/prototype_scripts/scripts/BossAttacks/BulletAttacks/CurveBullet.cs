using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveBullet : Bullet {

    public bool m_Clockwise = true;

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        transform.position = Vector3.Slerp(transform.position,
            transform.position + transform.forward,
            Time.deltaTime * m_Velocity);

        m_Velocity += Time.deltaTime * 2;

        float angleChange = 0f;
        if (m_Clockwise)
            angleChange = Time.deltaTime * 40;
        else
            angleChange = Time.deltaTime * -40;

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + angleChange, 0);
    }
}
