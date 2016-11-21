using UnityEngine;
using System.Collections;
using System;

public class BeamAttack : AEAttackPart, Updateable {

    public GameObject m_BeamVisuals;

    public GameObject m_BeamInstance;

    public Vector3 m_StartPosition;

    public AEAttackSeries m_Series;

    public float m_GrowthSpeed = 20f;
    public float m_GrowthTime = 1.5f;

    public float m_RotateTime = 8f;
    public float m_RotateSpeed = 1f;

    private float passedTime = 0f;

    public BeamAttack(GameObject beamPrefab, AEAttackSeries series)
    {
        this.m_BeamVisuals = beamPrefab;
        this.m_Series = series;
    }

    public override GameObject GetObject()
    {
        return m_BeamInstance;
    }

    public override void LaunchPart()
    {
        m_BeamInstance = (GameObject) GameObject.Instantiate(m_BeamVisuals, m_StartPosition, new Quaternion(0, 0, 0, 0));
        m_BeamInstance.transform.LookAt(GameController.Instance.m_Scarlet.transform);

        Quaternion rotation = m_BeamInstance.transform.rotation;
        Vector3 asCoords = rotation.eulerAngles;

        asCoords.x = 0;
        asCoords.z = 0;

        m_BeamInstance.transform.rotation = Quaternion.Euler(asCoords);

        m_BeamInstance.transform.localScale = new Vector3(2, 2, 0.02f);
        passedTime = 0;

        GameController.Instance.RegisterUpdateable(this);
    }

    public void Update()
    {
        passedTime += Time.deltaTime;
        if (passedTime < m_GrowthTime)
        {
            m_BeamInstance.transform.localScale = new Vector3(1, 1, passedTime * m_GrowthSpeed);
        }
        else if (passedTime - m_GrowthTime < m_RotateTime)
        {
            m_BeamInstance.transform.rotation = Quaternion.Slerp(m_BeamInstance.transform.rotation,
                Quaternion.Euler(m_BeamInstance.transform.rotation.eulerAngles + new Vector3(0, 179, 0)),
                Time.deltaTime * m_RotateSpeed);
        }
        else
        {
            GameObject.Destroy(m_BeamInstance);
            GameController.Instance.UnregisterUpdateable(this);
        }

    }
}
