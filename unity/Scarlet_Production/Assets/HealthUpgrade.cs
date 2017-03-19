using UnityEngine;

public class HealthUpgrade : Interactor
{
    public float m_BuffAmount = 20;

    public override void Interact()
    {
        CharacterHealth health = FindObjectOfType<CharacterHealth>();
        float currentMaxHP = PlayerPrefs.GetFloat("MaxHP", health.m_MaxHealth);
        PlayerPrefs.SetFloat("MaxHP", currentMaxHP + m_BuffAmount);
        EffectController.Instance.EmpoweredPeak();
        Destroy(this);
    }
}
