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

    private GameController() {
        instance = this;
    }


    public GameObject m_Boss;
    public GameObject m_Scarlet;
    public GameObject m_MainCamera;

    public GameObject m_Capsule;

    private PlayerControlsCharController m_ScarletController;

    private float m_BossHealthOld;

    public Slider m_HealthBarScarlet;
    public Slider m_LossBarScarlet;
    public Slider m_HealthBarBoss;
    public Slider m_LossBarBoss;

    public AudioClip m_AscensionSound;
    public AudioClip m_DefeatSound;
    public AudioClip m_Heartbeat;
    private AudioSource m_AudioSource;
    private bool m_HeartbeatPlaying;

    public Image HealingCharge1;
    public Image HealingCharge2;
    public Image HealingCharge3;



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
    private float healingHealth = 0;

    private bool isHealing = false;
    private bool isScarletDead = false;
    private bool isBossDead = false;

    public bool m_ScarletInvincible = false;

    public Text m_BossKillNotification;
    public Text m_DeathNotification;

    public float m_NotificationTime = 5.0f;
    private RadialBlur m_RadialBlur;

    private OnCollectibleVFX m_OnCollectibleVFX;

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

    public void HitScarlet(GameObject attacker, float damage, bool blockable)
    {
        if (!m_ScarletInvincible && m_ScarletController.m_Health > 0)
        {
            damage = m_ScarletController.GetHit(attacker, damage, blockable);

            CalculateScarletHealthBar(damage / m_ScarletController.m_StartHealth);
            elapsedTimeScarlet = 0;
        }

        if (m_ScarletController.m_Health <= m_ScarletController.m_StartHealth * 0.15f)
        {
            if (!m_HeartbeatPlaying)
                StartPlayingHeartbeat();
        }

        if (m_ScarletController.m_Health <= 0)
        {
            PlayDefeatSound();
            isScarletDead = true;
        }
    }


    public void HealScarlet(float amount, float chargesLeft)
    {
        if (!isScarletDead)
        {
            isHealing = true;
            if (chargesLeft >= 3)
            {
                HealingCharge1.enabled = true;
                HealingCharge2.enabled = true;
                HealingCharge3.enabled = true;
            }
            else if (chargesLeft == 2)
            {
                HealingCharge1.enabled = true;
                HealingCharge2.enabled = true;
                HealingCharge3.enabled = false;
            }
            else if (chargesLeft == 1)
            {
                HealingCharge1.enabled = true;
                HealingCharge2.enabled = false;
                HealingCharge3.enabled = false;
            }
            else if (chargesLeft == 0)
            {
                HealingCharge1.enabled = false;
                HealingCharge2.enabled = false;
                HealingCharge3.enabled = false;
            }

            float health = m_ScarletController.m_Health + amount;

            m_ScarletController.m_Health = (health > m_ScarletController.m_Health) ? 
                m_ScarletController.m_StartHealth : health;

            CalculateScarletHealthBar(-amount / m_ScarletController.m_StartHealth);
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

            AttackPattern bossBehavior = m_Boss.GetComponent<AttackPattern>();
            if (bossBehavior != null)
            {
                bossBehavior.OnBossHit();
            }

            CalculateBossHealthBar(damage / m_Boss.GetComponent<BossHealth>().GetMaxBossHealth());
            elapsedTimeBoss = 0;
        }

        if (m_Boss.GetComponent<BossHealth>().GetBossHealth() <= 0.0f)
        {
            PlayAscensionSound();
            isBossDead = true;
        }
    }


    // Use this for initialization
    void Start ()
    {
        m_ScarletController = m_Scarlet.GetComponent<PlayerControlsCharController>();
        m_AudioSource = GetComponent<AudioSource>();
        m_HeartbeatPlaying = false;
        m_RadialBlur = m_MainCamera.GetComponent<RadialBlur>();
        m_OnCollectibleVFX = m_MainCamera.GetComponent<OnCollectibleVFX>();

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

        m_RadialBlur.blurDuration = 1f;
        m_RadialBlur.blurWidth = 1f;
        m_RadialBlur.blurStrength = 1.45f;
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
        if (elapsedTimeScarlet >= 1 && m_ScarletController.m_HealthOld != m_ScarletController.m_Health)
        {
            m_ScarletController.m_HealthOld = m_ScarletController.m_Health;
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

//        float healthPercentageScarlet = Mathf.Max(m_ScarletHealth, 0) / m_ScarletStartHealth;
//        float healthPercentageBoss = Mathf.Max(m_Boss.GetComponent<BossHealth>().GetBossHealth(), 0) / m_Boss.GetComponent<BossHealth>().GetMaxBossHealth();

        Image imgS = m_HealthBarScarlet.transform.FindChild("Fill Area").GetChild(0).GetComponent<Image>();
        Image imgB = m_HealthBarBoss.transform.FindChild("Fill Area Boss").GetChild(0).GetComponent<Image>();
//        imgS.color = Color.Lerp(Color.black, Color.red, healthPercentageScarlet);
//        imgB.color = Color.Lerp(Color.black, Color.red, healthPercentageBoss);

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
        var endHealthScarletPrior = rtHealthScarlet.rect.width + rtHealthScarlet.anchoredPosition.x;

        if (isHealing && endHealthScarletPrior < healingHealth)
        {
            var endHealthScarlet = endHealthScarletPrior + 15;
            rtHealthScarlet.sizeDelta = new Vector2(Mathf.Min(healingHealth, endHealthScarlet, fullWidth), healthHeight);
            if(healingHealth <= endHealthScarlet)
            {
                isHealing = false;
            }
        }

        if (m_HeartbeatPlaying && m_ScarletController.m_Health > m_ScarletController.m_StartHealth * 0.15f)
        {
            StopPlayingHeartbeat();
        }
        else if (m_ScarletController.m_Health <= m_ScarletController.m_StartHealth * 0.15f)
        {
            m_RadialBlur.blurDuration = 1f;
            m_RadialBlur.blurWidth = 1f;
            m_RadialBlur.blurStrength = 1.45f;
            m_RadialBlur.haveRedBlurArea = true;

            m_RadialBlur.Reset();

        }

        HandleText(isBossDead, m_BossKillNotification);
        HandleText(isScarletDead, m_DeathNotification);  

        HandleCollectibleVisualization();
    }

    private void HandleCollectibleVisualization()
    {
        if(m_Capsule != null && m_Capsule.GetComponent<Renderer>().isVisible)
        {
            Vector3 target = m_MainCamera.GetComponent<Camera>().WorldToScreenPoint(m_Capsule.transform.position);
            m_OnCollectibleVFX.Activate(target);
        }
        else
        {
            m_OnCollectibleVFX.Deactivate();
        }
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
            if(newEndHealth == 0)
            {
                newEndLoss = Mathf.Min(endLoss + endHealth, fullWidth, endLoss + fullWidth * damage);
            } else
            {
                newEndLoss = Mathf.Min(fullWidth, endLoss + fullWidth * damage);
            }

            rtLossScarlet.anchoredPosition = new Vector2(newEndHealth, 0);
            rtLossScarlet.sizeDelta = new Vector2(newEndLoss, lossHeight);

            rtHealthScarlet.sizeDelta = new Vector2(newEndHealth, healthHeight);
        }
        else
        {
            //Scarlet is healed and damage is negative
            newEndHealth = Mathf.Min(fullWidth, endHealth - fullWidth * damage);
            healingHealth = newEndHealth;
            newEndLoss = Mathf.Max(0, endLoss + fullWidth * damage);
        }


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

    private void PlayAscensionSound()
    {
        if (isBossDead)
            return;

        m_Boss.GetComponent<AttackPattern>().Die();
        if (m_AudioSource != null)
        {
            m_AudioSource.clip = m_AscensionSound;
            m_AudioSource.loop = false;
            m_AudioSource.Play();
        }
    }

    private void PlayDefeatSound()
    {
        if (isScarletDead)
            return;
        if (m_AudioSource != null)
        {
            m_AudioSource.clip = m_DefeatSound;
            m_AudioSource.loop = false;
            m_AudioSource.Play();
        }
    }

    private void StartPlayingHeartbeat()
    {
        m_HeartbeatPlaying = true;
        if (m_AudioSource != null)
        {
            m_AudioSource.clip = m_Heartbeat;
            m_AudioSource.loop = true;
            m_AudioSource.Play();

        }
    }

    public void BrieflyBlurCamera(float time = 1f)
    {
        m_RadialBlur.blurDuration = time;
        m_RadialBlur.blurWidth = 1f;
        m_RadialBlur.blurStrength = 1.45f;
        m_RadialBlur.haveRedBlurArea = false;

        m_RadialBlur.Reset();
    }

    private void StopPlayingHeartbeat()
    {
        m_HeartbeatPlaying = false;
        if (m_AudioSource != null && m_AudioSource.isPlaying && m_AudioSource.clip == m_Heartbeat)
        {
            m_AudioSource.Stop();
        }
    }

}
