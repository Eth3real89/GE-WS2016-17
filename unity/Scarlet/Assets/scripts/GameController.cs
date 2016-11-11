using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public class GameController : MonoBehaviour {

    private static GameController instance;

    private static List<Updateable> updateables = new List<Updateable>();

    private GameController() { instance = this; }


    public GameObject m_Boss;
    public GameObject m_Scarlet;

    public float m_ScarletStartHealth;
    public float m_ScarletHealth;
    //Value after last loss bar animation
    public float m_ScarletHealthOld;

    public Slider m_HealthBarSlider;
    public Slider m_HealthLossSlider;


    private static Timer aTimer;
    private static bool timesUp = false;

    private RectTransform rtLoss;
    private RectTransform rtHealth;
    private float fullWidth;
    private float lossHeight;
    private float healthHeight;


    public bool m_ScarletInvincible = false;

    public Text m_BossKillNotification;
    public Text m_DeathNotification;

    public float m_NotificationTime = 5.0f;

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
        if (!m_ScarletInvincible && m_ScarletHealth > 0)
        {
            m_ScarletHealth -= damage;
            CalculateHealth(damage / m_ScarletStartHealth);
            elapsedTime = 0;
        }

        if (m_ScarletHealth <= 0) isScarletDead = true;
    }


    public void HitBoss(float damage)
    {
        m_Boss.GetComponent<BossHealth>().TakeDamage(damage);

        if (m_Boss.GetComponent<BossHealth>().GetBossHealth() <= 0.0f) isBossDead = true;
    }

    public void HealScarlet(float amount)
    {
        float health = m_ScarletHealth + amount;

        m_ScarletHealth = (health > m_ScarletStartHealth) ? m_ScarletStartHealth : health;
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

        Image imgHealth = m_HealthBarSlider.transform.FindChild("Fill Area").GetChild(0).GetComponent<Image>();
        Image imgLoss = m_HealthLossSlider.transform.FindChild("Fill Area 2").GetChild(0).GetComponent<Image>();
        rtLoss = imgLoss.rectTransform;
        rtHealth = imgHealth.rectTransform;

        fullWidth = rtHealth.rect.width;
        lossHeight = rtLoss.rect.height;
        healthHeight = rtHealth.rect.height;

        rtLoss.sizeDelta = new Vector2(0, lossHeight);
        rtLoss.anchoredPosition = new Vector2(fullWidth, 0);
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
    float elapsedTime = 0;

    // Update is called once per frame
    void Update () {
        lock (updateables)
        {
            for(int i = 0; i < updateables.Count; i++)
                updateables[i].Update();
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= 1 && m_ScarletHealthOld != m_ScarletHealth)
        {
            m_ScarletHealthOld = m_ScarletHealth;
            timesUp = true;
            elapsedTime = 0;
        }
        
        float healthPercentage = Mathf.Max(m_ScarletHealth, 0) / m_ScarletStartHealth;

        //m_HealthBarSlider.value = healthPercentage * 100f;
        Image img = m_HealthBarSlider.transform.FindChild("Fill Area").GetChild(0).GetComponent<Image>();
        img.color = Color.Lerp(Color.black, Color.red, healthPercentage);

        if (timesUp)
        {
            var endHealth = rtHealth.rect.width + rtHealth.anchoredPosition.x;
            var endLoss = rtLoss.rect.width + rtLoss.anchoredPosition.x - endHealth - 10;
            var test = endLoss < 0 ? 0 : endLoss;

            rtLoss.sizeDelta = new Vector2(endLoss < 0 ? 0 : endLoss, lossHeight);

            if (endLoss <= 0)
            {
                timesUp = false;
            }
        }
        
        HandleText(isBossDead, m_BossKillNotification);
        HandleText(isScarletDead, m_DeathNotification);
    }

    /// <summary>
    /// Animates the Healthbar after attack
    /// </summary>
    /// <param name="loss">Loss of health from current attack</param>
    private void CalculateHealth(float loss)
    {
        var endHealth = rtHealth.rect.width + rtHealth.anchoredPosition.x;
        var endLoss = rtLoss.rect.width + rtLoss.anchoredPosition.x - endHealth;
        
        var newEndHealth = endHealth - fullWidth * loss;
        var newEndLoss = endLoss + fullWidth * loss;

        rtLoss.anchoredPosition = new Vector2(newEndHealth, 0);
        rtLoss.sizeDelta = new Vector2(newEndLoss, lossHeight);

        rtHealth.sizeDelta = new Vector2(newEndHealth, healthHeight);
    }


    private void HandleText(bool isDead, Text text)
    {
        if (isDead)
        {
            m_NotificationTime -= Time.deltaTime;

            if (m_NotificationTime > 0)
            {
                text.enabled = true;

            }
            else
            {
                text.enabled = false;
            }
        }
    }
}
