using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {

    private static GameController instance;

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
	
	// Update is called once per frame
	void Update () {
        float healthPercentage = Mathf.Max(m_ScarletHealth, 0) / m_ScarletStartHealth;

        m_HealthBarSlider.value = healthPercentage * 100f;
        Image img = m_HealthBarSlider.transform.FindChild("Fill Area").GetChild(0).GetComponent<Image>();
        img.color = Color.Lerp(Color.red, Color.green, healthPercentage);
    }
}
