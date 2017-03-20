using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInRotatingConeAttack : GrowingThenRotatingConeAttack {

    public Material m_SeeThroughConeMaterial;

    public Color m_FlickerFromColorSetup = Color.black;
    public Color m_FlickerToColorSetup = Color.white;
    
    public float m_FlickerTimesSetup;

    public Color m_FlickerFromColorAttack = Color.gray;
    public Color m_FlickerToColorAttack = Color.white;

    public float m_FlickerTimesAttack;

    public bool m_FlickerOut = false;

    public float m_MinDistance = 0;

    /// <summary>
    /// how long flicker out lasts relative to flicker in.
    /// </summary>
    public float m_FadeInOutRatio = 4f;

    protected Color m_LastColor;

    public override void StartAttack()
    {
        base.StartAttack();
    }

    protected override IEnumerator Grow()
    {
        m_AttackVisuals.SetAngle(m_Angle);
        m_Damage.m_Angle = m_Angle;

        m_Damage.m_Distance = m_EndSize;
        m_Damage.m_MinDistance = m_MinDistance;
        m_Damage.m_Owner = m_AttackContainer.gameObject;
        m_Damage.m_TurnTowardsScarlet = m_FullTurnCommand;
        m_Damage.m_Callback = this;

        m_AttackVisuals.SetRadius(m_EndSize);
        m_AttackVisuals.ShowAttack();
        m_AttackVisuals.SetStartRadius(m_MinDistance / m_EndSize);

        float t = 0;
        while ((t += Time.deltaTime) < m_GrowTime)
        {
            if (m_Angle != 0)
            {
                float colorDeterminer = 1 - Mathf.Abs(Mathf.Cos(t / m_GrowTime * (m_FlickerTimesSetup + 0.5f) * 180 * Mathf.Deg2Rad));
                m_AttackVisuals.SetColor(Color.Lerp(m_FlickerFromColorSetup, m_FlickerToColorSetup, colorDeterminer));
            }
            yield return null;
        }

        m_Damage.Activate();
        AfterGrow();
    }

    protected override void AfterGrow()
    {
        m_GrowEnumerator = Rotate();
        StartCoroutine(m_GrowEnumerator);
    }

    protected override IEnumerator Rotate()
    {
        float t = 0;
        while ((t += Time.deltaTime) < m_RotationTime)
        {
            if (m_DontRotateBoss)
            {
                m_Damage.transform.Rotate(Vector3.up, m_RotationAngle * Time.deltaTime / m_RotationTime);
            }
            else
            {
                m_Boss.transform.Rotate(Vector3.up, m_RotationAngle * Time.deltaTime / m_RotationTime);
            }


            float colorDeterminer = 1 - Mathf.Abs(Mathf.Cos(t / m_RotationTime * (m_FlickerTimesAttack + 0.5f) * 180 * Mathf.Deg2Rad));
            m_LastColor = Color.Lerp(m_FlickerFromColorAttack, m_FlickerToColorAttack, colorDeterminer);
            m_AttackVisuals.SetColor(m_LastColor);

            yield return null;
        }

        AfterRotate();
    }

    protected virtual void AfterRotate()
    {

        if (m_LeaveCone)
        {
            m_Callback.OnAttackEnd(this);
            return;
        }

        m_Damage.DisableDamage();
        m_Callback.OnAttackEnd(this);

        if (m_FlickerOut)
        {
            StartCoroutine(FlickerOut());
        }
        else
        {
            m_AttackVisuals.HideAttack();
        }
    }

    private IEnumerator FlickerOut()
    {
        float t = 0;
        while ((t += Time.deltaTime) < m_GrowTime / m_FadeInOutRatio)
        {
            m_AttackVisuals.SetColor(Color.Lerp(m_FlickerToColorSetup, Color.black, t / (m_GrowTime / m_FadeInOutRatio)));

            yield return null;
        }
        m_AttackVisuals.HideAttack();
    }

    public override void OnSuccessfulHit()
    {
        if (m_StaggerScarlet == StaggerScarlet.Hard || m_StaggerScarlet == StaggerScarlet.Mega)
        {
            Vector3 diff = PlayerStaggerCommand.ScarletPosition() - m_AttackContainer.position;

            if (diff.magnitude > m_MinDistance + (m_EndSize - m_MinDistance) / 2f)
            {
                PlayerStaggerCommand.StaggerScarletAwayFrom(m_AttackContainer.position, 2, true);

                if (m_StaggerScarlet == StaggerScarlet.Mega)
                {
                    PlayerStaggerCommand.ApplyForceAwayFrom(diff * 2 - new Vector3(0, 1, 0), diff.magnitude * 2);
                }
            }
            else
            {
                diff.y = 0;
                PlayerStaggerCommand.StaggerScarletAwayFrom(diff * 2 - new Vector3(0, 1, 0), 3, false);

                if (m_StaggerScarlet == StaggerScarlet.Mega)
                {
                    PlayerStaggerCommand.ApplyForceAwayFrom(diff * 2 - new Vector3(0, 1, 0), diff.magnitude / 2);
                }
            }
        }
        else
        {
            base.OnSuccessfulHit();
        }
    }

}
