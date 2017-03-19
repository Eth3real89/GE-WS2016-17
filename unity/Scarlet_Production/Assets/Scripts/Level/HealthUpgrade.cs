using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthUpgrade : Interactor
{
    public float m_BuffAmount = 20;

    private int m_Index;

    private void Start()
    {
        m_Index = SceneManager.GetActiveScene().buildIndex;
        int isUsed = PlayerPrefs.GetInt("HealthUpgrade" + m_Index, 0);

        if (isUsed == 1)
            Destroy(gameObject);
    }

    public override void Interact()
    {
        CharacterHealth health = FindObjectOfType<CharacterHealth>();
        float currentMaxHP = PlayerPrefs.GetFloat("MaxHP", health.m_MaxHealth);
        PlayerPrefs.SetFloat("MaxHP", currentMaxHP + m_BuffAmount);
        PlayerPrefs.SetInt("HealthUpgrade" + m_Index, 1);
        EffectController.Instance.EmpoweredPeak();
        Destroy(gameObject);
    }
}
