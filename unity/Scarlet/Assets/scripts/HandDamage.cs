using UnityEngine;
using System.Collections;

public class HandDamage : MonoBehaviour {

    public bool m_CauseDamage = false;

    public float m_Damage = 20f;

    public AudioSource m_AudioSource;

    public PlayerControlsCharController.AttackType m_CurrentAttackType = PlayerControlsCharController.AttackType.Regular;

	// Use this for initialization
	void Start () {
        m_AudioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnTriggerEnter(Collider other)
    {
        CheckHit(other);
    }

    void OnTriggerStay(Collider other)
    {
        CheckHit(other);
    }

    private void CheckHit(Collider other)
    {
        if (m_CauseDamage)
        {
            if (GameController.Instance.IsBoss(other.GetComponent<Rigidbody>()))
            {
                PlayHitAudio();
                GameController.Instance.HitBoss(m_Damage, m_CurrentAttackType);
                m_CauseDamage = false;
            }
            else
            {
                Bullet b = other.gameObject.GetComponentInParent<Bullet>();
                if (b != null)
                {
                    b.DestroyBullet();
                }
            }
        }
    }

    public void PlayHitAudio()
    {
        m_AudioSource.Play();
    }

}
