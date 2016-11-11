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
        if (m_ScarletHealth > 0)
        {
            m_ScarletHealth -= damage;
            CalculateHealth(damage / m_ScarletStartHealth);
            elapsedTime = 0;
        }
    }

    

    // Use this for initialization
    void Start () {
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
            Debug.Log("endLoss: " + test);

            rtLoss.sizeDelta = new Vector2(endLoss < 0 ? 0 : endLoss, lossHeight);

            if (endLoss <= 0)
            {
                Debug.Log("Reset loose Health");
                timesUp = false;
            }
        }
    }

    /// <summary>
    /// Animates the Healthbar after attack
    /// </summary>
    /// <param name="loss">Loss of health from current attack</param>
    private void CalculateHealth(float loss)
    {
        var endHealth = rtHealth.rect.width + rtHealth.anchoredPosition.x;
        var endLoss = rtLoss.rect.width + rtLoss.anchoredPosition.x - endHealth;

        //Debug.Log("width: " + rtLoss.rect.width);
        //Debug.Log("anchored: " + rtLoss.anchoredPosition.x);
        var newEndHealth = endHealth - fullWidth * loss;
        var newEndLoss = endLoss + fullWidth * loss;

        //Debug.Log("newEndLoss: " + newEndLoss);
        //Debug.Log("newEndHealth: " + newEndHealth);

        rtLoss.anchoredPosition = new Vector2(newEndHealth, 0);
        rtLoss.sizeDelta = new Vector2(newEndLoss, lossHeight);

        rtHealth.sizeDelta = new Vector2(newEndHealth, healthHeight);
    }
}
