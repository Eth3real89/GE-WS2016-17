using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AEPrefabManager : MonoBehaviour {

    public enum BeamType
    {
        Rotating, Swaying, FromGround, Wonky
    }

    private static AEPrefabManager _Instance;

    public static AEPrefabManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    public GameObject m_BlastWaveWrapper;

    public GameObject m_BasicBeamVisuals;

    public GameObject m_ExpandingBeamDamagePrefab;
    public GameObject m_WonkyBeamDamagePrefab;
    public GameObject m_SwayingBeamDamagePrefab;
    public GameObject m_FromGroundBeamDamagePrefab;

    public GameObject m_SwayingBeamWrapper {
        get
        {
            return BuildBeamPrefab(BeamType.Swaying);
        }
    }

    public GameObject m_RotatingBeamWrapper
    {
        get
        {
            return BuildBeamPrefab(BeamType.Rotating);
        }
    }

    public GameObject m_FromGroundBeamWrapper
    {
        get
        {
            return BuildBeamPrefab(BeamType.FromGround);
        }
    }

    public GameObject m_WonkyBeamWrapper
    {
        get
        {
            return BuildBeamPrefab(BeamType.Wonky);
        }
    }

    public GameObject m_ConeSeeThroughVisuals;

    private void Start()
    {
        if (_Instance == null)
            _Instance = this;
    }

    private GameObject BuildBeamPrefab(BeamType type)
    {
        GameObject beam = Instantiate(m_BasicBeamVisuals);

        if (type == BeamType.FromGround)
        {
            GameObject fgBeam = Instantiate(m_FromGroundBeamDamagePrefab);
            beam.AddComponent(fgBeam.GetComponent<GroundBeamAEDamage>().GetType());
            CopyFields(fgBeam.GetComponent<GroundBeamAEDamage>(), beam.GetComponent<GroundBeamAEDamage>());
            beam.GetComponent<GroundBeamAEDamage>().m_VolumetricBehavior = beam.GetComponentInChildren<VolumetricLines.VolumetricLineStripBehavior>();
            Destroy(fgBeam);
        }
        else if (type == BeamType.Rotating)
        {
            GameObject eBeam = Instantiate(m_ExpandingBeamDamagePrefab);
            beam.AddComponent(eBeam.GetComponent<BeamAEDamage>().GetType());
            CopyFields(eBeam.GetComponent<BeamAEDamage>(), beam.GetComponent<BeamAEDamage>());
            beam.GetComponent<BeamAEDamage>().m_VolumetricBehavior = beam.GetComponentInChildren<VolumetricLines.VolumetricLineStripBehavior>();
            Destroy(eBeam);
        }
        else if (type == BeamType.Swaying)
        {
            GameObject sBeam = Instantiate(m_SwayingBeamDamagePrefab);
            beam.AddComponent(sBeam.GetComponent<SwayingAEDamage>().GetType());
            CopyFields(sBeam.GetComponent<SwayingAEDamage>(), beam.GetComponent<SwayingAEDamage>());
            beam.GetComponent<SwayingAEDamage>().m_VolumetricBehavior = beam.GetComponentInChildren<VolumetricLines.VolumetricLineStripBehavior>();
            Destroy(sBeam);
        }
        else if (type == BeamType.Wonky)
        {
            GameObject wBeam = Instantiate(m_WonkyBeamDamagePrefab);
            beam.AddComponent(wBeam.GetComponent<BeamAEDamageWonkyMovement>().GetType());
            CopyFields(wBeam.GetComponent<BeamAEDamageWonkyMovement>(), beam.GetComponent<BeamAEDamageWonkyMovement>());
            beam.GetComponent<BeamAEDamageWonkyMovement>().m_VolumetricBehavior = beam.GetComponentInChildren<VolumetricLines.VolumetricLineStripBehavior>();
            Destroy(wBeam);
        }

        return beam;
    }

    public void CopyFields(MonoBehaviour source, MonoBehaviour target)
    {
        FieldInfo[] sourceFields = source.GetType().GetFields(BindingFlags.Public |
                                                              BindingFlags.NonPublic |
                                                              BindingFlags.Instance);

        int i = 0;

        for (i = 0; i < sourceFields.Length; i++)
        {
            var value = sourceFields[i].GetValue(source);
            sourceFields[i].SetValue(target, value);
        }
    }
}
