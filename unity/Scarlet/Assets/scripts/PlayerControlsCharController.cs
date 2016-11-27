using UnityEngine;
using System.Collections;

public class PlayerControlsCharController : MonoBehaviour
{
    public enum ControlMode { Exploration, BossFight, Disabled };
    private enum ParryState { NoParry, Perfect, Regular, AdditionalDamage };

    public enum AttackType { Regular, LastHitInCombo };

    public GameObject trailContainer;

    public AudioClip m_PunchAudio;
    public AudioClip m_GetHitAudio;
    public AudioClip m_BlockAudio;
    public AudioClip m_StaggerAudio;

    private AudioSource m_StepSource;
    private AudioSource m_EffectSource;

    public float m_HorizontalInput;
    public float m_VerticalInput;

    public float m_StartHealth;
    public float m_Health;
    //Value after last loss bar animation
    public float m_HealthOld;

    public float m_SpeedRun;
    public float m_SpeedWalk;
    public float m_DashDistance;
    public float m_DashSpeed;
    public float m_DashCooldown;
    public float m_ClimbingDistance;

    private float m_LastDash;
    private float m_CurrentSpeed;
    private float lastStepSound;

    public bool m_ControlsEnabled = true;
    public bool canSprint;
    public ControlMode currentControlMode;
    private Rigidbody m_RigidBody;
    private Animator animator;

    public float m_healingCharges;
    public float m_healingAmount;

    public float m_PerfectParryTime = 0.1f;
    public float m_RegularParryTime = 0.25f;
    public float m_AdditionalDamageTime = 0.4f;
    private float m_LastParry;
    public float m_ParryCooldown;
    private ParryState m_Parrying = ParryState.NoParry;
    private IEnumerator m_ParryIEnumerator;

    private HandDamage handDamage;
    private bool m_InAttackAnimation;
    private IEnumerator m_DamageCoRoutine;
    private TrailRenderer m_TrailRenderer;

    private bool isClimbing;
    private int m_CurrentAttackCombo = 0;

    private void Awake()
    {
        m_TrailRenderer = trailContainer.GetComponent<TrailRenderer>();
        m_RigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        canSprint = true;
        currentControlMode = ControlMode.Exploration;

        handDamage = GetComponentInChildren<HandDamage>();
        m_CurrentAttackCombo = 0;

        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (AudioSource s in sources)
        {
            if (s.clip != null)
            {
                m_StepSource = s;
            }
            else
            {
                m_EffectSource = s;
            }
        }
    }

    public void Step()
    {
        //avoid spamming step sounds in animation transition
        if (Time.realtimeSinceStartup - lastStepSound > 0.2f)
        {
            lastStepSound = Time.realtimeSinceStartup;
            m_StepSource.Play();
        }
    }

    void Update()
    {
        if (!m_ControlsEnabled)
            return;

        m_HorizontalInput = Input.GetAxis("Horizontal");
        m_VerticalInput = Input.GetAxis("Vertical");

        Move();
        Rotate();
        CheckGround();
        if (currentControlMode == ControlMode.BossFight)
        {
            m_CurrentSpeed = m_SpeedRun;
            CheckParry();
            CheckDash();
            CheckAttack();
            CheckHealing();
        }
        else if (currentControlMode == ControlMode.Exploration)
        {
            CheckSprint();
        }
    }

    private void CheckGround()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, 0.2f) && !isClimbing)
        {
            m_RigidBody.AddForce(Vector3.down * 50);
        }
    }

    // move scarlet in the right direction
    void Move()
    {
        float normalizedSpeed = (Mathf.Abs(m_HorizontalInput) + Mathf.Abs(m_VerticalInput)) * m_CurrentSpeed;

        if (normalizedSpeed >= m_CurrentSpeed)
        {
            normalizedSpeed = m_CurrentSpeed;
        }

        if (normalizedSpeed >= 0.1f)
        {
            if (m_InAttackAnimation)
                CancelAttackAnimation();
        }
        Vector3 movement = new Vector3(m_HorizontalInput * normalizedSpeed, m_RigidBody.velocity.y, m_VerticalInput * normalizedSpeed);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.3f, transform.forward, out hit, 1f))
        {
            //Scarlet can only traverse slopes up to a certain angle
            float hitAngle = Vector3.Angle(transform.forward, hit.normal);
            if (hit.transform.tag != "Player" && hit.transform.tag != "Climbable" && hitAngle > 135)
            {
                movement = Vector3.zero;
                normalizedSpeed = 0;
            }
        }
        m_RigidBody.velocity = (movement);
        animator.SetFloat("Speed", normalizedSpeed);
    }

    void Rotate()
    {
        if (Mathf.Abs(m_HorizontalInput) <= 0.1f && Mathf.Abs(m_VerticalInput) <= 0.1f) return;


        float angle = Mathf.Atan2(m_HorizontalInput, m_VerticalInput);

        Quaternion rotation = Quaternion.Euler(0f, Mathf.Rad2Deg * angle, 0f);
        m_RigidBody.MoveRotation(rotation);
    }

    private void CheckParry()
    {
        if (Input.GetButtonDown("Parry"))
        {
            if (m_InAttackAnimation)
                CancelAttackAnimation();

            if (Time.time >= m_LastParry + m_ParryCooldown)
            {
                Parry();
                m_LastParry = Time.time;
            }
        }
    }

    private void CheckDash()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (m_InAttackAnimation)
                CancelAttackAnimation();

            if (Time.time >= m_LastDash + m_DashCooldown)
            {
                Dash();
                m_LastDash = Time.time;
            }
        }
    }

    private void CheckSprint()
    {
        RaycastHit hit;
        isClimbing = false;
        if (Input.GetButton("Jump") && canSprint)
        {
            m_CurrentSpeed = m_SpeedRun;
            if (Physics.Raycast(transform.position, transform.forward, out hit, m_ClimbingDistance))
            {
                if (hit.collider.tag == "Climbable")
                {
                    isClimbing = true;
                    m_RigidBody.velocity = new Vector3(0, 2, 0);
                }
            }
        }
        else
        {
            m_CurrentSpeed = m_SpeedWalk;
        }
        animator.SetBool("Climbing", isClimbing);
    }

    private void CheckAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (m_InAttackAnimation)
            {
                StopCoroutine(m_DamageCoRoutine);
            }
            Punch();

        }
    }

    private void CheckHealing()
    {
        if (m_healingCharges > 0)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if (m_InAttackAnimation)
                    CancelAttackAnimation();

                m_healingCharges--;
                GameController.Instance.HealScarlet(m_healingAmount, m_healingCharges);
            }
        }
    }

    private void Dash()
    {
        m_ControlsEnabled = false;
        GameController.Instance.m_ScarletInvincible = true;
        Vector3 dashStart = m_RigidBody.transform.position;
        Vector3 dashTarget = m_RigidBody.transform.position + m_RigidBody.transform.forward * m_DashDistance;

        StartCoroutine(Blink(dashStart, dashTarget));
    }

    private void Parry()
    {
        m_ControlsEnabled = false;
        m_Parrying = ParryState.Perfect;

        m_RigidBody.velocity = new Vector3();

        animator.SetTrigger("BlockTrigger");
        m_ParryIEnumerator = SetParryState(ParryState.Regular, m_PerfectParryTime);
        StartCoroutine(m_ParryIEnumerator);
    }

    private IEnumerator SetParryState(ParryState newState, float time)
    {
        yield return new WaitForSeconds(time);
        m_Parrying = newState;

        if (newState == ParryState.Regular)
        {
            m_ParryIEnumerator = SetParryState(ParryState.AdditionalDamage, m_RegularParryTime);
            StartCoroutine(m_ParryIEnumerator);
        }
        else if (newState == ParryState.AdditionalDamage)
        {
            PlayStaggerAudio();
            m_ParryIEnumerator = SetParryState(ParryState.NoParry, m_AdditionalDamageTime);
            StartCoroutine(m_ParryIEnumerator);
            animator.SetTrigger("StaggerTrigger");
        }
        else if (newState == ParryState.NoParry)
        {
            animator.SetTrigger("IdleTrigger");
            m_ControlsEnabled = true;
        }
    }

    private void Punch()
    {
        float enableControlsAfter = 0.25f;
        int punchAnimation = 1;

        m_RigidBody.velocity = new Vector3();

        if (m_CurrentAttackCombo <= 2)
        {
            handDamage.m_CurrentAttackType = AttackType.Regular;
            punchAnimation = m_CurrentAttackCombo * 3 + (int)Random.Range(1.001f, 3.999f);
            m_CurrentAttackCombo++;
        }
        else
        {
            handDamage.m_CurrentAttackType = AttackType.LastHitInCombo;
            enableControlsAfter = 0.8f;
            punchAnimation = 10;

            m_CurrentAttackCombo = 0;
        }

        animator.SetInteger("PunchCue", punchAnimation);
        animator.SetTrigger("PunchTrigger");
        PlayPunchSound();

        handDamage.m_CauseDamage = true;

        m_ControlsEnabled = false;
        m_InAttackAnimation = true;

        StartCoroutine(ReEnableControlsAfter(enableControlsAfter));
        m_DamageCoRoutine = DisableDamageAfter(0.5f);

        StartCoroutine(m_DamageCoRoutine);
    }

    public float GetHit(GameObject attacker, float damage, bool blockable)
    {
        if (blockable)
        {
            damage = ParryAttack(attacker, damage);
            if (damage <= 0)
                PlayBlockSound();
            else
                PlayGetHitSound();
        }

        if (damage <= 0)
            return 0;

        PlayerShield shield = GetComponentInChildren<PlayerShield>();
        if (shield != null)
        {
            damage = shield.OnPlayerTakeDamage(damage);
        }

        m_Health = Mathf.Max(0, m_Health - damage);

        return damage;
    }

    private float ParryAttack(GameObject attacker, float damage)
    {
        if (m_Parrying == ParryState.Perfect)
        {
            PerfectParry(attacker);
            return 0;
        }
        else if (m_Parrying == ParryState.Regular)
        {
            RegularParry(attacker);
            return 0;
        }
        else if (m_Parrying == ParryState.NoParry)
        {
            return damage;
        }
        else if (m_Parrying == ParryState.AdditionalDamage)
        {
            Stagger();
            return damage * 1.3f;
        }

        return damage;
    }

    private void PerfectParry(GameObject attacker)
    {
        // @todo refactor to: Behaviour that can also handle staggering etc.
        AttackPattern pattern = attacker.GetComponent<AttackPattern>();
        if (pattern != null)
        {
            if (pattern.m_CurrentAttack != null)
            {
                pattern.m_CurrentAttack.CancelAttack();
            }
        }

        GameController.Instance.BrieflyBlurCamera();
        StopCoroutine(m_ParryIEnumerator);
        m_ControlsEnabled = true;
        m_Parrying = ParryState.NoParry;
    }

    private void RegularParry(GameObject attacker)
    {
        // @todo refactor to: Behaviour that can also handle staggering etc.
        AttackPattern pattern = attacker.GetComponent<AttackPattern>();
        if (pattern != null)
        {
            if (pattern.m_CurrentAttack != null)
            {
                pattern.m_CurrentAttack.ParryAttack();
            }
        }

        StopCoroutine(m_ParryIEnumerator);
        m_ControlsEnabled = true;
        m_Parrying = ParryState.NoParry;
    }

    private void Stagger()
    {
        StopCoroutine(m_ParryIEnumerator);
        m_ControlsEnabled = false;
        m_Parrying = ParryState.NoParry;

        animator.SetTrigger("StaggerTrigger");
        StartCoroutine(ReEnableControlsAfter(0.3f));
    }

    private void SetVisibility(bool visible)
    {
        PlayerShield shield = GetComponentInChildren<PlayerShield>();
        bool shieldActive = false;
        Renderer shieldRenderer = null;

        if (shield != null)
        {
            shieldRenderer = shield.GetComponent<Renderer>();
            shieldActive = shield.m_ShieldActive;
        }


        for (int i = 0; i < transform.childCount; i++)
        {
            Renderer r = transform.GetChild(i).GetComponent<Renderer>();
            if (r != null)
            {
                if (shieldActive || (!shieldActive && shieldRenderer != null && r != shieldRenderer))
                    r.enabled = visible;
            }
        }
    }

    private bool InAttackAnimation()
    {
        AnimatorStateInfo info = this.animator.GetCurrentAnimatorStateInfo(0);
        return info.IsTag("Punch");
    }

    private IEnumerator Blink(Vector3 dashStart, Vector3 dashTarget)
    {
        float t = 0;
        SetVisibility(false);
        m_TrailRenderer.Clear();
        m_TrailRenderer.time = 1;
        while (t < 1)
        {
            yield return null;
            t += Time.deltaTime / m_DashSpeed;
            m_RigidBody.transform.position = Vector3.Lerp(dashStart, dashTarget, t);
        }
        m_RigidBody.MovePosition(dashTarget);
        m_TrailRenderer.time = 0;
        SetVisibility(true);
        m_ControlsEnabled = true;
        GameController.Instance.m_ScarletInvincible = false;
    }

    private IEnumerator ReEnableControlsAfter(float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);

        m_ControlsEnabled = true;
    }

    private IEnumerator DisableDamageAfter(float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        DisableDamage();
        m_InAttackAnimation = false;
    }

    private void DisableDamage()
    {
        handDamage.m_CauseDamage = false;
        m_CurrentAttackCombo = 0;
    }

    // Attack could have continued to combo at this point, but the user moved / blinked / etc.
    private void CancelAttackAnimation()
    {
        animator.SetTrigger("StopTrigger");
        m_InAttackAnimation = false;
        DisableDamage();
        StopCoroutine(m_DamageCoRoutine);
    }

    private void PlayGetHitSound()
    {
        if (m_EffectSource != null)
        {
            m_EffectSource.clip = m_GetHitAudio;
            m_EffectSource.Play();
        }
    }

    private void PlayBlockSound()
    {
        if (m_EffectSource != null)
        {
            m_EffectSource.clip = m_BlockAudio;
            m_EffectSource.Play();
        }
    }

    private void PlayPunchSound()
    {
        if (m_EffectSource != null)
        {
            m_EffectSource.clip = m_PunchAudio;
            m_EffectSource.Play();
        }
    }

    private void PlayStaggerAudio()
    {
        if (m_EffectSource != null)
        {
            m_EffectSource.clip = m_StaggerAudio;
            m_EffectSource.Play();
        }
    }

    public void CancelMovement()
    {
        m_RigidBody.velocity = Vector3.zero;
        animator.SetFloat("Speed", 0);
    }
}
