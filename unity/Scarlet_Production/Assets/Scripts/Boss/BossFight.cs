using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossFight : MonoBehaviour {

    public static bool s_ScarletCanDie = true;
    protected IEnumerator m_ScarletHealthEnumerator;
    protected IEnumerator m_ResetEnumerator;

    protected Vector3 m_ScarletPositionStart;
    protected Quaternion m_ScarletRotationStart;

    protected Vector3 m_BossPositionStart;
    protected Quaternion m_BossRotationStart;

    public float m_MaxScarletRegenerationAfterPhase = 100f;
    protected IEnumerator m_ScarletRegenerationEnumerator;

    public virtual void StartBossfight()
    {
        StartCoroutine(StartAfterFirstFrame());
    }

    public virtual void RestartBossfight()
    {
        PlayerHittable playerHittable = FindObjectOfType<PlayerHittable>();
        CharacterHealth scarletHealth = playerHittable.GetComponent<CharacterHealth>();
        scarletHealth.m_CurrentHealth = scarletHealth.m_HealthStart;

        BossHittable bossHittable = FindObjectOfType<BossHittable>();
        CharacterHealth bossHealth = bossHittable.GetComponent<CharacterHealth>();
        bossHealth.m_CurrentHealth = bossHealth.m_HealthStart;

        StartBossfight();
        PlayerControls controls = FindObjectOfType<PlayerControls>();
        controls.EnableAllCommands();
    }

    protected virtual void StoreInitialState()
    {
        // this kind of abuses the fact that hittables always have to be where the colliders are, that is, they're definitely at the correct transform
        PlayerHittable playerHittable = FindObjectOfType<PlayerHittable>();
        BossHittable bossHittable = FindObjectOfType<BossHittable>();

        m_ScarletPositionStart = playerHittable.transform.position + Vector3.zero;
        m_ScarletRotationStart = Quaternion.Euler(playerHittable.transform.rotation.eulerAngles);

        m_BossPositionStart = bossHittable.transform.position + Vector3.zero;
        m_BossRotationStart = Quaternion.Euler(bossHittable.transform.rotation.eulerAngles);
    }

    protected virtual IEnumerator StartAfterFirstFrame()
    {
        yield return null;

        StoreInitialState();
        m_ScarletHealthEnumerator = CheckScarletHealth();
        StartCoroutine(m_ScarletHealthEnumerator);
    }

    protected virtual IEnumerator CheckScarletHealth()
    {
        PlayerHittable playerHittable = FindObjectOfType<PlayerHittable>();
        CharacterHealth scarletHealth = playerHittable.GetComponent<CharacterHealth>();

        while (true)
        {
            if (scarletHealth.m_CurrentHealth <= 0 && s_ScarletCanDie)
            {
                OnScarletDead();
            }
            yield return null;
        }
    }

    protected virtual void OnScarletDead()
    {
        StopAllCoroutines();

        FancyAudio.Instance.StopAll();

        ScarletVOPlayer.Instance.PlayDeathSound();

        PlayerControls controls = FindObjectOfType<PlayerControls>();
        controls.DisableAllCommands();

        SlowTime.Instance.m_PreventChanges = false;
        SlowTime.Instance.StopAllCoroutines();
        SlowTime.Instance.StopSlowMo();

        m_ResetEnumerator = ResetRoutine();
        StartCoroutine(m_ResetEnumerator);
    }

    protected virtual IEnumerator ResetRoutine()
    {
        GetComponent<DeathScreenController>().ShowVictoryScreen(gameObject);

        PlayerHealCommand healCommand = FindObjectOfType<PlayerHealCommand>();
        healCommand.ResetPotions();

        ResetInitialPositions();

        // @todo remove blood trails

        DestroyAllBullets();

        yield return null;
        //RestartBossfight();
    }

    protected virtual void ResetInitialPositions()
    {
        PlayerHittable playerHittable = FindObjectOfType<PlayerHittable>();
        BossHittable bossHittable = FindObjectOfType<BossHittable>();

        playerHittable.transform.position = m_ScarletPositionStart + new Vector3();
        playerHittable.transform.rotation = Quaternion.Euler(m_ScarletRotationStart.eulerAngles);

        bossHittable.transform.position = m_BossPositionStart + new Vector3();
        bossHittable.transform.rotation = Quaternion.Euler(m_BossRotationStart.eulerAngles);
    }

    protected virtual void DestroyAllBullets()
    {
        List<GameObject> toDestroy = new List<GameObject>();

        foreach(BulletBehaviour b in FindObjectsOfType<Bullet>())
        {
            b.StopAllCoroutines();
            if (b is Bullet)
                toDestroy.Add(b.gameObject);
        }

        for(int i = 0; i < toDestroy.Count; i++)
        {
            if (!toDestroy[i].activeInHierarchy)
                continue;

            try
            {
                Destroy(toDestroy[i]);
            }
            catch { }
        }
    }

    protected virtual void RegenerateScarletAfterPhase()
    {
        PlayerHittable playerHittable = FindObjectOfType<PlayerHittable>();
        CharacterHealth scarletHealth = playerHittable.GetComponent<CharacterHealth>();

        PlayerHealCommand healCommand = FindObjectOfType<PlayerHealCommand>();
        healCommand.m_NumHealthPotions++;

        m_ScarletRegenerationEnumerator = ScarletRegenerationRoutine(scarletHealth);
        StartCoroutine(m_ScarletRegenerationEnumerator);
    }

    protected virtual IEnumerator ScarletRegenerationRoutine(CharacterHealth health)
    {
        float newHealth = Mathf.Min(health.m_MaxHealth, health.m_CurrentHealth + m_MaxScarletRegenerationAfterPhase * health.m_MaxHealth);
        float healthGain = newHealth - health.m_CurrentHealth;

        float regTime = 0.5f;
        float healedAmount = 0;
        float t = 0;
        while((t += Time.deltaTime) < regTime)
        {
            float healStep = Time.deltaTime * (healthGain / regTime);
            health.m_CurrentHealth += healStep;
            healedAmount += healStep;
            yield return null;
        }

        health.m_CurrentHealth += healthGain - healedAmount;
    }

    protected virtual void StopPlayerMove()
    {
        PlayerMoveCommand moveCommand = FindObjectOfType<PlayerMoveCommand>();
        if (moveCommand != null)
            moveCommand.StopMoving();
    }

    protected virtual void SetScarletVoice(ScarletVOPlayer.Version version)
    {
        ScarletVOPlayer voPlayer = FindObjectOfType<ScarletVOPlayer>();
        if (voPlayer != null)
        {
            voPlayer.m_Version = version;
            voPlayer.SetupPlayers();
        }
    }

}

public interface BossfightCallbacks
{
    void PhaseEnd(BossController whichPhase);
}

