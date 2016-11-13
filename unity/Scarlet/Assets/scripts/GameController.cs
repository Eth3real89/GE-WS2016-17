using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    private static GameController instance;

    private static List<Updateable> updateables = new List<Updateable>();

    private GameController() { instance = this; }


    public GameObject m_Boss;
    public GameObject m_Scarlet;

    public float m_ScarletStartHealth;
    public float m_ScarletHealth;
    //Value after last loss bar animation
    private float m_ScarletHealthOld;
    private float m_BossHealthOld;

    public Slider m_HealthBarScarlet;
    public Slider m_LossBarScarlet;
    public Slider m_HealthBarBoss;
    public Slider m_LossBarBoss;


    private static bool timesUpScarlet = false;
    private static bool timesUpBoss = false;

    private RectTransform rtLossScarlet;
    private RectTransform rtHealthScarlet;
    private RectTransform rtLossBoss;
    private RectTransform rtHealthBoss;
    private float fullWidth;
    private float lossHeight;
    private float healthHeight;
    private float elapsedTimeBoss = 0;
    private float elapsedTimeScarlet = 0;

    private bool isScarletDead = false;
    private bool isBossDead = false;

    public bool m_ScarletInvincible = false;

    public Text m_BossKillNotification;
    public Text m_DeathNotification;

    public float m_NotificationTime = 5.0f;


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
        if (!m_ScarletInvincible && m_ScarletHealth > 0)
        {
            PlayerShield shield = m_Scarlet.GetComponentInChildren<PlayerShield>();
            if (shield != null)
            {
                damage = shield.OnPlayerTakeDamage(damage);
            }

            m_ScarletHealth = Mathf.Max(0, m_ScarletHealth - damage);
            CalculateScarletHealthBar(damage / m_ScarletStartHealth);
            elapsedTimeScarlet = 0;
        }

        if (m_ScarletHealth <= 0) isScarletDead = true;
    }

    public void HealScarlet(float amount)
    {
        if(!isScarletDead)
        {
            float health = m_ScarletHealth + amount;

            m_ScarletHealth = (health > m_ScarletStartHealth) ? m_ScarletStartHealth : health;

            CalculateScarletHealthBar(-amount / m_ScarletStartHealth);
            elapsedTimeScarlet = 0;
        }
    }


    public void HitBoss(float damage)
    {
        if(m_Boss.GetComponent<BossHealth>().GetBossHealth() > 0)
        {
            if (m_Boss.GetComponent<BossHealth>().GetBossHealth() - damage < 0) {
                damage -= m_Boss.GetComponent<BossHealth>().GetBossHealth();
            }
            m_Boss.GetComponent<BossHealth>().TakeDamage(damage);
            CalculateBossHealthBar(damage / m_Boss.GetComponent<BossHealth>().GetMaxBossHealth());
            elapsedTimeBoss = 0;
        }

        if (m_Boss.GetComponent<BossHealth>().GetBossHealth() <= 0.0f) isBossDead = true;
    }


    // Use this for initialization
    void Start ()
    {
        m_BossKillNotification.enabled = false;
        m_BossKillNotification.text = "ASCENDED";

        m_DeathNotification.enabled = false;
        m_DeathNotification.text = "EXORCISED";

        isScarletDead = false;
        isBossDead = false;

        Image imgHealthScarlet = m_HealthBarScarlet.transform.FindChild("Fill Area").GetChild(0).GetComponent<Image>();
        Image imgLossScarlet = m_LossBarScarlet.transform.FindChild("Fill Area 2").GetChild(0).GetComponent<Image>();
        Image imgHealthBoss = m_HealthBarBoss.transform.FindChild("Fill Area Boss").GetChild(0).GetComponent<Image>();
        Image imgLossBoss = m_LossBarBoss.transform.FindChild("Fill Area Boss 2").GetChild(0).GetComponent<Image>();
        rtLossScarlet = imgLossScarlet.rectTransform;
        rtHealthScarlet = imgHealthScarlet.rectTransform;
        rtLossBoss = imgLossBoss.rectTransform;
        rtHealthBoss = imgHealthBoss.rectTransform;

        fullWidth = rtHealthScarlet.rect.width;
        lossHeight = rtLossScarlet.rect.height;
        healthHeight = rtHealthScarlet.rect.height;

        rtLossScarlet.sizeDelta = new Vector2(0, lossHeight);
        rtLossScarlet.anchoredPosition = new Vector2(fullWidth, 0);
        rtLossBoss.sizeDelta = new Vector2(0, lossHeight);
        rtLossBoss.anchoredPosition = new Vector2(fullWidth, 0);
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

        elapsedTimeScarlet += Time.deltaTime;
        if (elapsedTimeScarlet >= 1 && m_ScarletHealthOld != m_ScarletHealth)
        {
            m_ScarletHealthOld = m_ScarletHealth;
            timesUpScarlet = true;
            elapsedTimeScarlet = 0;
        }

        elapsedTimeBoss += Time.deltaTime;
        if (elapsedTimeBoss >= 1 && m_BossHealthOld != m_Boss.GetComponent<BossHealth>().GetBossHealth())
        {
            m_BossHealthOld = m_Boss.GetComponent<BossHealth>().GetBossHealth();
            timesUpBoss = true;
            elapsedTimeBoss = 0;
        }

        float healthPercentageScarlet = Mathf.Max(m_ScarletHealth, 0) / m_ScarletStartHealth;
        float healthPercentageBoss = Mathf.Max(m_Boss.GetComponent<BossHealth>().GetBossHealth(), 0) / m_Boss.GetComponent<BossHealth>().GetMaxBossHealth();

        Image imgS = m_HealthBarScarlet.transform.FindChild("Fill Area").GetChild(0).GetComponent<Image>();
        Image imgB = m_HealthBarBoss.transform.FindChild("Fill Area Boss").GetChild(0).GetComponent<Image>();
        imgS.color = Color.Lerp(Color.black, Color.red, healthPercentageScarlet);
        imgB.color = Color.Lerp(Color.black, Color.red, healthPercentageBoss);

        if (timesUpScarlet)
        {
            var endHealthScarlet = rtHealthScarlet.rect.width + rtHealthScarlet.anchoredPosition.x;
            var endLossScarlet = rtLossScarlet.rect.width + rtLossScarlet.anchoredPosition.x - endHealthScarlet - 10;

            rtLossScarlet.sizeDelta = new Vector2(Mathf.Max(0, endLossScarlet), lossHeight);


            if (endLossScarlet <= 0)
            {
                timesUpScarlet = false;
            }
        }
        if (timesUpBoss)
        {
            var endHealthBoss = rtHealthBoss.rect.width + rtHealthBoss.anchoredPosition.x;
            var endLossBoss = rtLossBoss.rect.width + rtLossBoss.anchoredPosition.x - endHealthBoss - 10;

            rtLossBoss.sizeDelta = new Vector2(Mathf.Max(0, endLossBoss), lossHeight);


            if (endLossBoss <= 0)
            {
                timesUpBoss = false;
            }
        }
                           
        HandleText(isBossDead, m_BossKillNotification);
        HandleText(isScarletDead, m_DeathNotification);       
    }

    /// <summary>
    /// Animates the Healthbar after attack
    /// </summary>
    /// <param name="damage">Loss of health from current attack</param>
    private void CalculateScarletHealthBar(float damage)
    {
        //prior width of health bar
        float endHealth = rtHealthScarlet.rect.width + rtHealthScarlet.anchoredPosition.x;
        //prior width of loss bar
        float endLoss = Mathf.Max(0, rtLossScarlet.rect.width + rtLossScarlet.anchoredPosition.x - endHealth);
        //start of loss bar and width of health bar
        float newEndHealth;
        //width of loss bar
        float newEndLoss;

        if (damage > 0)
        {
            newEndHealth = Mathf.Max(0, endHealth - fullWidth * damage);
            newEndLoss = Mathf.Min(fullWidth, endLoss + fullWidth * damage);
        }
        else
        {
            //Scarlet is healed and damage is negative
            newEndHealth = Mathf.Min(fullWidth, endHealth - fullWidth * damage);
            newEndLoss = Mathf.Max(0, endLoss + fullWidth * damage);
        }


        rtLossScarlet.anchoredPosition = new Vector2(newEndHealth, 0);
        rtLossScarlet.sizeDelta = new Vector2(newEndLoss, lossHeight);

        rtHealthScarlet.sizeDelta = new Vector2(newEndHealth, healthHeight);
    }

    /// <summary>
    /// Animates the Healthbar of the boss after attack
    /// </summary>
    /// <param name="damage">Loss of health from current attack</param>
    private void CalculateBossHealthBar(float damage)
    {
        var endHealth = rtHealthBoss.rect.width + rtHealthBoss.anchoredPosition.x;
        var endLoss = Mathf.Max(0, rtLossBoss.rect.width + rtLossBoss.anchoredPosition.x - endHealth);

        var newEndHealth = Mathf.Max(0, endHealth - fullWidth * damage);

        var newEndLoss = endLoss + fullWidth * damage;

        rtLossBoss.anchoredPosition = new Vector2(newEndHealth, 0);
        rtLossBoss.sizeDelta = new Vector2(newEndLoss, lossHeight);

        rtHealthBoss.sizeDelta = new Vector2(newEndHealth, healthHeight);
    }

    private void HandleText(bool isDead, Text text)
    {       
        if(isDead)
        {
            m_NotificationTime -= Time.deltaTime;

            if (m_NotificationTime > 0)
            {
                text.enabled = true;

            }
            else if(m_NotificationTime <= 1.5f)
            {
                GameObject.Find("_MainObject").GetComponent<Fading>().SetFadeSpeed(0.2f);
                GameObject.Find("_MainObject").GetComponent<Fading>().BeginFade(1);
            }  
        }            
    }
}
