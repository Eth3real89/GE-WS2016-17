using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    private static GameController instance;

    private static List<Updateable> updateables = new List<Updateable>();

    private GameController() { instance = this; }

    public GameObject m_Boss;
    public GameObject m_Scarlet;

    public float m_ScarletStartHealth = 100f;
    public float m_ScarletHealth = 100f;

    public bool m_ScarletInvincible = false;

    public Text m_BossKillNotification;
    public Text m_DeathNotification;

    public float m_NotificationTime = 5.0f;

    public Slider m_HealthBarSlider;

    private bool isScarletDead = false;
    private bool isBossDead = false;

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

    public bool IsBoss(Rigidbody body)
    {
        return m_Boss.GetComponent<Rigidbody>() == body;
    }

    public void HitScarlet(float damage)
    {
        if (!m_ScarletInvincible)
            m_ScarletHealth -= damage;

        if(m_ScarletHealth <= 0) isScarletDead = true;
    }

    public void HitBoss(float damage)
    {
        m_Boss.GetComponent<BossHealth>().TakeDamage(damage);

        if(m_Boss.GetComponent<BossHealth>().GetBossHealth() <= 0.0f) isBossDead = true;
    }

    public void HealScarlet(float amount) {
        float health = m_ScarletHealth + amount;
     
        m_ScarletHealth = (health > m_ScarletStartHealth) ? m_ScarletStartHealth : health;
    }

	// Use this for initialization
	void Start () {
        m_BossKillNotification.enabled = false;
        m_BossKillNotification.text = "ASCENDED";

        m_DeathNotification.enabled = false;
        m_DeathNotification.text = "EXORCISED";

        isScarletDead = false;
        isBossDead = false;
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
        //Image img = m_HealthBarSlider.transform.FindChild("Fill Area").GetChild(0).GetComponent<Image>();
        //img.color = Color.Lerp(Color.black, Color.red, healthPercentage);
     
        HandleText(isBossDead, m_BossKillNotification);
        HandleText(isScarletDead, m_DeathNotification);
    }

    private void HandleText(bool isDead, Text text) {
      if(isDead)
        {
            m_NotificationTime -= Time.deltaTime;

            if(m_NotificationTime > 0) {
              text.enabled = true;

            } else {
              text.enabled = false;
            }
        }
    }
}
