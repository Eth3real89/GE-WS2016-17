using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    private static GameController instance;

    private static List<Updateable> updateables = new List<Updateable>();

    private GameController() { instance = this; }

    public GameObject m_Scarlet;

    public float m_ScarletStartHealth = 100f;
    public float m_ScarletHealth = 100f;

    public Slider m_HealthBarSlider;

    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameController();
            }
            return instance;
        }
    }

    public bool IsScarlet(Rigidbody body)
    {
        return m_Scarlet.GetComponent<Rigidbody>() == body;
    }

    public void HitScarlet(float damage)
    {
        m_ScarletHealth -= damage;
    }

	// Use this for initialization
	void Start () {
	
	}

    public void RegisterUpdateable(Updateable updateable)
    {
        lock(updateables)
        {
            updateables.Add(updateable);
        }
    }

    public void UnregisterUpdateable(Updateable updateable)
    {
        lock(updateables)
        {
            updateables.Remove(updateable);
        }
    }
	
	// Update is called once per frame
	void Update () {
        lock (updateables)
        {
            for(int i = 0; i < updateables.Count; i++)
                updateables[i].Update();
        }
        float healthPercentage = Mathf.Max(m_ScarletHealth, 0) / m_ScarletStartHealth;

        m_HealthBarSlider.value = healthPercentage * 100f;
        Image img = m_HealthBarSlider.transform.FindChild("Fill Area").GetChild(0).GetComponent<Image>();
        img.color = Color.Lerp(Color.red, Color.green, healthPercentage);

    }
}
