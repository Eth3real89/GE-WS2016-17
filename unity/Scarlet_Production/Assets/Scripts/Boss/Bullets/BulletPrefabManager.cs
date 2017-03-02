using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefabManager : MonoBehaviour {

    private static BulletPrefabManager _Instance;

    public static BulletPrefabManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    public BulletBehaviour m_NonBlockable;
    public BulletBehaviour m_Blockable;
    public BulletBehaviour m_Deflectable;
    public BulletBehaviour m_Grenade;

    private void Start()
    {
        if (_Instance == null)
            _Instance = this;
    }
	
}
