using UnityEngine;
using System.Collections;

public class PlayerControlsCharController : MonoBehaviour
{
    public GameObject trailContainer;

    public float m_HorizontalInput;
    public float m_VerticalInput;

    public float m_Speed = 1.3f;
    public float m_DashDistance;
    public float m_DashSpeed;
    public float m_DashCooldown;

    private float m_LastDash;

    public bool m_ControlsEnabled = true;

    private Rigidbody m_RigidBody;
    private Animator animator;

    public float m_healingCharges;
    public float m_healingAmount;

    private HandDamage handDamage;
    private bool m_InAttackAnimation;
    private IEnumerator m_DamageCoRoutine;
    private TrailRenderer m_TrailRenderer;

    private int m_CurrentAttackCombo = 0;

    private void Awake()
    {
        m_TrailRenderer = trailContainer.GetComponent<TrailRenderer>();
        m_RigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        handDamage = GetComponentInChildren<HandDamage>();

        m_CurrentAttackCombo = 0;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!m_ControlsEnabled)
            return;

        m_HorizontalInput = Input.GetAxis("Horizontal");
        m_VerticalInput = Input.GetAxis("Vertical");

        Move();
        Rotate();
        CheckDash();
        CheckAttack();
        CheckHealing();
    }

    // move scarlet in the right direction
    void Move()
    {
        float normalizedSpeed = (Mathf.Abs(m_HorizontalInput) + Mathf.Abs(m_VerticalInput)) * m_Speed;
        if (normalizedSpeed >= m_Speed)
        {
            normalizedSpeed = m_Speed;
        }

        if (normalizedSpeed >= 0.1f)
        {
            if (m_InAttackAnimation)
                CancelAttackAnimation();
        }

        Vector3 movement = new Vector3(m_HorizontalInput * normalizedSpeed, 0, m_VerticalInput * normalizedSpeed);
        m_RigidBody.MovePosition(m_RigidBody.position + movement * Time.deltaTime);
        animator.SetFloat("Speed", normalizedSpeed);
    }

    // make sure scarlet is looking in the right direction
    void Rotate()
    {
        // don't want to change where Scarlet is looking when she is not moving
        if (Mathf.Abs(m_HorizontalInput) <= 0.1f && Mathf.Abs(m_VerticalInput) <= 0.1f) return;


        float angle = Mathf.Atan2(m_HorizontalInput, m_VerticalInput);

        Quaternion rotation = Quaternion.Euler(0f, Mathf.Rad2Deg * angle, 0f);
        m_RigidBody.MoveRotation(rotation);
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
                GameController.Instance.HealScarlet(m_healingAmount);
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

    private void Punch()
    {
        float enableControlsAfter = 0.25f;
        int punchAnimation = 1;

        if (m_CurrentAttackCombo <= 2)
        {
            Debug.Log("combo: " + m_CurrentAttackCombo);

            punchAnimation = m_CurrentAttackCombo * 3 + (int)Random.Range(1.001f, 3.999f);
            m_CurrentAttackCombo++;
        }
        else
        {
            Debug.Log("final hit in combo!");

            enableControlsAfter = 0.8f;
            punchAnimation = 10;

            m_CurrentAttackCombo = 0;
        }

        animator.SetInteger("PunchCue", punchAnimation);
        animator.SetTrigger("PunchTrigger");

        handDamage.m_CauseDamage = true;

        m_ControlsEnabled = false;
        m_InAttackAnimation = true;

        StartCoroutine(ReEnableControlsAfter(enableControlsAfter));
        m_DamageCoRoutine = DisableDamageAfter(0.5f);

        StartCoroutine(m_DamageCoRoutine);

    }

    private void SetVisibility(bool visible)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Renderer r = transform.GetChild(i).GetComponent<Renderer>();
            if (r != null)
                r.enabled = visible;
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
}
