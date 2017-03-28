using UnityEngine;

public class BossMoveCommand : BossCommand {

    public float m_Speed;
    public Rigidbody m_BossBody;

    public AudioSource m_StepsAudio;
    
    public override void InitCommand(GameObject m_Boss, Animator m_Animator)
    {
        base.InitCommand(m_Boss, m_Animator);
        m_BossBody = m_Boss.GetComponent<Rigidbody>();
    }

    public void DoMove(float horizontal, float vertical)
    {
        if (m_BossBody == null)
            return;

        Vector3 movement = CalculateMovemend(horizontal, vertical);

        m_BossBody.velocity = movement;

        m_Animator.SetFloat("Speed", movement.magnitude);

        if (m_StepsAudio != null)
        {
            if (m_StepsAudio.isPlaying && movement.magnitude <= 0.2)
                m_StepsAudio.Stop();
            else if (!m_StepsAudio.isPlaying && movement.magnitude >= 0.2)
                m_StepsAudio.Play();
        }
    }

    protected virtual Vector3 CalculateMovemend(float horizontal, float vertical)
    {
        Vector3 movement = new Vector3(horizontal, 0, vertical);

        if (movement.magnitude > 1)
            movement.Normalize();

        movement *= m_Speed;
        return movement;
    }

    public void DoMove(float horizontal, float vertical, Vector3 target)
    {
        if (m_BossBody == null)
            return;

        Vector3 movement = CalculateMovemend(horizontal, vertical);

        float dist = Vector3.Distance(m_Boss.transform.position, target);
        if (movement.magnitude * Time.deltaTime * 4 > dist) // rough estimate to fix an issue based on another estimate ~.~
                                                            // (first estimate: boss's position is about the same as its melee dmg. trigger)
                                                            // (second estimate: if scarlet/target is almost in range, the boss should barely move; that's what's in this condition.)
        {
            movement *= dist / movement.magnitude;
        }
        
        m_BossBody.velocity = movement;
        m_Animator.SetFloat("Speed", movement.magnitude);

        if (m_StepsAudio != null)
        {
            if (m_StepsAudio.isPlaying && movement.magnitude <= 0.2)
                m_StepsAudio.Stop();
            else if (!m_StepsAudio.isPlaying && movement.magnitude >= 0.2)
                m_StepsAudio.Play();
        }
    }

    public void StopMoving()
    {
        m_Animator.SetFloat("Speed", 0);
        m_BossBody.velocity = new Vector3(0, 0, 0);

        if (m_StepsAudio != null && m_StepsAudio.isPlaying)
            m_StepsAudio.Stop();
    }

}
